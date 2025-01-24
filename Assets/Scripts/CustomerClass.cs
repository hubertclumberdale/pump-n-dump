using UnityEngine;

public class CustomerClass : MonoBehaviour
{
    public MarketClass market;
    [Range(0f, 1f)]
    public static float copChance = 0.1f;  // 5% chance to be a cop, shared by all customers
    public bool isCop = false;
    private Color copColor = Color.blue;  // Or whatever color you want for cops

    [Header("Body Part Renderers")]
    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer legR;
    public SpriteRenderer legL;
    public SpriteRenderer armR;
    public SpriteRenderer armL;

    [Header("Body Part Sprite Collections")]
    public Sprite[] headSprites;
    public Sprite[] bodySprites;
    public Sprite[] legSprites;
    public Sprite[] armSprites;

    private void Start()
    {
        // Determine if this customer is a cop
        isCop = Random.value < copChance;

        if (!isCop)
        {
            RandomizeBodyParts();
            AssignRandomMarket();
        }
        else
        {
            market = null; // Cops don't have markets
            RandomizeBodyParts();
            Debug.Log("Cop spawned!");
            ApplyColorToBodyParts(copColor);
        }
    }

    private void RandomizeBodyParts()
    {
        if (headSprites.Length > 0)
            head.sprite = headSprites[Random.Range(0, headSprites.Length)];

        if (bodySprites.Length > 0)
            body.sprite = bodySprites[Random.Range(0, bodySprites.Length)];

        if (legSprites.Length > 0)
        {
            Sprite selectedLeg = legSprites[Random.Range(0, legSprites.Length)];
            legR.sprite = selectedLeg;
            legL.sprite = selectedLeg;
        }

        if (armSprites.Length > 0)
        {
            Sprite selectedArm = armSprites[Random.Range(0, armSprites.Length)];
            armR.sprite = selectedArm;
            armL.sprite = selectedArm;
        }
    }

    private void AssignRandomMarket()
    {
        if (MarketManager.Instance != null && MarketManager.Instance.markets.Count > 0)
        {
            int randomIndex = Random.Range(0, MarketManager.Instance.markets.Count);
            market = MarketManager.Instance.markets[randomIndex];
            
            if (market.marketData != null)
            {
                Color marketColor = market.marketData.marketColor;
                ApplyColorToBodyParts(marketColor);
            }
        }
        else
        {
            Debug.LogError("No markets available to assign to customer!");
        }
    }

    private void ApplyColorToBodyParts(Color color)
    {
        body.color = color;
        legR.color = color;
        legL.color = color;
        armR.color = color;
        armL.color = color;
    }
}