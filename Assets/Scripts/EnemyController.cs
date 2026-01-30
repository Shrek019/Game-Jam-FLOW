using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Death Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite deadSprite;
    [SerializeField] private float deathDuration = 2f;

    private Vector3 _startPos;
    private bool _isDead = false;

    private void Awake()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        if (!_isDead)
        {
            // zweef op en neer
            float newY = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    // Wordt aangeroepen door player
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        // verander sprite naar dood
        if (spriteRenderer != null && deadSprite != null)
            spriteRenderer.sprite = deadSprite;

        // disable collider zodat player niet opnieuw kan botsen
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // stop physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // destroy na paar seconden
        Destroy(gameObject, deathDuration);
    }
}
