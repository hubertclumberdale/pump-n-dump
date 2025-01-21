using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<CardClass> hand;
    public DeckManager deckManager;
    public int maxHandSize = 3;

    void Start()
    {
        DrawInitialHand();
    }

    public void DrawInitialHand()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        if (hand.Count < maxHandSize)
        {
            CardClass newCard = deckManager.DrawCard();
            if (newCard != null)
            {
                hand.Add(newCard);
                // Aggiorna l'UI qui se necessario
            }
        }
    }

    public void PlayCard(int cardIndex, CustomerClass targetCustomer)
    {
        if (cardIndex >= 0 && cardIndex < hand.Count)
        {
            CardClass cardToPlay = hand[cardIndex];
            cardToPlay.Play(targetCustomer);
            hand.RemoveAt(cardIndex);
            DrawCard();
        }
    }
}