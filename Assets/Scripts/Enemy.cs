using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public Vector3 pointA;   // Patrol start position
    [SerializeField] public Vector3 pointB;      // Patrol end position
    [SerializeField] public float patrolSpeed = 2f;  // Speed of patrol movement

    private Vector3 targetPosition; // Current target position for patrol

    private Rigidbody2D rb;

    PlayerHealth playerHealth;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //pointA = transform.position;
        playerHealth = FindObjectOfType<PlayerHealth>();
        targetPosition = transform.position; // Start moving towards pointB
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Patrol();
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
            
            
            
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the enemy collides with the player, deal damage
        if (collision.gameObject.CompareTag("Player"))
        {
            // Implement player damage here
            Debug.Log("Player hit by enemy!");

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

        }
    }



    private void OnDrawGizmos()
    {
        // Draw lines between the two points for visualization in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
    }

}
