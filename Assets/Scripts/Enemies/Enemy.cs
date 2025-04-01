using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    // Ground Layer hitbox
    [SerializeField] private bool respectEnvironmentHitbox = true;
    [SerializeField] private LayerMask environmentHitboxLayer;

    private Vector3 targetPosition; // Current target position for patrol

    private Rigidbody2D rb;
    public bool isFacingRight = true;
    public bool isStunned = false;

    public PlayerHealth playerHealth;
    public Transform playerFindTransform; // Reference to the player's transform

    public bool isAlive = true;
    Material mat;
    float fade;
    bool isDissolving;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //pointA = transform.position;
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerFindTransform = GameObject.FindWithTag("Player").transform;

        mat = GetComponent<SpriteRenderer>().material;
        fade = 1.0f;


        if (environmentHitboxLayer == 0)
        {
            environmentHitboxLayer = LayerMask.GetMask("Ground");
        }
        
        pointA = transform.position;

        targetPosition = pointA;
        //targetPosition = transform.position; // Start moving towards pointB
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

        DeathDissolve();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) // Only updates in Editor
        {
            pointA = transform.position;

            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }


    protected void Patrol()
    {
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);

        // Check if we've reached the target position and swap target between pointA and pointB
        if (Vector2.Distance(transform.position, targetPosition) <= 0.1f)
        {
            targetPosition = targetPosition == pointA ? pointB : pointA;

            // Flip only if the new direction is different
            if ((targetPosition.x < transform.position.x && isFacingRight) ||
                (targetPosition.x > transform.position.x && !isFacingRight))
            {
                Flip();
            }
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

            // Start knockback effect (only if still alive)
            StartCoroutine(KnockbackEffect(knockbackForce));

            // Start blinking effect
            StartCoroutine(BlinkRed());

        }
    }

    private IEnumerator KnockbackEffect(float knockbackForce)
    {
        float knockbackDuration = 0.2f; // Short knockback effect time
        float elapsed = 0f;

        Vector3 knockbackDirection = (transform.position - playerFindTransform.position).normalized;
        knockbackDirection.y = 0f;
        knockbackDirection.Normalize();

        //Vector3 targetPosition = transform.position + (knockbackDirection * (knockbackForce * 0.05f));
        Vector3 targetPosition = transform.position + (knockbackDirection * (knockbackForce * 0.05f));

        Vector3 startPos = transform.position;

        if (respectEnvironmentHitbox)
        {
            RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            GetComponent<Collider2D>().bounds.size,
            0f,
            knockbackDirection,
            Vector2.Distance(startPos, targetPosition),
            environmentHitboxLayer
            );

            if (hit.collider != null)
            {
                float adjustDistance = hit.distance * 0.9f;
                targetPosition = transform.position + knockbackDirection * adjustDistance;
            }

        }
        

        while (elapsed < knockbackDuration)
        {
            //transform.position = Vector3.Lerp(transform.position, targetPosition, elapsed / knockbackDuration);
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsed / knockbackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure it reaches the final position

        if (isPatrolActive)
        {
            Vector3 closetPoint = ClosestPointOnLine(pointA, pointB, transform.position);
            transform.position = closetPoint;
        }

    }

    private Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDir = (lineEnd - lineStart).normalized;
        float lineLength = Vector3.Distance(lineStart, lineEnd);
        float project = Mathf.Clamp(Vector3.Dot(point - lineStart, lineDir), 0f, lineLength);
        return lineStart + lineDir * project;
    }

    private IEnumerator BlinkRed()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        canDamagePlayer = false; // Temporarily disable damage

        float blinkDuration = 1.5f; // Time enemy stays invulnerable
        float blinkInterval = 0.1f; // Time between each blink

        Color originalColor = spriteRenderer.color;
        Color blinkColor = Color.red;

        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);

            elapsedTime += blinkInterval * 2;
        }

        canDamagePlayer = true; // Re-enable damage after blinking
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
        StartCoroutine(DestroyAfterDelay(1f));

        //Destroy(gameObject);
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //Destroy(gameObject);
        isDissolving = true;
    }

    protected virtual void DeathDissolve()
    {
        //dissolve effect on death
        if (isDissolving)
        {
            fade -= Time.deltaTime;
            //avoid negatives
            if (fade < 0)
            {
                fade = 0;
                isDissolving = false;
                Destroy(this.gameObject);
            }

            //update attached material
            mat.SetFloat("_Fade", fade);
        }
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




    protected virtual void OnTriggerEnter2D(Collider2D otherObject)
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
        if (isPatrolActive) // Only draw if patrol is enabled
        {
            // Draw lines between the two points for visualization in the editor
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA, pointB);
        }
    }

}
