using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
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
        // Check if the player entered the kill box
        if (other.CompareTag("Player"))
        {
            PlayerInputController player = other.GetComponent<PlayerInputController>();
            if (player != null)
            {
                player.Respawn(); // Call the Respawn method on the player
            }
        }
    }

}
