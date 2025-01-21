using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<CardScriptable> deck;

    public CardClass DrawCard()
    {
        if (deck.Count > 0)
        {
            CardScriptable cardData = deck[Random.Range(0, deck.Count)];
            return CreateCardInstance(cardData);
        }
        return null;
    }

    private CardClass CreateCardInstance(CardScriptable cardData)
    {
        GameObject cardObject = new GameObject(cardData.cardName);
        CardClass cardClass = cardObject.AddComponent<CardClass>();
        cardClass.Initialize(cardData);
        return cardClass;
    }
}