using System.Collections;
using UnityEngine;
using TMPro; // Use TextMesh Pro for score display
using UnityEngine.UI; // Add this line to use UI components like Slider
using UnityEngine.SceneManagement; // เพิ่มบรรทัดนี้


public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private float speed = 5f;
    public bool isFacingRight = true;
    private float move = 0;

    [Header("Dash")]
    private bool isDashing;
    [SerializeField] private float dashPower = 10f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private float lastDashTime;

    [Header("Score")]
    private int score = 0;
    [SerializeField] private Text scoreText; // Use TextMesh Pro for score display

    [Header("Attack")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.5f;
    private float lastAttackTime;

    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    private int health;
    [SerializeField] private Slider healthBar; // Use Slider from UnityEngine.UI

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        lastDashTime = -dashCooldown;
        health = maxHealth;
        UpdateHealthBar();
        UpdateScoreUI();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            HandleMovement();
        }
        HandleAnimation();
    }

    private void HandleMovement()
    {
        move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(speed * move, rb.velocity.y);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        else
        {
            UnityEngine.Debug.LogError("scoreText is not assigned in the Inspector!");
        }
    }

    private void HandleAnimation()
    {
        anim.SetFloat("speed", Mathf.Abs(move));
        if (move > 0 && !isFacingRight || move < 0 && isFacingRight)
        {
            Flip();
        }
        anim.SetBool("walk", Mathf.Abs(move) > 0.1f);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.E) && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        anim.SetTrigger("Attack");
        PlaySound(attackSound);

        Vector2 attackPosition = transform.position + (isFacingRight ? Vector3.right : Vector3.left) * attackRange / 2;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRange / 2, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                AddScore(10);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        
        health -= damage;
        UpdateHealthBar();
        PlaySound(hurtSound);
        anim.SetTrigger("Hurt");

        if (health <= 0)
        {
            Die();
            SceneManager.LoadScene(0);
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.value = (float)health / maxHealth;
    }

    private void Die()
    {
        anim.SetTrigger("Die");
        enabled = false; // ปิดการควบคุมผู้เล่น
        rb.velocity = Vector2.zero; // หยุดการเคลื่อนไหว

        // ทำให้ผู้เล่นไม่สามารถโต้ตอบได้
        // สามารถใส่โค้ดเพื่อเรียกหน้าจอ Game Over หรือ Restart ได้ที่นี่
        StartCoroutine(WaitAndDestroy(2f)); // ตัวอย่างเช่น ให้รอ 2 วินาทีก่อนที่จะลบผู้เล่น
    }

    private IEnumerator WaitAndDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject); // ลบผู้เล่น
    }


    private IEnumerator Dash()
    {
        isDashing = true;
        anim.SetTrigger("Dash");
        lastDashTime = Time.time;
        rb.velocity = new Vector2(dashPower * (isFacingRight ? 1 : -1), 0);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        rb.velocity = Vector2.zero;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 attackPosition = transform.position + (isFacingRight ? Vector3.right : Vector3.left) * attackRange / 2;
        Gizmos.DrawWireSphere(attackPosition, attackRange / 2);
    }
}
