using UnityEngine;

public class CustomerClass : MonoBehaviour
{
    public MarketScriptable market;


    public void Initialize(MarketScriptable marketData)
    {
        market = marketData;
    }
}