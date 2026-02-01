using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Run Animation (Sprite Loop)")]
    [SerializeField] private Sprite[] runSprites;
    [SerializeField] private float runFrameRate = 12f; // frames per second

    [Header("Hurt Sprite")]
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private float hurtTime = 0.2f;

    [Header("Death Animation (Sprite Loop)")]
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] private float deathFrameRate = 10f;

    [Header("Restart")]
    [SerializeField] private KeyCode restartKey = KeyCode.R;
    [SerializeField] private bool restartOnAnyKey = true;

    [Header("FlowMeter")]
    [SerializeField] private FlowMeter flowMeter;

    private Transform _currentTarget;
    private bool _enemyNearby;

    private bool isDead = false;
    public bool IsDead => isDead; // read-only property

    private bool isHurt = false;
    private bool canRestart = false;

    private float runTimer;
    private int runFrameIndex;

    private float deathTimer;
    private int deathFrameIndex;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead)
        {
            HandleDeathAnimation();

            // pas restart toestaan als death anim klaar is
            if (canRestart)
            {
                if (restartOnAnyKey)
                {
                    if (Input.anyKeyDown) RestartScene();
                }
                else
                {
                    if (Input.GetKeyDown(restartKey)) RestartScene();
                }
            }

            return; // niks anders doen als je dood bent
        }

        // target zoeken in Update
        FindTarget();

        // input check in Update
        if (_currentTarget != null && Input.GetKeyDown(zapKey))
        {
            ZapToTarget(_currentTarget);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckFlowMeter();
        }
        // run animatie updaten
        HandleRunAnimation();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        HandleSlowMovement();
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

    private void HandleRunAnimation()
    {
        if (isHurt) return; // als hij hurt sprite toont, geen run frames zetten

        if (runSprites == null || runSprites.Length == 0) return;

        // anim alleen als player beweegt (x snelheid)
        if (Mathf.Abs(rb.linearVelocity.x) < 0.05f)
        {
            // stil -> eerste frame
            spriteRenderer.sprite = runSprites[0];
            return;
        }

        runTimer += Time.deltaTime;
        float frameTime = 1f / runFrameRate;

        if (runTimer >= frameTime)
        {
            runTimer = 0f;
            runFrameIndex = (runFrameIndex + 1) % runSprites.Length;
            spriteRenderer.sprite = runSprites[runFrameIndex];
        }
    }

    private void ZapToTarget(Transform target)
    {
        float oldY = rb.position.y;

        // STOP velocity eventjes zodat hij niet terugschiet
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // teleport via rb.position (geen jitter)
        rb.position = new Vector2(target.position.x, oldY);

        KillEnemy(target.gameObject);
    }

    private void KillEnemy(GameObject obj)
    {
        EnemyController enemy = obj.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Die();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDeathSFX);
            return;
        }

        WormController worm = obj.GetComponent<WormController>();
        if (worm != null)
        {
            worm.Die();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDeathSFX);
            return;
        }

        Destroy(obj);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // enemy killen
            KillEnemy(collision.gameObject);

            // player damage
            TakeDamage(1);
        }
    }

    // ==========================
    // DAMAGE / HEALTH
    // ==========================

    private void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update health UI
        HealthManager health = FindFirstObjectByType<HealthManager>();
        if (health != null)
            health.TakeDamage(dmg);

        if (currentHealth <= 0)
        {
            Die();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeathSFX);
        }
        else
        {
            ShowHurtSprite();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHurtSFX);
        }
    }


    private void ShowHurtSprite()
    {
        if (hurtSprite == null) return;

        isHurt = true;
        spriteRenderer.sprite = hurtSprite;

        CancelInvoke(nameof(EndHurt));
        Invoke(nameof(EndHurt), hurtTime);
    }

    private void EndHurt()
    {
        isHurt = false;
    }

    // ==========================
    // DEATH
    // ==========================

    private void Die()
    {
        isDead = true;
        canRestart = false;

        // stop movement
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false; // physics uit zodat hij niet meer botst

        // reset anim timers
        deathFrameIndex = 0;
        deathTimer = 0f;

        // eerste death sprite zetten
        if (deathSprites != null && deathSprites.Length > 0)
        {
            spriteRenderer.sprite = deathSprites[0];
        }
    }

    private void HandleDeathAnimation()
    {
        if (deathSprites == null || deathSprites.Length == 0)
        {
            // als er geen death anim is -> meteen restart
            canRestart = true;
            return;
        }

        // als anim al klaar is
        if (canRestart) return;

        deathTimer += Time.deltaTime;
        float frameTime = 1f / deathFrameRate;

        if (deathTimer >= frameTime)
        {
            deathTimer = 0f;
            deathFrameIndex++;

            // als laatste frame bereikt -> restart mogelijk maken
            if (deathFrameIndex >= deathSprites.Length)
            {
                // op laatste frame blijven
                spriteRenderer.sprite = deathSprites[deathSprites.Length - 1];
                canRestart = true;
                return;
            }

            spriteRenderer.sprite = deathSprites[deathFrameIndex];
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ==========================
    // GIZMOS
    // ==========================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, zapRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, slowMoRange);
    }
    private void CheckFlowMeter()
    {
        if (flowMeter == null) return;

        FlowMeter.FlowResult result = flowMeter.CheckTiming();
        HealthManager health = FindFirstObjectByType<HealthManager>();
        int baseScore = 10;

        // Controleer eerst of de bar überhaupt in een zone zit
        if (!flowMeter.IsBarInAnyZone())
        {
            // Buiten alle zones: hart verliezen
            if (health != null) health.TakeDamage(1);

            ComboManager.Instance.ResetMultiplier();
            return;
        }

        // Als we hier zijn, zit de bar in een zone
        switch (result)
        {
            case FlowMeter.FlowResult.Green:
                ScoreManager.Instance.AddScore(baseScore);
                ComboManager.Instance.ResetMultiplier();
                break;

            case FlowMeter.FlowResult.Yellow:
                int totalScore = baseScore * ComboManager.Instance.CurrentMultiplier;
                ScoreManager.Instance.AddScore(totalScore);
                ComboManager.Instance.IncreaseMultiplier();
                break;

            case FlowMeter.FlowResult.Miss:
                // theoretisch zou dit hier niet meer gebeuren
                if (health != null) health.TakeDamage(1);
                ComboManager.Instance.ResetMultiplier();
                break;
        }
    }



}
