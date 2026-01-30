using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score { get; private set; }

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
        // DontDestroyOnLoad(gameObject); // alleen nodig als je score tussen scenes wil houden
    }

    private void Update()
    {
        // elke seconde +1
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
        Score += amount;
        // Debug.Log("Score: " + Score);
    }
}
