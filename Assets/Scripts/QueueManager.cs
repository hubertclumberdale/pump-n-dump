using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QueueManager : MonoBehaviour
{
    public Queue<CustomerClass> customerQueue; // La fila di persone
    public int queueSize = 10; // Dimensione della fila
    public GameObject customerPrefab; // Prefab del cliente
    public float animationDuration = 0.3f; // Durata dell'animazione
    public static QueueManager Instance; // Singleton
    public Transform spawnPoint; // Fixed missing semicolon
    public Transform playPosition; // Renamed back from combatPosition
    public Transform exitPosition; // Add this field for the exit point
    private CustomerClass currentPlayingCustomer; // Track customer at play position

    void Start()
    {
        Instance = this;

        StartCoroutine(InitializeQueue());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(MoveCustomerToPlayPosition());
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentPlayingCustomer != null)
        {
            StartCoroutine(HandleCustomerExit());
        }
    }

    // Inizializza la fila con persone casuali
    IEnumerator InitializeQueue()
    {
        customerQueue = new Queue<CustomerClass>();

        for (int i = 0; i < queueSize; i++)
        {
            AddRandomCustomerToQueue();
            yield return AdvanceQueue();
        }
    }

    // Aggiunge una persona casuale alla fine della fila
    void AddRandomCustomerToQueue()
    {
        GameObject newCustomerObject = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        CustomerClass newCustomer = newCustomerObject.GetComponent<CustomerClass>();
        customerQueue.Enqueue(newCustomer);
    }

    // Avanza la fila di una posizione
    public IEnumerator AdvanceQueue()
    {
        foreach (CustomerClass customer in customerQueue)
        {
            Vector3 currentPos = customer.transform.position;
            Vector3 targetPos = currentPos + new Vector3(0.65f, 0, -0.2f);

            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(customer.transform.DOMove(targetPos, animationDuration))
                       .Join(customer.transform.DOMoveY(currentPos.y + 0.1f, animationDuration * 0.5f)
                            .SetEase(Ease.OutQuad)
                            .SetLoops(2, LoopType.Yoyo));

            yield return moveSequence.WaitForCompletion();
        }
    }

    public IEnumerator MoveCustomerToPlayPosition() // Renamed back from MoveCustomerToCombatPosition
    {
        if (customerQueue.Count > 0)
        {
            CustomerClass customer = customerQueue.Dequeue();
            currentPlayingCustomer = customer; // Store reference
            
            // Move customer to play position
            yield return customer.transform.DOMove(playPosition.position, animationDuration)
                .SetEase(Ease.InOutQuad)
                .WaitForCompletion();

            // Advance the remaining queue
            yield return AdvanceQueue();

            // Add new customer at the end
            AddRandomCustomerToQueue();
        }
    }

    private IEnumerator HandleCustomerExit()
    {
        CustomerClass customerToExit = currentPlayingCustomer;
        currentPlayingCustomer = null;

        // Start moving next customer to play position
        StartCoroutine(MoveCustomerToPlayPosition());

        // Move current customer to exit
        yield return StartCoroutine(MoveCustomerToExit(customerToExit));
    }

    public IEnumerator MoveCustomerToExit(CustomerClass customer)
    {
        // Move customer to exit position
        yield return customer.transform.DOMove(exitPosition.position, animationDuration)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
            
        // Destroy the customer object after reaching exit
        Destroy(customer.gameObject);
    }
}