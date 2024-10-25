using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{

    [SerializeField] private float parryWindow = 0.3f;  
    [SerializeField] private float parryCooldown = 1f;  
    [SerializeField] private Transform parryHitbox; 

    private bool isParrying = false; 
    private bool canParry = true;



    // Update is called once per frame
    void Update()
    {
        // press parry button
        StartCoroutine(Parry());
        
    }

    private IEnumerator Parry()
    {
        Debug.Log("Parry Activated!");

        // Enable the parry state and enlarge the hitbox
        isParrying = true;
        parryHitbox.gameObject.SetActive(true);

        yield return new WaitForSeconds(parryWindow);  // Parry window duration

        // Disable parry state and hitbox
        isParrying = false;
        parryHitbox.gameObject.SetActive(false);
        Debug.Log("Parry Window Ended!");

        // Start parry cooldown
        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
        Debug.Log("Parry Cooldown Complete!");
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if an enemy or projectile enters the parry hitbox
        if (isParrying)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("EnemyProjectile"))
            {
                Debug.Log("Parry Successful!");

                // If it's a projectile, reflect it back at the enemy
                if (other.CompareTag("EnemyProjectile"))
                {
                    ReflectProjectile(other.gameObject);
                }

                // Destroy the enemy hurtbox or projectile to prevent damage
                Destroy(other.gameObject);
            }
        }
    }

    private void ReflectProjectile(GameObject projectile)
    {
        Debug.Log("Projectile Reflected");

    }



}
