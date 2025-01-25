using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;  // Add this line for DOTween

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

    public void UpdateUI()
    {
        if (valueText != null)
        {
            valueText.text = $"{marketValue}%";
        }
        
        if (barFillImage != null)
        {
            barFillImage.fillAmount = marketValue / 100f;
            
            // Optional: Change color based on value
            Color currentColor = marketData.marketColor;
            if (marketValue < 30)
            {
                currentColor.a = 0.5f; // Make it more transparent when low
            }
            barFillImage.color = currentColor;
        }

        if (iconImage != null)
        {
            // Optional: Shake or pulse the icon when value changes significantly
            if (Mathf.Abs(marketValue - marketData.initialValue) > 20)
            {
/*                 iconImage.transform.DOShakeScale(0.3f, 0.1f);
 */            }
        }
    }

    public void FadeOutAndDestroy()
    {
        // Fade out all UI elements
        if (barOutlineImage != null)
        {
            barOutlineImage.color = new Color(barOutlineImage.color.r, barOutlineImage.color.g, barOutlineImage.color.b, 1f);
            barOutlineImage.DOFade(0, 0.5f);
        }
        
        if (barFillImage != null)
        {
            barFillImage.color = new Color(barFillImage.color.r, barFillImage.color.g, barFillImage.color.b, 1f);
            barFillImage.DOFade(0, 0.5f);
        }
        
        if (valueText != null)
        {
            valueText.DOFade(0, 0.5f);
        }
        
        if (iconImage != null)
        {
            iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 1f);
            iconImage.DOFade(0, 0.5f);
        }
            
        // Destroy after fade
        Destroy(gameObject, 0.6f);
    }
}