using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonDamagingEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        canDamagePlayer = false; // Ensure this enemy does not deal damage
    }

    public override void TakeDamage(int amount, float knockbackForce, float breakDamage)
    {
        Debug.Log("This enemy is invincible!");
        // Do nothing, since this enemy cannot be damaged
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player landed on a non-damaging enemy!");
        }
    }
}
