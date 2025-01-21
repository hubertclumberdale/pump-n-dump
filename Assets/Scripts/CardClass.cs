using UnityEngine;

public class CardClass : MonoBehaviour
{
    private CardScriptable cardData;

    public void Initialize(CardScriptable data)
    {
        cardData = data;
    }

    public void Play(CustomerClass targetCustomer)
    {
        // Logica per applicare l'effetto della carta
        switch (cardData.effectType)
        {
            case CardEffectType.Plus:
                targetCustomer.ApplyEffect(cardData.bonusValue);
                break;
            case CardEffectType.Leave:
                targetCustomer.ApplyEffect(-cardData.malusValue);
                break;
            // Altri casi per altri tipi di effetti
        }

        Debug.Log($"Played {cardData.cardName} on {targetCustomer.customerName}");
    }
}