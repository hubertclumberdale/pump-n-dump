using UnityEngine;

[CreateAssetMenu(fileName = "NewMarket", menuName = "Scriptables/Market")]
public class MarketScriptable : ScriptableObject
{
    public string marketName;            // Add market name field
    public int initialValue;
    public Color marketColor = Color.white;  // Default white color
    public Sprite marketIcon;                // Icon/sprite for the market
}