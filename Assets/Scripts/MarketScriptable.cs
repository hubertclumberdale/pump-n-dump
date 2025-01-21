using UnityEngine;

[CreateAssetMenu(fileName = "NewMarket", menuName = "Scriptables/Market")]
public class MarketScriptable : ScriptableObject
{
    public string marketName;
    public int initialValue;
}