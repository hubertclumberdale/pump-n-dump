using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;  // Add this line for IEnumerator

public class CardClass : MonoBehaviour
{
    public CardScriptable cardData;
    public Button cardButton;
    private float playAnimationDuration = 0.5f;
    private float shakeAnimationDuration = 0.3f;
    private float fadeAnimationDuration = 0.3f;
    private int handIndex = -1;

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

    public void Initialize(CardScriptable data)
    {
        cardData = data;
    }

    public void Play(Transform playedPosition)
    {
        cardButton.interactable = false; // Prevent multiple clicks

        Sequence playSequence = DOTween.Sequence();

        playSequence.Append(transform.DOMove(playedPosition.position, playAnimationDuration)
            .SetEase(Ease.OutQuad));
        
        playSequence.Append(transform.DOShakePosition(shakeAnimationDuration, 0.5f, 10, 90, false));
        
        playSequence.Append(transform.DOScale(Vector3.zero, fadeAnimationDuration)
            .SetEase(Ease.InBack));

        playSequence.OnComplete(() => {
            ApplyCardEffects();
            // Move card destruction to after effects are applied
            QueueManager.Instance.StartCoroutine(DestroyAfterEffects());
        });
    }

    private IEnumerator DestroyAfterEffects()
    {
        // Wait a small amount of time to ensure effects are processed
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void ApplyCardEffects()
    {
        if (cardData.valueForTarget != 0)
        {
            // targetCustomer.Company.ApplyValue(cardData.valueForTarget);
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