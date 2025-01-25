using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Add this for Image
using TMPro;  // Add this for TextMeshProUGUI
using DG.Tweening;  // Add DOTween namespace

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public List<CardClass> hand;
    [Header("Card Positions")]
    public Transform[] handPositions = new Transform[3];  // Array of positions for cards
    public Vector3 finalCardRotation = Vector3.zero; // Add this to make it configurable
    public int maxHandSize = 3;
    private int currentDrawIndex = 0;
    private bool isDrawing = false;
    public Transform playedCardPosition; // Add this field
    private bool[] occupiedPositions; // Track which positions are taken
    private bool hasPlayedCardThisTurn = false;

    public MarketClass targetMarket; // The market the player needs to pump
    public Image targetMarketIcon; // Only keep the icon

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            hand = new List<CardClass>();
            occupiedPositions = new bool[maxHandSize];
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
    }

    public void Initialize()
    {
        hasPlayedCardThisTurn = false;
        AssignRandomTargetMarket();
        StartCoroutine(DrawInitialHandCoroutine());
    }

    public void Reset()
    {
        hasPlayedCardThisTurn = false;
        FlushHand();
        targetMarket = null;
        UpdateTargetMarketUI();
    }

    private void AssignRandomTargetMarket()
    {
        targetMarket = MarketManager.Instance.AssignRandomMarket();
        UpdateTargetMarketUI();
    }

    private void UpdateTargetMarketUI()
    {
        if (targetMarketIcon != null)
        {
            if (targetMarket != null)
            {
                targetMarketIcon.sprite = targetMarket.marketData.marketIcon;
            }
            else
            {
                targetMarketIcon.sprite = null;
            }
        }
    }

    public bool IsTargetMarket(string marketName)
    {
        return targetMarket != null && targetMarket.marketData.marketName == marketName;
    }

    private IEnumerator DrawInitialHandCoroutine()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            yield return StartCoroutine(DrawCardCoroutine());
        }
    }

    private int FindFirstEmptyPosition()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            if (!occupiedPositions[i])
            {
                return i;
            }
        }
        return -1;
    }

    private IEnumerator DrawCardCoroutine()
    {
        if (hand.Count < maxHandSize && DeckManager.Instance != null)
        {
            int emptyPos = FindFirstEmptyPosition();
            if (emptyPos != -1)
            {
                CardClass newCard = DeckManager.Instance.DrawCard();
                if (newCard != null)
                {
                    // Handle animation here
                    Vector3 startPos = newCard.transform.position;
                    Vector3 endPos = handPositions[emptyPos].position;
                    Vector3 midPoint = Vector3.Lerp(startPos, endPos, 0.5f) + (Vector3.up * 0.3f);

                    Sequence drawSequence = DOTween.Sequence();
                    Vector3[] path = new Vector3[] { 
                        startPos,
                        midPoint,
                        endPos 
                    };
                    
                    drawSequence.Append(newCard.transform.DOPath(path, 0.5f, PathType.Linear)
                        .SetEase(Ease.Linear));
                    drawSequence.Join(newCard.transform.DORotate(handPositions[emptyPos].rotation.eulerAngles, 0.5f)
                        .SetEase(Ease.InOutSine)); // Updated to use handPosition's rotation

                    newCard.SetHandIndex(emptyPos);
                    hand.Add(newCard);
                    occupiedPositions[emptyPos] = true;
                    
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }

    public void DrawCard()
    {
        StartCoroutine(DrawCardCoroutine());
    }

    public bool CanPlayCards()
    {
        return QueueManager.Instance.currentPlayingCustomer != null && !hasPlayedCardThisTurn;
    }

    public void PlayCard(int cardIndex)
    {
        if (!CanPlayCards()) return;
        
        if (cardIndex >= 0 && cardIndex < maxHandSize && occupiedPositions[cardIndex])
        {
            CardClass cardToPlay = null;
            foreach (CardClass card in hand)
            {
                if (card.GetHandIndex() == cardIndex)
                {
                    cardToPlay = card;
                    break;
                }
            }

            if (cardToPlay != null)
            {
                hand.Remove(cardToPlay);
                occupiedPositions[cardIndex] = false;
                cardToPlay.Play(playedCardPosition);
                hasPlayedCardThisTurn = true;
            }
        }
    }

    public void OnCustomerExited()
    {
        hasPlayedCardThisTurn = false;
    }

    public void FlushHand()
    {
        foreach (var card in hand)
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }
        
        hand.Clear();
        
        // Reset occupied positions
        for (int i = 0; i < occupiedPositions.Length; i++)
        {
            occupiedPositions[i] = false;
        }
    }

    public IEnumerator ResetHand()
    {
        // First fade out all cards
        List<CardClass> cardsToRemove = new List<CardClass>(hand);
        foreach (var card in cardsToRemove)
        {
            if (card != null)
            {
                card.FadeOut();
                yield return new WaitForSeconds(0.2f); // Slight delay between each card fade
            }
        }

        // Wait for fade animations
        yield return new WaitForSeconds(0.5f);

        // Clear the hand
        FlushHand();    

        // Draw new cards
        yield return StartCoroutine(DrawInitialHandCoroutine());

        // After drawing new cards, handle customer exit
        yield return StartCoroutine(QueueManager.Instance.HandleCustomerExit());
    }

}