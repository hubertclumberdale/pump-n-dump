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
    private bool customerAtPlayPosition = false; // Add this flag

    public float moveToPlayDuration = 0.8f; // Adjusted duration for moving to play position
    public float moveToExitDuration = 0.8f; // Adjusted duration for moving to exit
    public int jumpCount = 3; // Number of jumps during movement
    public float jumpHeight = 0.1f; // Reduced height for smaller jumps
    public int jumpsPerSecond = 4; // New variable to control jump frequency

    void Start()
    {
        Instance = this;

        StartCoroutine(InitializeQueue());
    }

    void Update()
    {
        if (!isQueueInitialized) return;  // Skip if queue isn't initialized yet

        if (Input.GetKeyDown(KeyCode.A) && currentPlayingCustomer == null) // Only allow if play position is empty
        {
            StartCoroutine(MoveCustomerToPlayPosition());
        }
        else if (Input.GetKeyDown(KeyCode.S) && customerAtPlayPosition) // Changed condition
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
        if (customerQueue.Count > 0 && currentPlayingCustomer == null) // Double check that position is empty
        {
            CustomerClass customer = customerQueue.Dequeue();
            currentPlayingCustomer = customer; // Store reference
            
            Sequence moveSequence = DOTween.Sequence();
            
            // Base movement
            moveSequence.Append(customer.transform.DOMove(playPosition.position, moveToPlayDuration)
                .SetEase(Ease.Linear));
            
            // Calculate total jumps based on duration and frequency
            int totalJumps = Mathf.FloorToInt(moveToPlayDuration * jumpsPerSecond);
            float jumpDuration = moveToPlayDuration / totalJumps;

            // Add rapid small jumps
            for (int i = 0; i < totalJumps; i++)
            {
                moveSequence.Join(customer.transform.DOMoveY(
                    customer.transform.position.y + jumpHeight,
                    jumpDuration * 0.5f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.OutQuad)
                    .SetDelay(i * jumpDuration));
            }

            yield return moveSequence.WaitForCompletion();
            customerAtPlayPosition = true; // Set flag when customer reaches play position
            AddRandomCustomerToQueue();
            // Advance the remaining queue
            yield return AdvanceQueue();

            // Add new customer at the end
            
        }
        yield break; // Added to handle case when conditions aren't met
    }

    private IEnumerator HandleCustomerExit()
    {
        CustomerClass customerToExit = currentPlayingCustomer;
        currentPlayingCustomer = null;
        customerAtPlayPosition = false; // Reset flag when customer starts exiting

        // First wait for current customer to exit completely
        yield return StartCoroutine(MoveCustomerToExit(customerToExit));

        // Only after exit is complete, move the next customer
        yield return StartCoroutine(MoveCustomerToPlayPosition());
    }

    public IEnumerator MoveCustomerToExit(CustomerClass customer)
    {
        Sequence exitSequence = DOTween.Sequence();
        
        // Base movement
        exitSequence.Append(customer.transform.DOMove(exitPosition.position, moveToExitDuration)
            .SetEase(Ease.Linear));
            
        // Calculate total jumps based on duration and frequency
        int totalJumps = Mathf.FloorToInt(moveToExitDuration * jumpsPerSecond);
        float jumpDuration = moveToExitDuration / totalJumps;

        // Add rapid small jumps
        for (int i = 0; i < totalJumps; i++)
        {
            exitSequence.Join(customer.transform.DOMoveY(
                customer.transform.position.y + jumpHeight,
                jumpDuration * 0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad)
                .SetDelay(i * jumpDuration));
        }

        yield return exitSequence.WaitForCompletion();
        Destroy(customer.gameObject);
    }
}