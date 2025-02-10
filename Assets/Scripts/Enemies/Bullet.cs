using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletTime = 10f; // Life Time of Bullet
    private float lifeCounter = 0f;  // Counter tracking bullet's current life time
    [SerializeField] private int bulletDamage = 1; // Damage bullet deals
    PlayerHealth playerHealth;


    // Start is called before the first frame update
    void Start()
    {
        lifeCounter = 0f;  // Initialize counter
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the lifetime counter
        lifeCounter += Time.deltaTime;

        // Check if the projectile has exceeded its lifetime
        if (lifeCounter >= bulletTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D bulletTarget)
    {

        if (bulletTarget.CompareTag("Parry"))
        {
            lifeCounter = 0f;  // reset counter
            //Debug.Log("PARRY");
        }

        // Destroy the bullet on player contact
        if (bulletTarget.CompareTag("Player") && CompareTag("EnemyProjectile"))
        {
            Debug.Log("Player hit by bullet!");

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }


            Destroy(gameObject);
        }
        else if (bulletTarget.CompareTag("Enemy") && CompareTag("PlayerProjectile"))
        {
            Debug.Log("Enemy hit by bullet!");

            Destroy(gameObject);
        }
    }
}
