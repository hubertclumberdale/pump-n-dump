using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Button playResetButton;
    public TextMeshProUGUI statusText;  // Add reference to status text
    private bool isGameRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (playResetButton != null)
        {
            playResetButton.onClick.AddListener(HandlePlayResetButton);
            UpdateButtonText();
        }
    }

    private void HandlePlayResetButton()
    {
        // Update game state first
        isGameRunning = !isGameRunning;
        UpdateButtonText();  // Update text before actions
        
        if (isGameRunning)
        {
            StartGame();
        }
        else
        {
            ResetGame();
        }
    }

    public void WinGame(string marketName)
    {
        if (!isGameRunning) return;
        
        bool isTargetMarket = PlayerManager.Instance.IsTargetMarket(marketName);
                             
        if (isTargetMarket)
        {
            DisplayStatus($"Congratulations! You successfully pumped {marketName} to 100%!");
        }
        else
        {
            DisplayStatus($"Wrong market! {marketName} reached 100% but your target was {PlayerManager.Instance.targetMarket.marketData.marketName}");
        }
        
        isGameRunning = false;
        UpdateButtonText();
    }

    public void LoseGame(string marketName)
    {
        if (!isGameRunning) return;
        
        DisplayStatus($"Game Lost! Market {marketName} crashed to 0!");
        isGameRunning = false;
        // TODO: Show lose screen or animation
        UpdateButtonText();
    }

    public void LoseGameCop()
    {
        if (!isGameRunning) return;
        
        DisplayStatus("Game Over - Busted by the cops!");
        isGameRunning = false;
        UpdateButtonText();
    }

    public void EndGameDeckEmpty()
    {
        if (!isGameRunning) return;
        
        DisplayStatus("Game Over - No more cards in deck!");
        
        if (PlayerManager.Instance.targetMarket != null)
        {
            float targetValue = PlayerManager.Instance.targetMarket.marketValue;
            string marketName = PlayerManager.Instance.targetMarket.marketData.marketName;
            
            if (targetValue >= 100)
            {
                DisplayStatus($"You won! Successfully pumped {marketName} to {targetValue}%!");
            }
            else
            {
                DisplayStatus($"You lost! {marketName} only reached {targetValue}%");
            }
        }
        
        isGameRunning = false;
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        TextMeshProUGUI buttonText = playResetButton?.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = isGameRunning ? "Reset" : "Play Again";
            Debug.Log($"Button text updated to: {buttonText.text}");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in button children!");
        }
    }

    private void StartGame()
    {
        DisplayStatus("Game Started!");
        MarketManager.Instance.Initialize();
        DeckManager.Instance.Initialize();
        QueueManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
    }

    private void ResetGame()
    {
        DisplayStatus("Game Reset!");
        PlayerManager.Instance.Reset(); 
        MarketManager.Instance.Reset();
        DeckManager.Instance.Reset();
        QueueManager.Instance.Reset();
    }

    private void DisplayStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}