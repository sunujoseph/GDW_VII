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
    [SerializeField] private float blinkInterval = 0.2f; // Time between each blink

    private bool isPlayerInvulnerable = false;

    private UIManager uiManager;
    private SpriteRenderer playerSpriteRenderer;

    [SerializeField] private GameObject playerObject; // Reference to the player object

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

        TryReassignPlayerObject();

    }

    private void Update()
    {
        if (playerObject == null || playerSpriteRenderer == null)
        {
            TryReassignPlayerObject();
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


        float elapsedTime = 0f;

        while (elapsedTime < invulnerabilityTime)
        {
            if (playerSpriteRenderer != null)
            {
                playerSpriteRenderer.enabled = !playerSpriteRenderer.enabled; // Toggle sprite visibility
                //Debug.Log("Blinking: SpriteRenderer.enabled = " + playerSpriteRenderer.enabled);
            }
            else if (playerSpriteRenderer == null)
            {
                Debug.Log("Player" + playerObject.name);
            }
            yield return new WaitForSeconds(blinkInterval); // Wait before toggling again
            elapsedTime += blinkInterval;
        }

        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.enabled = true; // Ensure sprite is visible after blinking ends
        }

        //yield return new WaitForSeconds(invulnerabilityTime);  // Wait for the invulnerability period
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

    private void TryReassignPlayerObject()
    {
        // Find player object if it's missing
        if (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                Debug.LogWarning("Player object found.");

                playerSpriteRenderer = playerObject.GetComponent<SpriteRenderer>();
                if (playerSpriteRenderer != null)
                {
                    Debug.Log("Player and SpriteRenderer reassigned successfully.");
                }
                else
                {
                    Debug.LogError("SpriteRenderer not found.");
                }
            }
            else
            {
                Debug.LogWarning("Player object not found.");
            }
        }
        else if (playerObject != null)
        {
            playerSpriteRenderer = playerObject.GetComponent<SpriteRenderer>();
        }
    }


}
