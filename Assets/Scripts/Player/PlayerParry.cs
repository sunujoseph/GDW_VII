using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{

    [SerializeField] private float parryDuration = 0.5f;  
    [SerializeField] private float parryCooldown = 1f;  
    [SerializeField] private float parryProjectileSpeedMod = 10f;  
    [SerializeField] private Transform parryHitbox; 

    private bool isParrying = false; 
    private bool canParry = true;


    private void Start()
    {
        // Set parry Hitbox not active
        parryHitbox.gameObject.SetActive(false);
    }


    public void OnParry(InputAction.CallbackContext context)
    {
        if (context.performed && canParry)
        {
            StartCoroutine(Parry());
        }
    }

    private IEnumerator Parry()
    {
        Debug.Log("Parry Activated!");

        // Enable the parry hitbox
        // Set parry active
        isParrying = true;
        canParry = false;
        parryHitbox.gameObject.SetActive(true);

        // Parry Window Duration
        yield return new WaitForSeconds(parryDuration);  

        // Disable the parry hitbox
        // End Parry State
        isParrying = false;
        parryHitbox.gameObject.SetActive(false);

        // Start Parry Cooldown
        // Can parry set to true
        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("EnemyProjectile"))
            {
                Debug.Log("Parry Successful!");

                if (other.CompareTag("EnemyProjectile"))
                {
                    ReflectProjectile(other.gameObject);  // Reflect projectile 
                }

                // Kill enemy
                //Destroy(other.gameObject);
            }
        }
    }

    private void ReflectProjectile(GameObject projectile)
    {
        Debug.Log("Projectile Reflected");

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Reflection Direction
            Vector2 reflectDirection = -rb.velocity.normalized;

            // Apply the new velocity to the projectile
            // Increase Projectile
            rb.velocity = reflectDirection * parryProjectileSpeedMod;  
        }

        // Change the tag so it doesn't hurt the player again
        projectile.tag = "PlayerProjectile";

    }

    
}
