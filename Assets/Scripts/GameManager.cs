using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score = 0;

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

    // CHANGED: Add score on enemy death
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score: {score}");
    }

    public int GetScore() => score;
}