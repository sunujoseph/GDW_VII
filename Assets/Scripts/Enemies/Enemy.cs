using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public Vector3 pointA;   // Patrol start position
    [SerializeField] public Vector3 pointB;      // Patrol end position
    [SerializeField] public float patrolSpeed = 2f;  // Speed of patrol movement

    [SerializeField] protected int health = 1;
    [SerializeField] public float toughness = 1f;
    [SerializeField] public bool isPatrolActive = true;

    [SerializeField] public bool canDamagePlayer = true; // New flag for damage behavior

    //sounds
    [SerializeField] public AudioClip damageSound;
    [SerializeField] public AudioClip deathSound;
    [SerializeField] public AudioClip attackSound;


    private Vector3 targetPosition; // Current target position for patrol

    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isStunned = false;

    PlayerHealth playerHealth;
    private Transform playerFindTransform; // Reference to the player's transform

    public bool isAlive = true;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //pointA = transform.position;
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerFindTransform = GameObject.FindWithTag("Player").transform;
        targetPosition = transform.position; // Start moving towards pointB
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isPatrolActive)
        {
            Patrol();
        }
        else
        {
            FacePlayer();
        }
        
    }

    protected void Patrol()
    {
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);

        // Check if we've reached the target position and swap target between pointA and pointB
        if (Vector2.Distance(transform.position, targetPosition) <= 0.1f)
        {
            // Swap target position between pointA and pointB
            //targetPosition = targetPosition == pointA ? pointB : pointA;
            targetPosition = targetPosition == pointA ? pointB : pointA;
            Flip();

        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight; // Toggle facing direction
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Invert the x scale to flip the sprite
        transform.localScale = localScale;
    }


    private void FacePlayer()
    {
        if (playerFindTransform == null) return;

        // Check if the player is to the left or right of the enemy
        if ((playerFindTransform.position.x < transform.position.x && isFacingRight) ||
            (playerFindTransform.position.x > transform.position.x && !isFacingRight))
        {
            Flip();
        }
    }

    // When Enemy takes damage
    public virtual void TakeDamage(int amount, float knockbackForce, float breakDamage)
    {
        if (isStunned)
        {
            // double damage
        }
        else
        {
            // normal
        }

        health -= amount;
        if (health <= 0)
        {
            Die(knockbackForce);
            SoundManager.instance.Play(deathSound, transform, 1.0f);
        }
        else
        {
            SoundManager.instance.Play(damageSound, transform, 1.0f);
            
        }
    }

    public virtual void TakeBreakDmage(float breakDamage)
    {
        toughness -= breakDamage;
        if (toughness <= 0)
        {
            Stunned();
        }
    }

    // Call when Enemy dies
    protected void Die(float knockbackForce)
    {
        isAlive = false;

        // Disable enemy behavior
        isPatrolActive = false;
        GetComponent<Collider2D>().isTrigger = false;
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;

        rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        Vector2 knockbackDirection = (transform.position - playerFindTransform.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        float randomSpin = Random.Range(-10f, 10f);
        rb.AddTorque(randomSpin, ForceMode2D.Impulse);


        // Destroy enemy after delay
        StartCoroutine(DestroyAfterDelay(2f));

        //Destroy(gameObject);
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    protected void Stunned()
    {
        // double damage
        isStunned = true;
    }

    protected void RecoverFromStun()
    {
        isStunned = false;
    }
  

    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        if (otherObject.CompareTag("Player"))
        {
            // If the player is invulnerable
            if (canDamagePlayer && otherObject.gameObject.layer != LayerMask.NameToLayer("Invulnerable"))
            {
                Debug.Log("Player hit by enemy!");

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                }
            }
            else
            {
                Debug.Log("Player is invulnerable, no damage taken.");
            }
        }
        else if (otherObject.CompareTag("PlayerProjectile"))
        {
            TakeDamage(1, 1f, 1f);
        }
    }



    private void OnDrawGizmos()
    {
        // Draw lines between the two points for visualization in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
    }

}
