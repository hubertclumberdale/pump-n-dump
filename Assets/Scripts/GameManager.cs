using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public MarketManager marketManager;
    public bool canPlay = true;

    private void Start()
    {
        StartGame();
    }

    // Inizia la partita
    public void StartGame()
    {
        /* playerManager.DrawInitialHand(); */
        StartTurn();
    }

    // Inizia un nuovo turno
    public void StartTurn()
    {
        if (!canPlay)
        {
            return;
        }

        QueueManager.Instance.AdvanceQueue();
        CheckGameState();
        canPlay = true;
    }

    // Metodo invocato dal player per giocare una carta
    public void CardPlayed(int cardIndex)
    {
        if (!canPlay)
        {
            return;
        }

        canPlay = false;
        CheckGameState();
        StartTurn();
    }

    // Controlla le condizioni di vittoria o sconfitta
    public void CheckGameState()
    {
        foreach (var market in marketManager.markets)
        {
            if (market.marketValue == 0)
            {
                Debug.Log($"{market.marketData.marketName} has reached 0 and is out of the game.");
                marketManager.RemoveMarket(market);
                //queueManager.RemoveCustomersByCompany(market.marketData.company);
            }
            else if (market.marketValue == 100)
            {
                /* if (market.marketData.company == playerManager.assignedCompany)
                {
                    Debug.Log("You win!");
                    EndGame(true);
                }
                else
                {
                    Debug.Log("You lose!");
                    EndGame(false);
                } */
            }
        }
    }

    // Termina la partita
    public void EndGame(bool hasWon)
    {
        canPlay = false;
        if (hasWon)
        {
            Debug.Log("Congratulations! You have won the game.");
        }
        else
        {
            Debug.Log("Game Over! You have lost the game.");
        }
    }
}