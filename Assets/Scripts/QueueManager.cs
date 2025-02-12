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
    public Transform waitPoint; // Reference point for queue positioning
    public float distanceBetweenCustomers = 0.65f; // Distance between customers in queue
    public float queueAngle = -11.3f; // Angle of the queue line (in degrees)

    public int safePositions = 9;  // Changed from const to public variable

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

    // New method to calculate position based on steps from wait point
    private Vector3 GetPositionFromWaitPoint(int steps)
    {
        float angle = queueAngle * Mathf.Deg2Rad; // Convert to radians
        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distanceBetweenCustomers * steps,
            0,
            Mathf.Sin(angle) * distanceBetweenCustomers * steps
        );
        return waitPoint.position - offset;
    }

    // Inizializza la fila con persone casuali
    IEnumerator InitializeQueue()
    {
        customerQueue = new Queue<CustomerClass>();
        isQueueInitialized = false;
        List<CustomerClass> allCustomers = new List<CustomerClass>();

        // Create all customers at once
        for (int i = 0; i < queueSize; i++)
        {
            GameObject newCustomerObject = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
            CustomerClass newCustomer = newCustomerObject.GetComponent<CustomerClass>();
            
            if (i < safePositions)  // Changed from SAFE_POSITIONS to safePositions
            {
                newCustomer.ForceCivilian();
            }
            
            customerQueue.Enqueue(newCustomer);
            allCustomers.Add(newCustomer);
        }

        // Move all customers simultaneously
        List<Sequence> moveSequences = new List<Sequence>();
        for (int i = 0; i < allCustomers.Count; i++)
        {
            Vector3 targetPos = GetPositionFromWaitPoint(i);
            moveSequences.Add(CreateMoveSequence(allCustomers[i], targetPos));
        }

        // Start all sequences simultaneously
        foreach (var seq in moveSequences)
        {
            seq.Play();
        }

        // Wait for the longest sequence to complete
        yield return new WaitForSeconds(moveToPlayDuration + 0.1f);

        isQueueInitialized = true;
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveCustomerToPlayPosition());
    }

    // New helper method to create move sequence
    private Sequence CreateMoveSequence(CustomerClass customer, Vector3 targetPosition)
    {
        AudioManager.Instance.PlayCustomerMove();  // Add sound for any movement
        Sequence moveSequence = DOTween.Sequence();
        
        // Base movement
        moveSequence.Append(customer.transform.DOMove(targetPosition, moveToPlayDuration)
            .SetEase(Ease.Linear));
        
        int totalJumps = Mathf.FloorToInt(moveToPlayDuration * jumpsPerSecond);
        float jumpDuration = moveToPlayDuration / totalJumps;

        for (int i = 0; i < totalJumps; i++)
        {
            moveSequence.Join(customer.transform.DOMoveY(
                customer.transform.position.y + jumpHeight,
                jumpDuration * 0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad)
                .SetDelay(i * jumpDuration));
        }

        return moveSequence;
    }

    // Aggiunge una persona casuale alla fine della fila
    void AddRandomCustomerToQueue()
    {
        GameObject newCustomerObject = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        CustomerClass newCustomer = newCustomerObject.GetComponent<CustomerClass>();
        customerQueue.Enqueue(newCustomer);
    }

    // Add this helper method to check if customer needs to move
    private bool NeedsRepositioning(CustomerClass customer, Vector3 targetPosition)
    {
        return Vector3.Distance(customer.transform.position, targetPosition) > 0.01f;
    }

    // Avanza la fila di una posizione
    public IEnumerator AdvanceQueue()
    {
        CustomerClass[] customers = customerQueue.ToArray();
        List<Sequence> moveSequences = new List<Sequence>();
        bool anyMovement = false;

        for (int i = 0; i < customers.Length; i++)
        {
            if (customers[i] != null && customers[i].gameObject != null)
            {
                Vector3 targetPos = GetPositionFromWaitPoint(i);
                if (NeedsRepositioning(customers[i], targetPos))
                {
                    moveSequences.Add(CreateMoveSequence(customers[i], targetPos));
                    anyMovement = true;
                }
            }
        }

        if (anyMovement)
        {
            foreach (var seq in moveSequences)
            {
                seq.Play();
            }
            yield return new WaitForSeconds(moveToPlayDuration + 0.1f);
        }
    }

    public IEnumerator MoveCustomerToPlayPosition() // Renamed back from MoveCustomerToCombatPosition
    {
        if (customerQueue.Count > 0 && currentPlayingCustomer == null) // Double check that position is empty
        {
            AudioManager.Instance.PlayCustomerMove();  // Keep this for entering play position
            CustomerClass customer = customerQueue.Dequeue();
            currentPlayingCustomer = customer; // Store reference
            
            // Move the customer (even if they're a cop) to the play position first
            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(customer.transform.DOMove(playPosition.position, moveToPlayDuration)
                .SetEase(Ease.Linear));
            
            int totalJumps = Mathf.FloorToInt(moveToPlayDuration * jumpsPerSecond);
            float jumpDuration = moveToPlayDuration / totalJumps;

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
            customerAtPlayPosition = true;

            // Only check for cop and trigger game over after they've reached the position
            if (customer.isCop)
            {
                yield return new WaitForSeconds(0.5f); // Add a small dramatic pause
                GameManager.Instance.LoseGameCop();
                yield break;
            }

            AddRandomCustomerToQueue();
            yield return AdvanceQueue();
        }
        yield break;
    }

    public IEnumerator HandleCustomerExit()
    {
        if (currentPlayingCustomer != null)
        {
            CustomerClass customerToExit = currentPlayingCustomer;
            currentPlayingCustomer = null;
            customerAtPlayPosition = false;

            yield return StartCoroutine(MoveCustomerToExit(customerToExit));
            PlayerManager.Instance.OnCustomerExited();  // Reset the flag
            PlayerManager.Instance.DrawCard();
            yield return new WaitForSeconds(0.2f);
            
            if (customerQueue.Count > 0)
            {
                yield return StartCoroutine(MoveCustomerToPlayPosition());
            }
        }
    }

    public IEnumerator MoveCustomerToExit(CustomerClass customer)
    {
        AudioManager.Instance.PlayCustomerMove();  // Keep only customer move sound here
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

        List<CustomerClass> customerList = customerQueue.ToList();
        
        // Fisher-Yates shuffle
        for (int i = customerList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CustomerClass temp = customerList[i];
            customerList[i] = customerList[randomIndex];
            customerList[randomIndex] = temp;
        }

        customerQueue.Clear();
        List<Sequence> moveSequences = new List<Sequence>();
        bool anyMovement = false;

        for (int i = 0; i < customerList.Count; i++)
        {
            customerQueue.Enqueue(customerList[i]);
            Vector3 newPosition = GetPositionFromWaitPoint(i);
            if (NeedsRepositioning(customerList[i], newPosition))
            {
                moveSequences.Add(CreateMoveSequence(customerList[i], newPosition));
                anyMovement = true;
            }
        }

        if (anyMovement)
        {
            foreach (var seq in moveSequences)
            {
                seq.Play();
            }
            yield return new WaitForSeconds(moveToPlayDuration + 0.1f);
        }
        
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

    private IEnumerator RemoveCustomersFromQueue(System.Predicate<CustomerClass> shouldRemove)
    {
        // Step 1: Find all matching customers
        List<CustomerClass> customerList = customerQueue.ToList();
        List<CustomerClass> customersToRemove = new List<CustomerClass>();
        List<CustomerClass> remainingCustomers = new List<CustomerClass>();

        // Separate matching customers from others
        foreach (CustomerClass customer in customerList)
        {
            if (shouldRemove(customer))
            {
                customersToRemove.Add(customer);
            }
            else
            {
                remainingCustomers.Add(customer);
            }
        }

        // If no matching customers found, just handle customer exit normally
        if (customersToRemove.Count == 0)
        {
            yield return StartCoroutine(HandleCustomerExit());
            yield break;
        }

        // Continue with customer removal sequence
        customerQueue.Clear();

        // Move all matching customers to exit simultaneously
        foreach (CustomerClass customer in customersToRemove)
        {
            StartCoroutine(MoveCustomerToExit(customer));
        }

        yield return new WaitForSeconds(moveToExitDuration + 0.2f);

        // Realign remaining customers
        List<Sequence> realignSequences = new List<Sequence>();
        bool anyMovement = false;
        for (int i = 0; i < remainingCustomers.Count; i++)  // Fix: Changed condition from remainingCustomers.Count to i < remainingCustomers.Count
        {
            CustomerClass customer = remainingCustomers[i];
            customerQueue.Enqueue(customer);
            Vector3 newPosition = GetPositionFromWaitPoint(i);
            
            if (NeedsRepositioning(customer, newPosition))
            {
                realignSequences.Add(CreateMoveSequence(customer, newPosition));
                anyMovement = true;
            }
        }

        if (anyMovement)
        {
            foreach (var seq in realignSequences)
            {
                seq.Play();
            }
            yield return new WaitForSeconds(moveToPlayDuration + 0.1f);
        }

        yield return StartCoroutine(AdvanceQueue());
        yield return new WaitForSeconds(0.2f);

        // Spawn new customers to fill the queue
        int customersToAdd = queueSize - remainingCustomers.Count;
        for (int i = 0; i < customersToAdd; i++)
        {
            GameObject newCustomerObject = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
            CustomerClass newCustomer = newCustomerObject.GetComponent<CustomerClass>();
            customerQueue.Enqueue(newCustomer);
            
            yield return StartCoroutine(AdvanceQueue());
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(HandleCustomerExit());
    }

    public IEnumerator RemoveAllCopsFromQueue()
    {
        yield return StartCoroutine(RemoveCustomersFromQueue(customer => customer.isCop));
    }

    public IEnumerator RemoveCustomersOfMarketFromQueue()
    {
        if (currentPlayingCustomer == null || currentPlayingCustomer.market == null)
        {
            yield return StartCoroutine(HandleCustomerExit());
            yield break;
        }

        string targetMarketName = currentPlayingCustomer.market.marketData.marketName;
        yield return StartCoroutine(RemoveCustomersFromQueue(customer => 
            customer.market != null && customer.market.marketData.marketName == targetMarketName));
    }

    public IEnumerator RemoveCustomersOfFailedMarket(string failedMarketName)
    {
        yield return StartCoroutine(RemoveCustomersFromQueue(customer => 
            customer.market != null && customer.market.marketData.marketName == failedMarketName));
    }

    public string GetMostCommonMarketInQueue()
    {
        Dictionary<string, int> marketCounts = new Dictionary<string, int>();
        
        // Count customers for each market
        foreach (CustomerClass customer in customerQueue)
        {
            if (customer.market != null && !customer.isCop)
            {
                string marketName = customer.market.marketData.marketName;
                if (!marketCounts.ContainsKey(marketName))
                {
                    marketCounts[marketName] = 0;
                }
                marketCounts[marketName]++;
            }
        }

        // Find market with highest count
        string mostCommonMarket = "";
        int maxCount = -1;

        foreach (var kvp in marketCounts)
        {
            if (kvp.Value > maxCount)
            {
                maxCount = kvp.Value;
                mostCommonMarket = kvp.Key;
            }
        }

        return mostCommonMarket;
    }

    public (string marketName, int sequenceLength) GetLongestMarketSequence()
    {
        if (customerQueue.Count == 0) return ("", 0);

        CustomerClass[] customers = customerQueue.ToArray();
        string currentMarket = "";
        int currentCount = 0;
        string longestMarket = "";
        int maxCount = 0;

        foreach (CustomerClass customer in customers)
        {
            if (customer.isCop || customer.market == null) 
            {
                // Reset sequence on cops or null markets
                currentCount = 0;
                currentMarket = "";
                continue;
            }

            string marketName = customer.market.marketData.marketName;

            if (marketName == currentMarket)
            {
                currentCount++;
                if (currentCount > maxCount)
                {
                    maxCount = currentCount;
                    longestMarket = currentMarket;
                }
            }
            else
            {
                currentMarket = marketName;
                currentCount = 1;
                if (currentCount > maxCount)
                {
                    maxCount = currentCount;
                    longestMarket = currentMarket;
                }
            }
        }

        return (longestMarket, maxCount);
    }
}