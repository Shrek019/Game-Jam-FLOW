using UnityEngine;

public class WormController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f; // naar links

    [Header("Sprite Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 10f;

    [Header("Death Settings")]
    [SerializeField] private Sprite deadSprite;
    [SerializeField] private float deathDuration = 2f;

    private bool _isDead;

    private int _frameIndex;
    private float _frameTimer;

    private Rigidbody2D _rb;
    private Collider2D _col;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (_isDead) return;

        // constant naar links
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        AnimateSprite();
    }

    private void AnimateSprite()
    {
        if (frames == null || frames.Length == 0 || spriteRenderer == null) return;

        _frameTimer += Time.deltaTime;
        float frameTime = 1f / fps;

        if (_frameTimer >= frameTime)
        {
            _frameTimer -= frameTime;
            _frameIndex = (_frameIndex + 1) % frames.Length;
            spriteRenderer.sprite = frames[_frameIndex];
        }
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        // sprite veranderen
        if (spriteRenderer != null && deadSprite != null)
            spriteRenderer.sprite = deadSprite;

        // collider uit
        if (_col != null)
            _col.enabled = false;

        // physics stoppen zodat hij niet valt
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Destroy(gameObject, deathDuration);
    }
}
