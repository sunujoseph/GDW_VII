using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private AudioClip checkpointGet;

    protected Animator animator;
    private bool activated;

   

    private void Awake()
    {
        if (playerPrefab == null)
        {
            playerPrefab = GameObject.FindWithTag("Player");
        }

        animator = GetComponent<Animator>();
        activated = false;

        spawnPlayer();

    }

    private void Update()
    {
        animator.SetBool("isActivated", activated);
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
                //checkpoint sound
                if (!activated)
                {
                    SoundManager.instance.Play(checkpointGet, transform, 1.0f);
                }

                playerHealth.SetCheckpoint(this.transform.position);
                activated = true;
                //player.SetCheckpoint(transform.position); // Update the player's checkpoint
                Debug.Log("Checkpoint Updated to: " + transform.position);
                
            }
        }
    }

}
