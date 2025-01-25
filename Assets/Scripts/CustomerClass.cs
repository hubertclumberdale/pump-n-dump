using UnityEngine;

public class CustomerClass : MonoBehaviour
{
    public MarketClass market;
    [Range(0f, 1f)]
    public static float copChance = 0.08f;  // 5% chance to be a cop, shared by all customers
    public bool isCop = false;
    public Color copColor = Color.blue;  // Made public to be configurable in inspector
    private bool forceNoCop = false;

    [Header("Body Part Renderers")]
    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer legR;
    public SpriteRenderer legL;
    public SpriteRenderer armR;
    public SpriteRenderer armL;

    public SpriteRenderer marketSymbol;

    [Header("Body Part Sprite Collections")]
    public Sprite[] headSprites;
    public Sprite[] bodySprites;
    public Sprite[] legSprites;
    public Sprite[] armSprites;

    [Header("Cop Body Part Sprite Collections")]
    public Sprite[] copHeadSprites;
    public Sprite[] copBodySprites;
    public Sprite[] copLegSprites;
    public Sprite[] copArmSprites;

    public void ForceCivilian()
    {
        forceNoCop = true;
    }

    private void Start()
    {
        // Determine if this customer is a cop
        isCop = !forceNoCop && (Random.value < copChance);

        if (!isCop)
        {
            RandomizeBodyParts();
            AssignRandomMarket();
            marketSymbol.gameObject.SetActive(true);
        }
        else
        {
            market = null; // Cops don't have markets
            marketSymbol.gameObject.SetActive(false);
            RandomizeBodyParts();
            ApplyColorToBodyParts(copColor);
        }
    }

    private void RandomizeBodyParts()
    {
        Sprite[] currentHeadSprites = isCop ? copHeadSprites : headSprites;
        Sprite[] currentBodySprites = isCop ? copBodySprites : bodySprites;
        Sprite[] currentLegSprites = isCop ? copLegSprites : legSprites;
        Sprite[] currentArmSprites = isCop ? copArmSprites : armSprites;

        if (currentHeadSprites.Length > 0)
            head.sprite = currentHeadSprites[Random.Range(0, currentHeadSprites.Length)];

        if (currentBodySprites.Length > 0)
            body.sprite = currentBodySprites[Random.Range(0, currentBodySprites.Length)];

        if (currentLegSprites.Length > 0)
        {
            Sprite selectedLeg = currentLegSprites[Random.Range(0, currentLegSprites.Length)];
            legR.sprite = selectedLeg;
            legL.sprite = selectedLeg;
        }

        if (currentArmSprites.Length > 0)
        {
            Sprite selectedArm = currentArmSprites[Random.Range(0, currentArmSprites.Length)];
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
                marketSymbol.sprite = market.marketData.marketIcon;
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