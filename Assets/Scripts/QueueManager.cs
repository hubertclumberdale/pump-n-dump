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

    void Start()
    {
        Instance = this;

        StartCoroutine(InitializeQueue());
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
            Vector3 targetPos = currentPos + new Vector3(0.65f, 0, -0.2f); // Single diagonal step

            yield return customer.transform.DOMove(targetPos, animationDuration)
                .SetEase(Ease.InOutQuad).WaitForCompletion();
        }
    }
}