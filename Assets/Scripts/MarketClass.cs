using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Add TextMeshPro namespace

public class MarketClass : MonoBehaviour
{
    public MarketScriptable marketData; // Riferimento allo Scriptable Object
    public float marketValue; // Valore attuale del mercato
    
    [Header("UI Elements")]
    public Image barOutlineImage;    // Reference to the bar outline
    public Image barFillImage;       // Reference to the bar fill
    public TextMeshProUGUI valueText;  // Add reference to TextMeshPro
    public Image iconImage;     // Add reference to icon image component

    public void Initialize(MarketScriptable scriptable)
    {
        marketData = scriptable;
        marketValue = scriptable.initialValue;
        
        // Set the colors of both bar elements
        if (barOutlineImage != null)
        {
            barOutlineImage.color = scriptable.marketColor;
        }
        
        if (barFillImage != null)
        {
            barFillImage.color = scriptable.marketColor;
        }

        // Set the text color and initial value
        if (valueText != null)
        {
            valueText.color = scriptable.marketColor;
            valueText.text = marketValue.ToString();
        }

        // Set the icon
        if (iconImage != null && scriptable.marketIcon != null)
        {
            iconImage.sprite = scriptable.marketIcon;
        }
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (valueText != null)
        {
            valueText.text = $"{marketValue}%";
        }
        
        if (barFillImage != null)
        {
            // Update fill amount if it's using Image.fillAmount
            barFillImage.fillAmount = marketValue / 100f;
        }
    }
}