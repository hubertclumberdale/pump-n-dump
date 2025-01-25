using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;  // Add this line for IEnumerator
using TMPro;  
public class CardClass : MonoBehaviour
{
    public CardScriptable cardData;
    public Button cardButton;
    private float playAnimationDuration = 0.5f;
    private float shakeAnimationDuration = 0.2f;  // Reduced from 0.3f to 0.2f
    private float fadeAnimationDuration = 0.3f;
    private int handIndex = -1;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImg;

    public Image cardBackground;

    public void SetHandIndex(int index)
    {
        handIndex = index;
        if (cardButton == null)
        {
            cardButton = GetComponent<Button>();
        }
        
        cardButton.onClick.AddListener(() => {
            if (handIndex != -1)
            {
                PlayerManager.Instance.PlayCard(handIndex);
            }
        });
    }

    public int GetHandIndex()
    {
        return handIndex;
    }

    public void Initialize(CardScriptable data)
    {
        cardData = data;

        // Set card background color
        if (cardBackground != null)
        {
            cardBackground.color = cardData.color;
        }

        // Set card title
        if (titleText != null)
        {
            titleText.text = data.cardName;  // Make sure to use the correct property name from CardScriptable
        }
        else
        {
            Debug.LogError("Title Text component is missing!");
        }

        // Set card description
        if (descriptionText != null)
        {
            descriptionText.text = data.description;
        }
        else
        {
            Debug.LogError("Description Text component is missing!");
        }

        // Set card icon
        if (iconImg != null)
        {
            if (cardData.cardSprite != null)
            {
                iconImg.sprite = cardData.cardSprite;
                iconImg.enabled = true;      // Make sure it's visible
            }
            else
            {
                Debug.LogWarning("Card sprite is missing in CardScriptable!");
                iconImg.enabled = false;     // Hide if no sprite
            }
        }
        else
        {
            Debug.LogError("Icon Image component is missing!");
        }
    }

    public void Play(Transform playedPosition)
    {
        cardButton.interactable = false;

        Sequence playSequence = DOTween.Sequence();

        // First move to position
        playSequence.Append(transform.DOMove(playedPosition.position, playAnimationDuration)
            .SetEase(Ease.OutQuad));
        
        // Then shake, and wait until it's complete
        playSequence.Append(transform.DOShakePosition(shakeAnimationDuration, 0.1f, 20, 45, false));
        
        // Add a small pause between shake and fade
        playSequence.AppendInterval(0.1f);
        
        // Finally fade out
        playSequence.Append(transform.DOScale(Vector3.zero, fadeAnimationDuration)
            .SetEase(Ease.InBack));

        playSequence.OnComplete(() => {
            ApplyCardEffects();
            QueueManager.Instance.StartCoroutine(DestroyAfterEffects());
        });
    }

    public void FadeOut()
    {
        cardButton.interactable = false;
        Sequence fadeSequence = DOTween.Sequence();
        
        // Scale down to zero
        fadeSequence.Append(transform.DOScale(Vector3.zero, fadeAnimationDuration)
            .SetEase(Ease.InBack));
            
        fadeSequence.OnComplete(() => {
            Destroy(gameObject);
        });
    }

    private IEnumerator DestroyAfterEffects()
    {
        // Wait a small amount of time to ensure effects are processed
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject, 2f);
    }

    private void ApplyCardEffects()
    {
        CustomerClass currentCustomer = QueueManager.Instance.currentPlayingCustomer;
        if (currentCustomer == null) return;

        string targetMarket = currentCustomer.market.marketData.marketName;

        if (cardData.valueForTarget != 0 && !cardData.affectsBestMarket && !cardData.affectsWorstMarket)
        {
            MarketManager.Instance.ModifyMarketValue(targetMarket, cardData.valueForTarget);
        }

        if (cardData.affectsAllOtherMarketsToo)
        {
            foreach (MarketClass market in MarketManager.Instance.markets)
            {
                if (market.marketData.marketName != targetMarket)
                {
                    MarketManager.Instance.ModifyMarketValue(market.marketData.marketName, cardData.valueForOthers);
                }
            }
        }

        if (cardData.shuffleQueue)
        {
            QueueManager.Instance.StartCoroutine(QueueManager.Instance.ShuffleQueue());
        }

        if (cardData.resetQueue)
        {
            QueueManager.Instance.StartCoroutine(QueueManager.Instance.ResetQueue());
        }

        if (cardData.resetHand)
        {
            PlayerManager.Instance.StartCoroutine(PlayerManager.Instance.ResetHand());
        }

        if (cardData.removesCopFromQueue)
        {
            QueueManager.Instance.StartCoroutine(QueueManager.Instance.RemoveAllCopsFromQueue());
        }

        if (cardData.movesCopToEndOfQueue)
        {
            // QueueManager.Instance.MoveCopToEndOfQueue();
        }

        if(cardData.affectsWorstMarket) 
        {
            string worstMarketName = MarketManager.Instance.GetWorstMarket().marketData.marketName;
            MarketManager.Instance.ModifyMarketValue(worstMarketName, cardData.valueForTarget);
        }

        if(cardData.affectsBestMarket) 
        {
            string bestMarketName = MarketManager.Instance.GetBestMarket().marketData.marketName;
            MarketManager.Instance.ModifyMarketValue(bestMarketName, cardData.valueForTarget);
        }

        if(!cardData.shuffleQueue && !cardData.resetQueue && !cardData.removesCopFromQueue && !cardData.resetHand){
            QueueManager.Instance.StartCoroutine(QueueManager.Instance.HandleCustomerExit());
        }
    }

    private void OnDestroy()
    {
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
        }
    }
}