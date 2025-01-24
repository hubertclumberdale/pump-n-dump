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
    }

    public void Initialize()
    {
        markets = new List<MarketClass>();
        InitializeMarkets();
        UpdateAllMarketsUI();
    }

    public void Reset()
    {
        foreach (var market in markets)
        {
            if (market != null && market.gameObject != null)
            {
                Destroy(market.gameObject);
            }
        }
        markets.Clear();
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
        if (market.marketValue <= 0)
        {
            market.marketValue = 0;  // Ensure it's exactly 0
            GameManager.Instance.LoseGame(market.marketData.marketName);
        }
        else if (market.marketValue >= 100)
        {
            market.marketValue = 100;  // Ensure it's exactly 100
            GameManager.Instance.WinGame(market.marketData.marketName);
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

    public MarketClass GetWorstMarket()
    {
        if (markets == null || markets.Count == 0)
            return null;

        MarketClass worstMarket = markets[0];
        float lowestValue = worstMarket.marketValue;

        foreach (MarketClass market in markets)
        {
            if (market.marketValue < lowestValue)
            {
                lowestValue = market.marketValue;
                worstMarket = market;
            }
        }

        return worstMarket;
    }

    public MarketClass GetBestMarket()
    {
        if (markets == null || markets.Count == 0)
            return null;

        MarketClass bestMarket = markets[0];
        float highestValue = bestMarket.marketValue;

        foreach (MarketClass market in markets)
        {
            if (market.marketValue > highestValue)
            {
                highestValue = market.marketValue;
                bestMarket = market;
            }
        }

        return bestMarket;
    }

    public MarketClass AssignRandomMarket()
    {
        if (markets == null || markets.Count == 0)
        {
            Debug.LogError("No markets available to assign!");
            return null;
        }

        int randomIndex = Random.Range(0, markets.Count);
        return markets[randomIndex];
    }
}