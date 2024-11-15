using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the checkpoint
        if (other.CompareTag("Player"))
        {
            PlayerInputController player = other.GetComponent<PlayerInputController>();
            if (player != null)
            {
                //player.SetCheckpoint(transform.position); // Update the player's checkpoint
                Debug.Log("Checkpoint Updated to: " + transform.position);
            }
        }
    }

}
