using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance { get; private set; }  // Add singleton Instance
    public List<MarketScriptable> marketScriptables; // List of market scriptables
    public GameObject marketPrefab; // Prefab to instantiate
    public Transform marketsParent; // Parent transform for instantiated markets
    public List<MarketClass> markets; // Lista di tutti i mercati nel gioco

    private void Awake()
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

    private void Start()
    {
        markets = new List<MarketClass>();
        InitializeMarkets();
        UpdateAllMarketsUI();
    }

    // Inizializza i mercati con i valori dai MarketScriptable
    private void InitializeMarkets()
    {
        foreach (var scriptable in marketScriptables)
        {   
            // Instantiate the market prefab
            GameObject marketObject = Instantiate(marketPrefab, marketsParent);
            MarketClass marketClass = marketObject.GetComponent<MarketClass>();
            
            if (marketClass != null)
            {
                marketClass.Initialize(scriptable);
                markets.Add(marketClass);
            }
            else
            {
                Debug.LogError("Market prefab is missing MarketClass component!");
            }
        }
    }

    // Metodo per modificare il valore di un mercato
    public void ModifyMarketValue(string marketName, int amount)
    {
        MarketClass market = markets.Find(m => m.marketData.marketName == marketName);
        if (market != null)
        {
            market.marketValue += amount;
            market.marketValue = Mathf.Clamp(market.marketValue, 0, 100);
            market.UpdateUI();
            CheckMarketStatus(market);
        }
        else
        {
            Debug.LogWarning($"Market {marketName} not found!");
        }
    }

    // Metodo per controllare lo stato di un mercato (es. se arriva a 0 o 100)
    private void CheckMarketStatus(MarketClass market)
    {
        if (market.marketValue == 0)
        {
            Debug.Log($"{market.marketData.marketName} has reached 0 and is out of the game.");
        }
        else if (market.marketValue == 100)
        {
            Debug.Log($"{market.marketData.marketName} has reached 100!");
            // Aggiungere qui la logica per gestire la vittoria o la perdita
        }
    }

    // Metodo per aggiornare tutti i mercati nella UI (ad esempio all'inizio del gioco)
    public void UpdateAllMarketsUI()
    {
        foreach (MarketClass market in markets)
        {
            market.UpdateUI();
        }
    }
}