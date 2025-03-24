using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

   

    private void Awake()
    {
        if (playerPrefab == null)
        {
            playerPrefab = GameObject.FindWithTag("Player");
        }

        spawnPlayer();

    }

    private void spawnPlayer()
    {
        playerPrefab.transform.position = this.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        // Check if the player entered the checkpoint
        if (other.CompareTag("Player"))
        {
            PlayerInputController player = other.GetComponent<PlayerInputController>();
            if (player != null)
            {
                playerHealth.SetCheckpoint(this.transform.position);
                //player.SetCheckpoint(transform.position); // Update the player's checkpoint
                Debug.Log("Checkpoint Updated to: " + transform.position);
                
            }
        }
    }

}
