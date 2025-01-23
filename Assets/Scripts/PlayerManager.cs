using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // Add DOTween namespace

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public List<CardClass> hand;
    public Transform[] handPositions = new Transform[3];  // Array of positions for cards
    public int maxHandSize = 3;
    private int currentDrawIndex = 0;
    private bool isDrawing = false;
    public Transform playedCardPosition; // Add this field

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            hand = new List<CardClass>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (handPositions == null || handPositions.Length < maxHandSize)
        {
            Debug.LogError("Hand positions not properly set in PlayerManager!");
            return;
        }

        if (DeckManager.Instance == null)
        {
            Debug.LogError("DeckManager instance not found!");
            return;
        }

        StartCoroutine(DrawInitialHandCoroutine());
    }

    private IEnumerator DrawInitialHandCoroutine()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            yield return StartCoroutine(DrawCardCoroutine());
        }
    }

    private IEnumerator DrawCardCoroutine()
    {
        if (hand.Count < maxHandSize && DeckManager.Instance != null)
        {
            CardClass newCard = DeckManager.Instance.DrawCard(handPositions[hand.Count]);
            if (newCard != null)
            {
                int handIndex = hand.Count;
                newCard.SetHandIndex(handIndex);
                hand.Add(newCard);
                yield return new WaitForSeconds(DeckManager.Instance.drawAnimationDuration);
            }
        }
    }

    public void DrawCard()
    {
        StartCoroutine(DrawCardCoroutine());
    }

    public bool CanPlayCards()
    {
        return QueueManager.Instance.currentPlayingCustomer != null;
    }

    public void PlayCard(int cardIndex)
    {
        if (!CanPlayCards()) return;
        
        if (cardIndex >= 0 && cardIndex < hand.Count)
        {
            CardClass cardToPlay = hand[cardIndex];
            hand.RemoveAt(cardIndex);
            cardToPlay.Play(playedCardPosition);
            DrawCard();
        }
    }
}