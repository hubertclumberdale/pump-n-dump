using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class CardScriptable : ScriptableObject
{

    [Header("Card Description")]
    public string cardName;
    public string probability;

    public Color color;
    public Sprite cardSprite;

    public int valueForTarget;
    public int valueForOthers;

    public int numberOfNextAffectedCustomers;
    
    public bool affectsAllOtherMarketsToo;

    public bool affectsWorstMarket;
    public bool affectsBestMarket;


    public bool shuffleQueue;
    public bool shuffleHand;

    public bool removesCopFromQueue;
    public bool movesCopToEndOfQueue;
}