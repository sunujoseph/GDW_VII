using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    // Flying Enemy like the angry sun from mario

    [SerializeField] public float agroRange = 5f;                // Detection range to trigger attack
    

    private bool isHostile = false;             // Whether enemy is Hostile State
    private Vector3 lastKnownPlayerPosition;    // Track last known player position
    private Camera mainCamera;                  

    private Vector3 topRight;                   // Screen top-right position
    private Vector3 topLeft;                    // Screen top-left position
    private Vector3 parabolaStartPoint;

    private float cornerOffset = 10f;

    private Transform playerTransform;


    private float parabolaTime = 0;              // Time-based parameter for parabola calculation
    private bool movingToPlayer = false;           // Determines if we're moving from top right to left or vice versa


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
        //base.Update();  // Call the base class Update function for shared behavior

        // Check agro range if not hostile yet
        if (!isHostile)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= agroRange)
            {
                isHostile = true;  // Enter attack phase
            }
        }

        if (isHostile)
        {
            Attack();
        }
    }

    void StartAttack()
    {
        // Calculate screen top-right and top-left positions
        //Vector3 screenTopRight = new Vector3(Screen.width, Screen.height, 0);
        //Vector3 screenTopLeft = new Vector3(0, Screen.height, 0);
        Vector3 screenTopRight = new Vector3(Screen.width - cornerOffset, Screen.height - cornerOffset, 0);
        Vector3 screenTopLeft = new Vector3(cornerOffset, Screen.height - cornerOffset, 0);


        topRight = mainCamera.ScreenToWorldPoint(screenTopRight);
        topLeft = mainCamera.ScreenToWorldPoint(screenTopLeft);
        lastKnownPlayerPosition = playerTransform.position;

        // Start from top-right of the screen
        parabolaStartPoint = topRight;
        parabolaTime = 0f;  // Reset parabola time

    }


    // Main attack behavior for the flying enemy
    void Attack()
    {

        // Increment the time for smooth movement along the parabola
        parabolaTime += patrolSpeed * Time.deltaTime;

        


    }

    




    void MoveInParabola(Vector3 start, Vector3 end)
    {
        Vector3 midPoint = (start + end) / 2f;     
        midPoint.y = Mathf.Max(start.y, end.y) + 5f; 

        Vector3 m1 = Vector3.Lerp(start, midPoint, parabolaTime);
        Vector3 m2 = Vector3.Lerp(midPoint, end, parabolaTime);
        transform.position = Vector3.Lerp(m1, m2, parabolaTime);
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
            Debug.Log("Player hit by Flyer!");
        }
    }

}
