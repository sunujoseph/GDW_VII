using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDamage : MonoBehaviour
{
    private bool canDamage = false; // Only true when laser is fully active
    public PlayerHealth playerHealth;
    [SerializeField] private int Damage = 1;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canDamage) return;

        if (other.CompareTag("Player") && other.gameObject.layer != LayerMask.NameToLayer("Invulnerable"))
        {
            Debug.Log("AOE hit player.");

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Damage); 
                Debug.Log("AOE damaged player!");
            }
        }
    }

    public void EnableDamage()
    {
        canDamage = true;
    }

    public void DisableDamage()
    {
        canDamage = false;
    }
}
