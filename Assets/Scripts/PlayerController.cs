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
    [SerializeField] private float slowMovementFactor = 0.5f;
    [SerializeField] private float slowMoRange = 4.5f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private Transform _currentTarget;
    private bool _enemyNearby;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // target zoeken in Update (mag)
        FindTarget();

        // input check in Update (moet)
        if (_currentTarget != null && Input.GetKeyDown(zapKey))
        {
            ZapToTarget(_currentTarget);
        }
    }

    private void FixedUpdate()
    {
        HandleSlowMovement(); // physics movement in FixedUpdate
    }

    private void FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, zapRange, enemyLayer);
        _currentTarget = hit != null ? hit.transform : null;

        _enemyNearby = Physics2D.OverlapCircle(transform.position, slowMoRange, enemyLayer) != null;
    }

    private void HandleSlowMovement()
    {
        float currentSpeed = _enemyNearby ? moveSpeed * slowMovementFactor : moveSpeed;
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    private void ZapToTarget(Transform target)
    {
        float oldY = rb.position.y;

        // STOP velocity eventjes zodat hij niet "terugschiet"
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // teleport via rb.position (geen jitter)
        rb.position = new Vector2(target.position.x, oldY);

        KillEnemy(target.gameObject);

        ScoreManager.Instance.AddScore(10);
    }

    private void KillEnemy(GameObject obj)
    {
        EnemyController enemy = obj.GetComponent<EnemyController>();
        if (enemy != null) { enemy.Die(); return; }

        WormController worm = obj.GetComponent<WormController>();
        if (worm != null) { worm.Die(); return; }

        Destroy(obj);
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
            KillEnemy(collision.gameObject);

            HealthManager health = FindFirstObjectByType<HealthManager>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
        }
    }
}
