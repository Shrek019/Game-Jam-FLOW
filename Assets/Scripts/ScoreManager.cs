using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score { get; private set; }
    public static int HighScore { get; private set; } // <-- static keeps it alive across scene reloads

    [Header("Score Settings")]
    [SerializeField] private int scorePerSecond = 1;

    private float _timer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject); // optional
    }

    private void Update()
    {
        // Don't count score if player is dead
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null && player.IsDead) return;

        _timer += Time.deltaTime;

        if (_timer >= 1f)
        {
            int secondsPassed = Mathf.FloorToInt(_timer);
            _timer -= secondsPassed;

            AddScore(secondsPassed * scorePerSecond);
        }
    }

    public void AddScore(int amount)
    {
        int oldScore = Score;
        Score += amount;

        // update session high score
        if (Score > HighScore)
        {
            HighScore = Score;
        }

        // Play 500-point sound
        if (Score / 500 > oldScore / 500)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.score500SFX);
        }
    }
}
