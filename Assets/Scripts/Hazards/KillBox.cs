using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{



    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the kill box
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Respawn(other.gameObject); // Pass the Player GameObject
            }
            else
            {
                Debug.LogError("PlayerHealth instance not found!");
            }
        }
    }

}
