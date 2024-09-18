using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    [SerializeField] public int maxLives = 3; // Max Lives player starts in game            
    [SerializeField] public int currentLives; // Current amount of lives the player has


    private void Awake()
    {
        // Keep this object persistent across scenes
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);  
        }
    }

    private void Start()
    {
        // Initialize player lives at game start
        currentLives = maxLives;
    }

    // Called when player takes damage
    public void TakeDamage(int damage)
    {
        currentLives -= damage;

        if (currentLives <= 0)
        {
            GameOver();
        }
    }


    // Game over called when player dies
    private void GameOver()
    {
        Debug.Log("Game Over!");

        // Reset lives
        currentLives = maxLives;

        // Restart the game
        SceneManager.LoadScene(0);
    }

    // If the player picks up a life pickup
    public void GainLife()
    {
        currentLives++;
    }


}
