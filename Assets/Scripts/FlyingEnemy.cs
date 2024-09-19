using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    public float flySpeed = 5f;
    public float attackDelay = 2f;
    public float agroRange = 10f;


    private Transform playerTransform;
    private bool isHostile = false;
    //private bool isSweeping = false;
    //private Rigidbody2D rb;



    protected override void Start()
    {
        base.Start();
        //rb = GetComponent<Rigidbody2D>();
        //rb.gravityScale = 0;  // Disable gravity for the Angry Sun
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Force them to stay their current position
        pointA = transform.position;
        pointB = transform.position;      


    }

    protected override void Update()
    {
        //base.Update();
        //Debug.Log(playerTransform.position);
        
    }



    private void CheckAgro()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) <= agroRange)
        {
            isHostile = true;

            // 
        }
    }

    private void SweepingAttack()
    {
        if (isHostile) {
            // Move towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);
        }
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
