using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletTime = 10f; // Life Time of Bullet
    private float lifeCounter = 0f;  // Counter tracking bullet's current life time


    // Start is called before the first frame update
    void Start()
    {
        lifeCounter = 0f;  // Initialize counter
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy the bullet on player contact
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by bullet!");

            Destroy(gameObject);
        }
    }
}
