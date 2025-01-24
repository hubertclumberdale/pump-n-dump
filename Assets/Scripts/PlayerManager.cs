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
    private bool[] occupiedPositions; // Track which positions are taken

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

        StartCoroutine(DrawInitialHandCoroutine());

        StartBreathing();
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
                    drawSequence.Join(newCard.transform.DORotate(Vector3.zero, 0.5f));

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
        return QueueManager.Instance.currentPlayingCustomer != null;
    }

    public void PlayCard(int cardIndex)
    {
        if (!CanPlayCards()) return;
        
        if (cardIndex >= 0 && cardIndex < maxHandSize && occupiedPositions[cardIndex])
        {
            CardClass cardToPlay = null;
            // Find the card with matching index
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
                DrawCard();
            }
        }
    }

    private void StartBreathing()
    {
        Quaternion initialRotation = Camera.main.transform.localRotation;
          Camera.main.transform.DORotateQuaternion(initialRotation * Quaternion.Euler(0.5f, 0, -0.8f), 3f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}