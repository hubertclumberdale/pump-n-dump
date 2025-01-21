using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
    [System.Serializable]
    public class Company
    {
        public CompanyType companyType;
        public int value;
        public GameObject companyUI; // Riferimento alla UI dell'azienda
    }

    public List<Company> companies;
    public int minValue = 0;
    public int maxValue = 100;

    void Start()
    {
        InitializeCompanies();
        UpdateCompanyUI();
    }

    // Inizializza le aziende con valori iniziali
    private void InitializeCompanies()
    {
        foreach (var company in companies)
        {
            company.value = Random.Range(25, 75); // Inizializza con un valore casuale
        }
    }

    // Aggiorna il valore di una specifica azienda
    public void UpdateCompanyValue(CompanyType companyType, int valueChange)
    {
        Company targetCompany = companies.Find(c => c.companyType == companyType);
        if (targetCompany != null)
        {
            targetCompany.value = Mathf.Clamp(targetCompany.value + valueChange, minValue, maxValue);
            UpdateCompanyUI();

            // Controlla le condizioni di vittoria/sconfitta
            CheckGameState(targetCompany);
        }
    }

    // Aggiorna l'UI delle aziende
    private void UpdateCompanyUI()
    {
        foreach (var company in companies)
        {
            // Supponiamo che la UI mostri il valore dell'azienda tramite un componente di testo
            if (company.companyUI != null)
            {
                var uiText = company.companyUI.GetComponent<UnityEngine.UI.Text>();
                if (uiText != null)
                {
                    uiText.text = $"{company.companyType}: {company.value}";
                }
            }
        }
    }

    // Controlla lo stato del gioco basato sui valori delle aziende
    private void CheckGameState(Company targetCompany)
    {
        if (targetCompany.value <= minValue)
        {
            Debug.Log($"Company {targetCompany.companyType} has reached {minValue}. Removing from the game.");
            RemoveCompany(targetCompany.companyType);
        }
        else if (targetCompany.value >= maxValue)
        {
            Debug.Log($"Company {targetCompany.companyType} has reached {maxValue}. Checking for win/loss conditions.");
            // Verifica vittoria o sconfitta
            GameManager.Instance.CheckGameState(); // Assumendo che il GameManager abbia un singleton
        }
    }

    // Rimuove un'azienda che ha raggiunto il valore minimo
    private void RemoveCompany(CompanyType companyType)
    {
        companies.RemoveAll(c => c.companyType == companyType);
        UpdateCompanyUI();

        // Notifica al GameManager o ad altre classi che l'azienda è stata rimossa
        GameManager.Instance.OnCompanyRemoved(companyType); // Funzione che gestisce la rimozione nel GameManager
    }

    // Ritorna la lista di aziende attive
    public List<CompanyType> GetActiveCompanies()
    {
        return companies.ConvertAll(c => c.companyType);
    }
}