using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public Vector3 pointA;   // Patrol start position
    [SerializeField] public Vector3 pointB;      // Patrol end position
    [SerializeField] public float patrolSpeed = 2f;  // Speed of patrol movement

    [SerializeField] protected int health = 1;
    [SerializeField] public bool isPatrolActive = true;

    private Vector3 targetPosition; // Current target position for patrol

    private Rigidbody2D rb;
    private bool isFacingRight = true;

    PlayerHealth playerHealth;
    private Transform playerFindTransform; // Reference to the player's transform


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
    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    // Call when Enemy dies
    protected void Die()
    {
        Destroy(gameObject);
    }

  

    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        if (otherObject.CompareTag("Player"))
        {
            // If the player is invulnerable
            if (otherObject.gameObject.layer != LayerMask.NameToLayer("Invulnerable"))
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
            TakeDamage(1);
        }
    }



    private void OnDrawGizmos()
    {
        // Draw lines between the two points for visualization in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
    }

}
