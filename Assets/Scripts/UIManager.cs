using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI; // Add this using directive to resolve 'Image'

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _aiCountText;
    [SerializeField] private TextMeshProUGUI _timeRemainingText;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private Image _warningPanel;
    [SerializeField] private Image _winLosePanel; // Add this for win/lose screen background
    [SerializeField] private TextMeshProUGUI _winLoseText; // Add this for win/lose messages

    private int _score = 0;
    private int _escapedCount = 0;
    private int _ammo = 30;
    private bool _isReloading = false;
    private Coroutine _blinkCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _ammoText.text = $"Ammo: {_ammo}";
        
        _warningPanel.enabled = false; // Ensure the warning panel is hidden at the start

        _winLosePanel.enabled = false; // Ensure the win/lose panel is hidden at the start
    }

    public void UpdateScore(int points)
    {
        _score += points;
        _scoreText.text = $"Score: {_score}";
    }

    public void SetEscapedCount(int count)
    {
        _escapedCount = count;
        _aiCountText.text = $"Escaped: {_escapedCount}/10";

        GameManager.Instance.CheckLoseCondition(_escapedCount); // Check lose condition whenever escaped count updates

        if (_escapedCount > 6)
        {
            if (_blinkCoroutine == null)
            {
                _blinkCoroutine = StartCoroutine(BlinkWarning());
            }
        }
        else
        {
            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }
        }
    }

    private IEnumerator BlinkWarning()
    {
        while (_escapedCount > 6)
        {
            _warningPanel.enabled = true;
            yield return new WaitForSeconds(0.5f);
            _warningPanel.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void UpdateAmmo (int amount)
    {
        _ammo = amount;
        if (_ammo <= 0 && !_isReloading)
            StartReloadTimer();
        else
            _ammoText.text = $"Ammo: {_ammo}";
    }

    private void StartReloadTimer()
    {
        StartCoroutine(ReloadCountdown());
    }

    private IEnumerator ReloadCountdown()
    {
        _isReloading = true;
        int reloadTime = 15;
        
        while (reloadTime >= 0)
        {
            _ammoText.text = $"Reload: {reloadTime}s";
            yield return new WaitForSeconds(1f);
            reloadTime--;
        }

        _ammo = 30;
        _isReloading = false;
        _ammoText.text = $"Ammo: {_ammo}";

        Object.FindFirstObjectByType<PlayerShooter>().ResetAmmo();
    }

    public void UpdateTimeRemaining(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        _timeRemainingText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    public int GetCurrentAmmo() => _ammo;

    public void ShowGameOver(bool isWin)
    {
        _winLosePanel.enabled = true; // Show the panel background
        _winLoseText.text = isWin ? "You Won!" : "Game Over!";
    }

    public void HideGameOver()
    {
        _winLosePanel.enabled = false; // Hide the panel background
        _winLoseText.text = ""; // Clear the text
    }
}