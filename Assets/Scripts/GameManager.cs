using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Button playResetButton;
    public TextMeshProUGUI statusText;  // Add reference to status text
    private bool isGameRunning = false;
    public bool IsGameOver { get; private set; }
    private bool hasPlayedBefore = false; // Add this to track if it's first time
    public Sprite playIcon;   // Reference to play icon sprite
    public Sprite restartIcon; // Reference to restart icon sprite
    public Image buttonIcon;   // Reference to the button's image component
    public GameObject winScreen;     // Changed to GameObject
    public GameObject loseScreen;    // Changed to GameObject

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
        AudioManager.Instance.PlayMenuMusic();  // Add this line to play menu music on start
        
        // Initialize screens as hidden
        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);
    }

    private void HandlePlayResetButton()
    {
        isGameRunning = !isGameRunning;
        UpdateButtonText();
        
        if (isGameRunning)
        {
            StartGame();
        }
        else
        {
            ResetGame();
            AudioManager.Instance.PlayMenuMusic();  // Add this line to play menu music when resetting
        }
    }

    private void ShowWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            loseScreen.SetActive(false);
        }
    }

    private void ShowLoseScreen()
    {
        if (loseScreen != null)
        {
            loseScreen.SetActive(true);
            winScreen.SetActive(false);
        }
    }

    private void HideResultScreens()
    {
        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);
    }

    private void HandleWinCondition(string marketName, bool isTargetMarket)
    {
        AudioManager.Instance.PlayWinSound();
        if (isTargetMarket)
        {
            DisplayStatus($"Congratulations! You successfully pumped {marketName} to 100%!");
            ShowWinScreen();
        }
        else
        {
            DisplayStatus($"Wrong market! {marketName} reached 100% but your target was {PlayerManager.Instance.targetMarket.marketData.marketName}");
            ShowLoseScreen();
        }
        GameOver();
    }

    private void HandleLoseCondition(string message)
    {
        DisplayStatus(message);
        ShowLoseScreen();
        GameOver();
    }

    public void WinGame(string marketName)
    {
        if (!isGameRunning) return;
        HandleWinCondition(marketName, PlayerManager.Instance.IsTargetMarket(marketName));
    }

    public void LoseGame(string marketName)
    {
        if (!isGameRunning) return;
        HandleLoseCondition($"Game Lost! Market {marketName} crashed to 0!");
    }

    public void LoseGameCop()
    {
        if (!isGameRunning) return;
        AudioManager.Instance.PlayCopCatchSound();
        HandleLoseCondition("Game Over - Busted by the cops!");
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
                HandleWinCondition(marketName, true);
            }
            else
            {
                HandleLoseCondition($"You lost! {marketName} only reached {targetValue}%");
            }
        }
    }

    private void UpdateButtonText()
    {
        if (buttonIcon != null)
        {
            if (!isGameRunning)
            {
                buttonIcon.sprite = playIcon;
            }
            else
            {
                buttonIcon.sprite = restartIcon;
            }
        }
    }

    private void StartGame()
    {
        ResetGame();
        DisplayStatus("Game Started!");
        Initialize();
        AudioManager.Instance.PlayGameplayMusic();
        HideResultScreens();
        
        MarketManager.Instance.Initialize();
        DeckManager.Instance.Initialize();
        QueueManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
        hasPlayedBefore = true; // Set this when game starts for the first time
    }

    private void ResetGame()
    {
        DisplayStatus("Game Reset!");
        Initialize();
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

    public void Initialize()
    {
        IsGameOver = false;
        
    }

    public void GameOver()
    {
        isGameRunning = false;
        IsGameOver = true;
        UpdateButtonText();
    }
}