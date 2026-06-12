using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score = 0;

    private float _levelTimer = 180f; // 3 minutes in seconds
    private bool _gameActive = true;

    public bool IsGameActive() => _gameActive;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UIManager.Instance.UpdateTimeRemaining(_levelTimer);
    }

    private void Update()
    {
        if (!_gameActive) return;

        _levelTimer -= Time.deltaTime;
        UIManager.Instance.UpdateTimeRemaining(_levelTimer);

        if (_levelTimer <= 0f)
        {
            WinLevel();
        }
    }

    // Add score on enemy death
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score: {score}");
        UIManager.Instance.UpdateScore(points);
    }

    public void CheckLoseCondition(int escapedEnemyCount)
    {
        if (escapedEnemyCount >= 10)
        {
            LoseLevel();
        }
    }

    void WinLevel()
    {
        _gameActive = false;
        Debug.Log("You Won!");
        UIManager.Instance.ShowGameOver(true);
        Time.timeScale = 0f; // Pause the game
    }

    void LoseLevel()
    {
        _gameActive = false;
        Debug.Log("You Lost!");
        UIManager.Instance.ShowGameOver(true);
        Time.timeScale = 0f; // Pause the game 
    }

    public int GetScore() => score;
}