using UnityEngine;

public class CardClass : MonoBehaviour
{
    public CardScriptable cardData;

    public void Initialize(CardScriptable data)
    {
        cardData = data;
    }

    public void Play(CustomerClass targetCustomer)
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
    }
}