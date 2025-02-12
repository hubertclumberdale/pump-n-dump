using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class CardScriptable : ScriptableObject
{

    [Header("Card Description")]
    public string cardName;        // Changed from 'name' to avoid confusion with Unity's Object.name
    public string description;
    [Range(0f, 1f)]
    public float probability = 1f; // Default probability of 100%

    [Header("Card Appearance")]
    public Color color;
    public Sprite cardSprite;
    public AudioClip playSound;  // Add this field for card-specific sound

    public int valueForTarget;
    public int valueForOthers;

    public int numberOfNextAffectedCustomers;
    
    public bool affectsAllOtherMarketsToo;

    public bool affectsWorstMarket;
    public bool affectsBestMarket;
    public bool affectsMostCommonMarket;

    public bool shuffleQueue;
    public bool resetQueue;
    public bool resetHand;

    public bool removeCurrentCustomersFromQueue;

    public bool removesCopFromQueue;
    public bool movesCopToEndOfQueue;

    public bool affectsLongestMarketSequence;
    public int valueMultiplierForSequence = 10; // Default multiplier of 10
}