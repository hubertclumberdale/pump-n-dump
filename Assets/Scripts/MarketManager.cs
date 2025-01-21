using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MarketManager : MonoBehaviour
{
    [System.Serializable]
    public class Market
    {
        public MarketScriptable marketData; // Riferimento allo Scriptable Object
        public int marketValue; // Valore attuale del mercato
        public Text marketUIText; // UI associata per visualizzare i dati
    }

    public List<Market> markets; // Lista di tutti i mercati nel gioco

    private void Start()
    {
        InitializeMarkets();
        UpdateAllMarketsUI();
    }

    // Inizializza i mercati con i valori dai MarketScriptable
    private void InitializeMarkets()
    {
        foreach (var market in markets)
        {
            market.marketValue = market.marketData.initialValue;
        }
    }

    // Metodo per modificare il valore di un mercato
    public void ModifyMarketValue(string marketName, int amount)
    {
        Market market = markets.Find(m => m.marketData.marketName == marketName);
        if (market != null)
        {
            market.marketValue += amount;
            market.marketValue = Mathf.Clamp(market.marketValue, 0, 100); // Limita il valore tra 0 e 100
            UpdateMarketUI(market);
            CheckMarketStatus(market);
        }
        else
        {
            Debug.LogWarning($"Market {marketName} not found!");
        }
    }

    // Metodo per aggiornare l'interfaccia utente del mercato
    private void UpdateMarketUI(Market market)
    {
        
    }

    // Metodo per controllare lo stato di un mercato (es. se arriva a 0 o 100)
    private void CheckMarketStatus(Market market)
    {
        if (market.marketValue == 0)
        {
            Debug.Log($"{market.marketData.marketName} has reached 0 and is out of the game.");
            RemoveMarket(market);
        }
        else if (market.marketValue == 100)
        {
            Debug.Log($"{market.marketData.marketName} has reached 100!");
            // Aggiungere qui la logica per gestire la vittoria o la perdita
        }
    }

    // Metodo per rimuovere un mercato dalla lista attiva (quando raggiunge 0)
    public void RemoveMarket(Market market)
    {
        markets.Remove(market);
        if (market.marketUIText != null)
        {
            market.marketUIText.text = $"{market.marketData.marketName}: Removed";
        }
        // Logica per gestire la rimozione del mercato (ad es. rimuovere le persone dalla fila)
    }

    // Metodo per aggiornare tutti i mercati nella UI (ad esempio all'inizio del gioco)
    public void UpdateAllMarketsUI()
    {
        
    }
}