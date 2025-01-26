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
        if (market == null)
        {
            // Market no longer exists, ignore modification
            Debug.Log($"Attempted to modify non-existent market: {marketName}");
            return;
        }

        market.marketValue += amount;
        market.marketValue = Mathf.Clamp(market.marketValue, 0, 100);
        market.UpdateUI();
        CheckMarketStatus(market);
    }

    // Metodo per controllare lo stato di un mercato (es. se arriva a 0 o 100)
    private void CheckMarketStatus(MarketClass market)
    {
        if (market.marketValue <= 0)
        {
            market.marketValue = 0;  // Ensure it's exactly 0
            AudioManager.Instance.PlayMarketCrashSound();
            // Check if it's the target market
            if (PlayerManager.Instance.IsTargetMarket(market.marketData.marketName))
            {
                GameManager.Instance.LoseGame(market.marketData.marketName);
            }
            else
            {
                RemoveMarket(market);
            }
        }
        else if (market.marketValue >= 100)
        {
            market.marketValue = 100;  // Ensure it's exactly 100
            GameManager.Instance.WinGame(market.marketData.marketName);
        }
    }

    public void RemoveMarket(MarketClass market)
    {
        // Don't remove if it's the last market or target market
        if (markets.Count <= 1 || PlayerManager.Instance.IsTargetMarket(market.marketData.marketName))
        {
            GameManager.Instance.LoseGame(market.marketData.marketName);
            return;
        }

        markets.Remove(market);

        // Start coroutine to remove customers with this market
        StartCoroutine(QueueManager.Instance.RemoveCustomersOfFailedMarket(market.marketData.marketName));

        // Fade out and destroy the market UI
        market.FadeOutAndDestroy();
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
        var activeMarkets = markets.FindAll(m => m != null && m.gameObject != null && m.marketValue > 0);
        if (activeMarkets == null || activeMarkets.Count == 0)
            return null;

        MarketClass worstMarket = activeMarkets[0];
        float lowestValue = worstMarket.marketValue;

        foreach (MarketClass market in activeMarkets)
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
        var activeMarkets = markets.FindAll(m => m != null && m.gameObject != null && m.marketValue > 0);
        if (activeMarkets == null || activeMarkets.Count == 0)
            return null;

        MarketClass bestMarket = activeMarkets[0];
        float highestValue = bestMarket.marketValue;

        foreach (MarketClass market in activeMarkets)
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
        var activeMarkets = markets.FindAll(m => m != null && m.gameObject != null && m.marketValue > 0);
        if (activeMarkets == null || activeMarkets.Count == 0)
        {
            Debug.LogError("No active markets available to assign!");
            return null;
        }

        int randomIndex = Random.Range(0, activeMarkets.Count);
        return activeMarkets[randomIndex];
    }
}