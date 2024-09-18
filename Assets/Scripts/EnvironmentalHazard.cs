using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnvironmentalHazard : MonoBehaviour
{
    // If player touches these things
    // The player takes damage
    // Base Script if we wanna add more types of Environmental Hazards

    [SerializeField] protected int damageAmount = 1;
    PlayerHealth playerHealth;


    protected virtual void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit by enemy!");
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
