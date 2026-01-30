using UnityEngine;
using UnityEngine.UI; // voor UI images
using TMPro;          // voor restart text

public class HealthManager : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("UI Heart Sprites")]
    [SerializeField] private Image[] hearts;       // sleep je 3 UI images hier
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [Header("Restart UI")]
    [SerializeField] private GameObject restartUI; // TextMeshPro of Canvas object
    [SerializeField] private KeyCode restartKey = KeyCode.Space;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
        restartUI.SetActive(false);
    }

    private void Update()
    {
        if (isDead && Input.GetKeyDown(restartKey))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHeartsUI();

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    private void Die()
    {
        isDead = true;
        restartUI.SetActive(true);
        // eventueel player stoppen (beweging uitzetten)
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.enabled = false;
    }
}
