using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public Queue<CustomerClass> customerQueue; // La fila di persone
    public int queueSize = 10; // Dimensione della fila
    public List<CustomerClass> customerTemplates; // Template delle persone associate alle aziende

    void Start()
    {
        InitializeQueue();
    }

    // Inizializza la fila con persone casuali
    void InitializeQueue()
    {
        customerQueue = new Queue<CustomerClass>();

        for (int i = 0; i < queueSize; i++)
        {
            AddRandomCustomerToQueue();
        }
    }

    // Aggiunge una persona casuale alla fine della fila
    void AddRandomCustomerToQueue()
    {
        int randomIndex = Random.Range(0, customerTemplates.Count);
        CustomerClass newCustomer = Instantiate(customerTemplates[randomIndex]);
        customerQueue.Enqueue(newCustomer);
    }

    // Rimuove la persona in testa alla fila e aggiunge una nuova persona alla fine
    public void AdvanceQueue()
    {
        if (customerQueue.Count > 0)
        {
            CustomerClass frontCustomer = customerQueue.Dequeue();
            Destroy(frontCustomer.gameObject);
            AddRandomCustomerToQueue();
        }
    }

    // Restituisce la persona in testa alla fila
    public CustomerClass GetFrontCustomer()
    {
        if (customerQueue.Count > 0)
        {
            return customerQueue.Peek();
        }
        return null;
    }

    // Restituisce la lista delle prossime persone nella fila
    public List<CustomerClass> GetNextCustomers(int count)
    {
        List<CustomerClass> nextCustomers = new List<CustomerClass>(customerQueue);
        return nextCustomers.GetRange(1, Mathf.Min(count, nextCustomers.Count - 1));
    }

    // Rimuove tutte le persone associate a un'azienda specifica
    /* public void RemoveCustomersByCompany(CompanyType company)
    {
        Queue<CustomerClass> newQueue = new Queue<CustomerClass>();

        foreach (CustomerClass customer in customerQueue)
        {
            if (customer.market.company != company)
            {
                newQueue.Enqueue(customer);
            }
            else
            {
                Destroy(customer.gameObject);
            }
        }

        customerQueue = newQueue;
    } */
}