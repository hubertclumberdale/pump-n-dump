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
    private bool isQueueInitialized = false;

    void Start()
    {
        Instance = this;

        StartCoroutine(InitializeQueue());
    }

    void Update()
    {
        if (!isQueueInitialized) return;  // Skip if queue isn't initialized yet

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
        isQueueInitialized = false;  // Make sure it's false at start

        for (int i = 0; i < queueSize; i++)
        {
            AddRandomCustomerToQueue();
            yield return AdvanceQueue();
        }

        isQueueInitialized = true;  // Queue is now fully initialized
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
        // Convert queue to array to avoid modification issues during iteration
        CustomerClass[] customersToMove = customerQueue.ToArray();
        
        foreach (CustomerClass customer in customersToMove)
        {
            if (customer != null && customer.gameObject != null)  // Add null check for safety
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

        // First wait for current customer to exit completely
        yield return StartCoroutine(MoveCustomerToExit(customerToExit));

        // Only after exit is complete, move the next customer
        yield return StartCoroutine(MoveCustomerToPlayPosition());
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