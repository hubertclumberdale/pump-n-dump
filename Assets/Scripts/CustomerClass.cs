using UnityEngine;

public class CustomerClass : MonoBehaviour
{
    public MarketClass market;

    private void Start()
    {
        AssignRandomMarket();
    }

    private void AssignRandomMarket()
    {
        if (MarketManager.Instance != null && MarketManager.Instance.markets.Count > 0)
        {
            // Get a random market from the available markets
            int randomIndex = Random.Range(0, MarketManager.Instance.markets.Count);
            market = MarketManager.Instance.markets[randomIndex];
            
            // Optional: You can add visual feedback here
            // For example, tint the customer sprite with the market color
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && market.marketData != null)
            {
                spriteRenderer.color = market.marketData.marketColor;
            }

        }
        else
        {
            Debug.LogError("No markets available to assign to customer!");
        }
    }
}