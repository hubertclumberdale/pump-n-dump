using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // Add DOTween namespace

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }
    public GameObject cardPrefab; // Changed from List to single prefab
    public int initialDeckSize = 30; // Dimensione iniziale del deck
    public Transform deckPosition; // Position where deck is stacked
    public float cardSpacing = 0.01f; // Vertical spacing between cards
    public float drawAnimationDuration = 0.5f; // Duration of draw animation
    private Stack<CardClass> deck; // Changed from Queue to Stack

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            deck = new Stack<CardClass>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (Instance == this)
        {
            GenerateDeck();
        }
    }

    // Genera il mazzo all'inizio del gioco
    private void GenerateDeck()
    {
        deck = new Stack<CardClass>();
        Vector3 currentPosition = deckPosition.position;
        Quaternion faceDownRotation = Quaternion.Euler(270, 0, 0); // Changed to 270 degrees to face down

        for (int i = 0; i < initialDeckSize; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, currentPosition, faceDownRotation);
            CardClass cardClass = cardObject.GetComponent<CardClass>();
            if (cardClass == null)
            {
                Debug.LogError("Card prefab is missing CardClass component!");
                continue;
            }
            deck.Push(cardClass);
            currentPosition += Vector3.up * cardSpacing;
        }
    }

    // Simplified draw method that just returns a card
    public CardClass DrawCard()
    {
        if (deck.Count > 0)
        {
            CardClass drawnCard = deck.Pop();
            drawnCard.transform.rotation = Quaternion.Euler(270, 0, 0); // Keep face down
            return drawnCard;
        }
        else
        {
            Debug.Log("Deck is empty! Generating a new deck.");
            GenerateDeck();
            return DrawCard();
        }
    }

    // Restituisce il numero di carte rimanenti nel mazzo
    public int GetRemainingCards()
    {
        return deck.Count;
    }
}