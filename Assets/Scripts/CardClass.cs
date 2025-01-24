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
    public TextMeshProUGUI descrText;
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

        cardBackground.color = cardData.color;
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
       

        if (cardData.valueForTarget != 0)
        {

            // Apply value to the current customer's market
            MarketManager.Instance.ModifyMarketValue(targetMarket, cardData.valueForTarget);
        }

        if (cardData.affectsAllOtherMarketsToo)
        {
            // MarketManager.Instance.ApplyValueToAllMarkets(cardData.valueForOthers);
        }

        // Apply shuffle, remove, or move effects based on the flags in CardScriptable
        if (cardData.shuffleQueue)
        {
            // QueueManager.Instance.ShuffleQueue();
        }

        if (cardData.shuffleHand)
        {
            // HandManager.Instance.ShuffleHand();
        }

        if (cardData.removesCopFromQueue)
        {
            // QueueManager.Instance.RemoveCopFromQueue();
        }

        if (cardData.movesCopToEndOfQueue)
        {
            // QueueManager.Instance.MoveCopToEndOfQueue();
        }

        // Start customer exit from QueueManager directly
        QueueManager.Instance.StartCoroutine(QueueManager.Instance.HandleCustomerExit());
    }

    private void OnDestroy()
    {
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
        }
    }
}