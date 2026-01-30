using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Zap Settings")]
    [SerializeField] private float zapRange = 3f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private KeyCode zapKey = KeyCode.Space;

    [Header("Slow Movement Settings")]
    [SerializeField] private float slowMovementFactor = 0.5f; // snelheid factor als enemy dichtbij
    [SerializeField] private float slowMoRange = 4.5f;         // bereik waarin de movement vertraagd

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private Transform _currentTarget;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Vind target in zapRange
        FindTarget();

        // Beweeg player naar rechts met slow factor als enemy dichtbij
        HandleSlowMovement();

        // Zap input
        if (_currentTarget != null && Input.GetKeyDown(zapKey))
        {
            ZapToTarget(_currentTarget);
        }
    }

    private void FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, zapRange, enemyLayer);
        _currentTarget = hit != null ? hit.transform : null;
    }

    private void HandleSlowMovement()
    {
        bool enemyNearby = Physics2D.OverlapCircle(transform.position, slowMoRange, enemyLayer) != null;
        float currentSpeed = enemyNearby ? moveSpeed * slowMovementFactor : moveSpeed;
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    private void ZapToTarget(Transform target)
    {
        float oldY = transform.position.y;

        // teleport naar enemy (X overnemen, Y blijft player hoogte)
        transform.position = new Vector3(target.position.x, oldY, transform.position.z);

        // probeer EnemyController te pakken en call Die()
        EnemyController enemy = target.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Die();
        }

        // score +10
        ScoreManager.Instance.AddScore(10);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, zapRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, slowMoRange);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 1. enemy dood
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Die();
            }

            // 2. player hartje verliezen
            HealthManager health = FindFirstObjectByType<HealthManager>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
        }
    }


}
