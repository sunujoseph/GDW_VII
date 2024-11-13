using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    [SerializeField] public int maxLives = 3; // Max Lives player starts in game            
    [SerializeField] public int currentLives; // Current amount of lives the player has

    // Invulnerability settings
    [SerializeField] private float invulnerabilityTime = 5f;
    private bool isPlayerInvulnerable = false;

    private UIManager uiManager;

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

        uiManager = FindObjectOfType<UIManager>();

        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentLives);
        }
    }

    // Called when player takes damage
    public void TakeDamage(int damage)
    {
        if (!isPlayerInvulnerable)
        {
            currentLives -= damage;

            // Update UI when health changes
            if (uiManager != null)
            {
                uiManager.UpdateHealth(currentLives);
            }

            if (currentLives <= 0)
            {
                GameOver();
            }
            else
            {
                StartCoroutine(HandleInvulnerability());
            }
        }
    }

    // Handle invulnerability period after taking damage
    private IEnumerator HandleInvulnerability()
    {
        isPlayerInvulnerable = true;  // Enable invulnerability
        yield return new WaitForSeconds(invulnerabilityTime);  // Wait for the invulnerability period
        isPlayerInvulnerable = false;  // Disable invulnerability
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

        // Update UI when health changes
        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentLives);
        }
    }


}
