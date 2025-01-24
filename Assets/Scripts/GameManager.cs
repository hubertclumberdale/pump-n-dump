using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Button playResetButton;
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
            Debug.Log($"Congratulations! You successfully pumped {marketName} to 100%!");
        }
        else
        {
            Debug.Log($"Wrong market! {marketName} reached 100% but your target was {PlayerManager.Instance.targetMarket.marketData.marketName}");
        }
        
        isGameRunning = false;
        UpdateButtonText();
    }

    public void LoseGame(string marketName)
    {
        if (!isGameRunning) return;
        
        Debug.Log($"Game Lost! Market {marketName} crashed to 0!");
        isGameRunning = false;
        // TODO: Show lose screen or animation
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
        MarketManager.Instance.Initialize();
        DeckManager.Instance.Initialize();
        QueueManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
    }

    private void ResetGame()
    {
        PlayerManager.Instance.Reset(); // This will handle both hand and target market reset
        MarketManager.Instance.Reset();
        DeckManager.Instance.Reset();
        QueueManager.Instance.Reset();
    }
}