using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<CardScriptable> cardTemplates; // Lista di tutte le carte possibili
    public int initialDeckSize = 30; // Dimensione iniziale del deck
    private Queue<CardScriptable> deck; // Coda che rappresenta il mazzo

    void Start()
    {
        GenerateDeck();
    }

    // Genera il mazzo all'inizio del gioco
    private void GenerateDeck()
    {
        deck = new Queue<CardScriptable>();

        List<CardScriptable> shuffledDeck = new List<CardScriptable>();

        for (int i = 0; i < initialDeckSize; i++)
        {
            int randomIndex = Random.Range(0, cardTemplates.Count);
            shuffledDeck.Add(cardTemplates[randomIndex]);
        }

        // Mescola le carte
        for (int i = shuffledDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardScriptable temp = shuffledDeck[i];
            shuffledDeck[i] = shuffledDeck[randomIndex];
            shuffledDeck[randomIndex] = temp;
        }

        foreach (var card in shuffledDeck)
        {
            deck.Enqueue(card);
        }
    }

    // Pesca una carta dal mazzo e crea un'istanza
    public CardClass DrawCard()
    {
        if (deck.Count > 0)
        {
            CardScriptable cardData = deck.Dequeue();
            return CreateCardInstance(cardData);
        }
        else
        {
            Debug.Log("Deck is empty! Generating a new deck.");
            GenerateDeck();
            CardScriptable cardData = deck.Dequeue();
            return CreateCardInstance(cardData);
        }
    }

    // Metodo per creare un'istanza della carta
    private CardClass CreateCardInstance(CardScriptable cardData)
    {
        GameObject cardObject = new GameObject(cardData.cardName);
        CardClass cardClass = cardObject.AddComponent<CardClass>();
        cardClass.Initialize(cardData);
        return cardClass;
    }

    // Restituisce il numero di carte rimanenti nel mazzo
    public int GetRemainingCards()
    {
        return deck.Count;
    }
}