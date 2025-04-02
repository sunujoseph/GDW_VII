using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    // Flying Enemy like the angry sun from mario

    [SerializeField] private float parabolaHeight = 3f; 
    [SerializeField] public float agroRange = 5f; // Detection range to trigger attack
    

    private bool isHostile = false; // Whether enemy is Hostile State
    private bool movingRightToLeft;
    private bool isHurt = false;

    private Camera mainCamera;                  

    private Vector3 topRight; // Screen top-right position
    private Vector3 topLeft;  // Screen top-left position
    private Vector3 lastKnownPlayerPosition; // Track last known player position



    private float cornerOffset = 100f;

    private Transform playerTransform;


    private float parabolaTime = 0f; // Time-based parameter for parabola calculation


    protected override void Start()
    {
        base.Start();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // Force them to stay their current position
        //pointA = transform.position;
        //pointB = transform.position;

        mainCamera = Camera.main;
    }


    protected override void Update()
    {
        //base.Update();

        // Check agro range if not hostile yet
        if (!isHostile)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= agroRange)
            {
                isHostile = true;  // Enter attack phase
                movingRightToLeft = true;

            }
        }

        if (isHostile)
        {
            Attack();
        }

        base.DeathDissolve();
    }


   


    // Main attack behavior for the flying enemy
    void Attack()
    {
        // Get top-right and top-left positions
        //Vector3 screenTopRight = new Vector3(Screen.width, Screen.height, 0);
        //Vector3 screenTopLeft = new Vector3(0, Screen.height, 0);
        Vector3 screenTopRight = new Vector3(Screen.width - cornerOffset, Screen.height - cornerOffset, 0);
        Vector3 screenTopLeft = new Vector3(cornerOffset, Screen.height - cornerOffset, 0);


        topRight = mainCamera.ScreenToWorldPoint(screenTopRight);
        topLeft = mainCamera.ScreenToWorldPoint(screenTopLeft);
        lastKnownPlayerPosition = playerTransform.position;

        topRight.z = 0;
        topLeft.z = 0;
        lastKnownPlayerPosition.z = 0;


        // Increment the time for smooth movement along the parabola
        parabolaTime += patrolSpeed * Time.deltaTime;


        if (movingRightToLeft)
        {
            /*
            if (parabolaTime == 0f)
            {
                // Set last known position at the start of the attack
                lastKnownPlayerPosition = playerTransform.position;
                lastKnownPlayerPosition.z = 0;
            }
            */

            // Parabolic movement top right to top left
            Vector3 newPos = CalculateParabolicPosition(topRight, topLeft, lastKnownPlayerPosition, parabolaTime);
            transform.position = newPos;

            if (parabolaTime >= 1f)  // When the parabola completes
            {
                parabolaTime = 0f;  // Reset the time
                movingRightToLeft = false;  // Switch direction
            }
        }
        else
        {
            // Parabolic movement top left to top right
            Vector3 newPos = CalculateParabolicPosition(topLeft, topRight, lastKnownPlayerPosition, parabolaTime);
            transform.position = newPos;

            if (parabolaTime >= 1f)
            {
                parabolaTime = 0f;
                movingRightToLeft = true;  // Switch direction back to right-to-left
            }
        }

    }

    // Calculate a parabolic path between two points and vertex height
    private Vector3 CalculateParabolicPosition(Vector3 startPoint, Vector3 endPoint, Vector3 vertex, float t)
    {
        float u = 1 - t;
        Vector3 midPoint = (startPoint + endPoint) / 2;
        midPoint.y = vertex.y + parabolaHeight;

        // Parabolic interpolation
        Vector3 parabolaPosition = (u * u * startPoint) + (2 * u * t * midPoint) + (t * t * endPoint);
        return parabolaPosition;
    }


    public override void TakeDamage(int amount, float knockbackForce, float breakDamage)
    {
        base.TakeDamage(amount, knockbackForce, breakDamage);


    }





    // Agro range in Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;  
        Gizmos.DrawWireSphere(transform.position, agroRange);  
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle damage to the player here
            Debug.Log("Player hit by Flying Enemy!");
        }


        if (other.CompareTag("Player"))
        {
            // If the player is invulnerable
            if (canDamagePlayer && other.gameObject.layer != LayerMask.NameToLayer("Invulnerable"))
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
        else if (other.CompareTag("PlayerProjectile"))
        {
            TakeDamage(1, 1f, 1f);
        }

    }

}
