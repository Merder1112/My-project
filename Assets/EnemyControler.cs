using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] public EnemySpawner enemySpawner; // Public reference

    [Header("Movement")]
    [SerializeField] private float agroRange = 5f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;

    [Header("Health")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    private Animator anim;
    private Rigidbody2D rb;
    private float lastAttackTime;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;

        // Find the player
        PlayerControl playerControl = FindObjectOfType<PlayerControl>();
        if (playerControl != null)
        {
            player = playerControl.transform;
        }
        else
        {
            UnityEngine.Debug.LogError("PlayerControl not found in the scene!");
        }
    }

    private void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ตรวจสอบว่าอยู่ในระยะการโจมตี
        if (distanceToPlayer <= attackRange)
        {
            // ถ้าไม่ได้โจมตีในช่วง cooldown
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
            else
            {
                StopChasingPlayer(); // หยุดการไล่ตามเมื่อโจมตี
            }
        }
        else if (distanceToPlayer <= agroRange)
        {
            ChasePlayer();
        }
        else
        {
            StopChasingPlayer();
        }
    }


    private void ChasePlayer()
    {
        anim.SetBool("Run", true);
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        FlipSprite(direction.x);
    }

    private void StopChasingPlayer()
    {
        rb.velocity = Vector2.zero;
        anim.SetBool("Run", false);
    }

    private void AttackPlayer()
    {
        lastAttackTime = Time.time;
        anim.SetTrigger("Attack");
        PlaySound(attackSound);

        // ตรวจสอบการหาผู้เล่นที่อยู่ในระยะการโจมตี
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            PlayerControl playerControl = hitPlayer.GetComponent<PlayerControl>();
            if (playerControl != null)
            {
                playerControl.TakeDamage(attackDamage);
            }
        }
    }



    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("Hurt");
        PlaySound(hurtSound);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("Die");
        PlaySound(deathSound);
        if (enemySpawner != null)
        {
            enemySpawner.EnemyDefeated(gameObject);
        }
        Destroy(gameObject, 2f);
    }

    private void FlipSprite(float directionX)
    {
        // Flip the sprite based on the direction
        if (directionX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Facing right
        }
        else if (directionX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Facing left
        }
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
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
