using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    public float sweepSpeed = 5f;
    public float sweepDelay = 2f;
    public float agroRange = 10f;


    private Transform playerTransform;
    private bool isHostile = false;
    private bool isSweeping = false;
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

    

    private void CheckAgro()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) <= agroRange)
        {
            isHostile = true;
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
