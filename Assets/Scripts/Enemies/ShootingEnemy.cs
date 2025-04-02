using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : Enemy
{

    [SerializeField] public GameObject projectilePrefab;  // The projectile to shoot
    [SerializeField] public float agroRange = 5f; // Detection range of enemy before they become hostile
    [SerializeField] public float fireRate = 5f; // Fire rate of gun
    
    
    private Transform playerTransform;
    private bool isHostile = false;
    private float timeSinceLastShot = 0f;  // Tracks time since last shot

    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Force them to stay their current position
        pointA = transform.position;
        pointB = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        if (!isHostile)
        {
            CheckAgro();
        }

        if (isHostile & isAlive)
        {
            // Only shoot if enough time has passed since the last shot
            timeSinceLastShot += Time.deltaTime;
            if (timeSinceLastShot >= fireRate)
            {
                ShootAtPlayer();
                timeSinceLastShot = 0f;  // Reset timer after shooting
            }

            CheckAgro();
        }
    }

    private void CheckAgro()
    {
        // Check if the player is within the agro range
        if (Vector2.Distance(transform.position, playerTransform.position) <= agroRange)
        {
            // Once hostile, it stays hostile
            // Possibly change this if we wanna tweak enemy behaviour
            isHostile = true;  
        }
        else
        {
            isHostile = false;
        }
    }

    private void ShootAtPlayer()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().velocity = direction * 5f;
            SoundManager.instance.Play(attackSound, transform, 1.0f);
        }
    }

    // Agro Range in Editior
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agroRange);  
    }


}
