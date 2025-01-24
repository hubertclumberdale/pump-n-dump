using System.Collections;
using System.Collections.Generic;
using System.Linq;  // Add this line to use ToList()
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
    public CustomerClass currentPlayingCustomer; // Track customer at play position
    private bool isQueueInitialized = false;
    private bool customerAtPlayPosition = false; // Add this flag

    public float moveToPlayDuration = 0.8f; // Adjusted duration for moving to play position
    public float moveToExitDuration = 0.8f; // Adjusted duration for moving to exit
    public int jumpCount = 3; // Number of jumps during movement
    public float jumpHeight = 0.1f; // Reduced height for smaller jumps
    public int jumpsPerSecond = 4; // New variable to control jump frequency

    public Transform handPosition;

    private const int SAFE_POSITIONS = 3;  // Number of initial positions where cops can't spawn

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        StartCoroutine(InitializeQueue());
    }

    public void Reset()
    {
        StopAllCoroutines();
        
        if (currentPlayingCustomer != null)
        {
            Destroy(currentPlayingCustomer.gameObject);
            currentPlayingCustomer = null;
        }

        if (customerQueue != null)
        {
            foreach (var customer in customerQueue)
            {
                if (customer != null && customer.gameObject != null)
                {
                    Destroy(customer.gameObject);
                }
            }
            customerQueue.Clear();
        }

        isQueueInitialized = false;
        customerAtPlayPosition = false;
    }

    // Remove the A key check from Update since we don't need manual movement anymore
    void Update()
    {
        if (!isQueueInitialized) return;
    }

    // Inizializza la fila con persone casuali
    IEnumerator InitializeQueue()
    {
        customerQueue = new Queue<CustomerClass>();
        isQueueInitialized = false;

        // Create and position all customers at once
        for (int i = 0; i < queueSize; i++)
        {
            GameObject newCustomerObject = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
            CustomerClass newCustomer = newCustomerObject.GetComponent<CustomerClass>();
            
            // Force non-cop for first SAFE_POSITIONS
            if (i < SAFE_POSITIONS)
            {
                newCustomer.ForceCivilian();
            }
            
            customerQueue.Enqueue(newCustomer);

            Vector3 targetPos = GetQueuePosition(queueSize - 1 - i);  // Position from back to front
            
            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(newCustomer.transform.DOMove(targetPos, moveToPlayDuration)
                .SetEase(Ease.Linear));

            // Add jumping effect
            int totalJumps = Mathf.FloorToInt(moveToPlayDuration * jumpsPerSecond);
            float jumpDuration = moveToPlayDuration / totalJumps;

            for (int j = 0; j < totalJumps; j++)
            {
                moveSequence.Join(newCustomer.transform.DOMoveY(
                    newCustomer.transform.position.y + jumpHeight,
                    jumpDuration * 0.5f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.OutQuad)
                    .SetDelay(j * jumpDuration));
            }

            yield return new WaitForSeconds(0.2f); // Small delay between each customer's start
        }

        yield return new WaitForSeconds(moveToPlayDuration); // Wait for all movements to complete
        isQueueInitialized = true;
        
        // Automatically move first customer after queue initialization
        yield return new WaitForSeconds(0.5f); // Small delay for better visual flow
        yield return StartCoroutine(MoveCustomerToPlayPosition());
    }

    private Vector3 GetQueuePosition(int positionFromFront)
    {
        return spawnPoint.position + new Vector3(0.65f * (positionFromFront + 1), 0, -0.2f * (positionFromFront + 1));
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
            
            // Check if cop immediately when they reach play position
            if (customer.isCop)
            {
                GameManager.Instance.LoseGameCop();
                yield break;
            }

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

    public IEnumerator HandleCustomerExit()
    {
        if (currentPlayingCustomer != null)
        {
            CustomerClass customerToExit = currentPlayingCustomer;
            currentPlayingCustomer = null;
            customerAtPlayPosition = false;

            // Wait for current customer to exit
            yield return StartCoroutine(MoveCustomerToExit(customerToExit));
            
            // Small delay before next customer moves
            yield return new WaitForSeconds(0.2f);
            
            // Move next customer to play position
            if (customerQueue.Count > 0)
            {
                yield return StartCoroutine(MoveCustomerToPlayPosition());
            }
        }
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

    public IEnumerator ShuffleQueue()
    {
        if (customerQueue.Count <= 1) yield break;

        // Convert queue to list for shuffling
        List<CustomerClass> customerList = customerQueue.ToList();
        
        // Fisher-Yates shuffle
        for (int i = customerList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CustomerClass temp = customerList[i];
            customerList[i] = customerList[randomIndex];
            customerList[randomIndex] = temp;
        }

        // Clear and refill queue
        customerQueue.Clear();
        foreach (CustomerClass customer in customerList)
        {
            customerQueue.Enqueue(customer);
        }

        // Reposition all customers
        CustomerClass[] customers = customerQueue.ToArray();
        for (int i = 0; i < customers.Length; i++)
        {
            Vector3 newPosition = GetQueuePosition(customers.Length - 1 - i);
            
            Sequence moveSequence = DOTween.Sequence();
            
            // Jump up
            moveSequence.Append(customers[i].transform.DOMoveY(
                customers[i].transform.position.y + 0.5f, 0.3f)
                .SetEase(Ease.OutQuad));
            
            // Move to new position
            moveSequence.Append(customers[i].transform.DOMove(newPosition, 0.5f)
                .SetEase(Ease.InOutQuad));
            
            // Land down
            moveSequence.Append(customers[i].transform.DOMoveY(
                newPosition.y, 0.3f)
                .SetEase(Ease.InQuad));
            
            yield return new WaitForSeconds(0.1f);
        }

        // Wait a bit after shuffle completes
        yield return new WaitForSeconds(0.5f);
        
        // Now handle the customer exit and next customer
        yield return StartCoroutine(HandleCustomerExit());
    }

    public IEnumerator ResetQueue()
    {
        // Move all current customers to exit
        CustomerClass[] currentCustomers = customerQueue.ToArray();
        customerQueue.Clear();
        
        // Move all customers to exit simultaneously
        foreach (CustomerClass customer in currentCustomers)
        {
            StartCoroutine(MoveCustomerToExit(customer));
        }

        // Wait for customers to reach exit point
        yield return new WaitForSeconds(moveToExitDuration + 0.2f);

        // Reinitialize the queue
        yield return StartCoroutine(InitializeQueue());

        // Now handle the customer exit and next customer
        yield return StartCoroutine(HandleCustomerExit());
    }
}