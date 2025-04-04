using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    [SerializeField] public int maxLives = 3; // Max lives
    [SerializeField] public int currentLives; // Current lives

    [SerializeField] private float invulnerabilityTime = 5f;
    [SerializeField] private float blinkInterval = 0.2f; // Blink interval

    private bool isPlayerInvulnerable = false;
    private float invulnerabilityTimer;
    private float blinkTimer;

    public UIManager uiManager;
    private SpriteRenderer playerSpriteRenderer;

    public Vector3 lastCheckpoint; // Last checkpoint position
    [SerializeField] public Checkpoint checkpoint;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;


    [SerializeField] private GameObject playerObject; // Reference to player object

    private void Awake()
    {
        // Ensure singleton instance
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
        currentLives = maxLives;
        uiManager = FindObjectOfType<UIManager>();
        checkpoint = FindAnyObjectByType<Checkpoint>();
        lastCheckpoint = checkpoint.transform.position;
        uiManager?.UpdateHealth(currentLives);

        TryReassignPlayerObject();
    }

    private void Update()
    {
        HandleInvulnerability();

        // Ensure player and SpriteRenderer references are assigned
        if (playerObject == null || playerSpriteRenderer == null)
        {
            TryReassignPlayerObject();
        }

        //lastCheckpoint = checkpoint.transform.position;
    }

    public void Respawn(GameObject player)
    {
        // Reduce a life
        currentLives--;
        uiManager?.UpdateHealth(currentLives);

        // Check if the player has lives remaining
        if (currentLives > 0)
        {
            // Respawn at the last checkpoint
            if (player != null)
            {
                player.transform.position = lastCheckpoint;
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero; // Reset velocity
                }
            }
        }
        else
        {
            GameOver();
        }
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        if (newCheckpoint != Vector3.zero)
        {
            lastCheckpoint = newCheckpoint;

            Debug.Log("Checkpoint updated: " + lastCheckpoint);
        }
    }



    public void TakeDamage(int damage)
    {
        if (currentLives < 0)
        {
            return;
        }

        if (!isPlayerInvulnerable)
        {

            //sound 
            SoundManager.instance.Play(damageSound, playerObject.transform, 1f);

            currentLives -= damage;
            uiManager?.UpdateHealth(currentLives);

            if (currentLives <= 0)
            {
                GameOver();
            }
            else
            {
                StartInvulnerability();
            }
        }
    }

    private void HandleInvulnerability()
    {
        if (isPlayerInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;

            // Blink logic
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0f && playerSpriteRenderer != null)
            {
                playerSpriteRenderer.enabled = !playerSpriteRenderer.enabled; // Toggle visibility
                blinkTimer = blinkInterval; // Reset blink timer
            }

            // End invulnerability
            if (invulnerabilityTimer <= 0f)
            {
                isPlayerInvulnerable = false;
                if (playerSpriteRenderer != null)
                {
                    playerSpriteRenderer.enabled = true; // Ensure sprite is visible
                }
            }
        }
    }

    private void StartInvulnerability()
    {
        isPlayerInvulnerable = true;
        invulnerabilityTimer = invulnerabilityTime;
        blinkTimer = 0f; // Start blinking immediately
    }



    private void GameOver()
    {
        Debug.Log("Game Over!");

        SoundManager.instance.Play(deathSound, playerObject.transform, 1f);
        // Player Animation Here



        // Access the MenuOverlayManager to open the Game Over menu
        MenuOverlayManager menuOverlayManager = FindObjectOfType<MenuOverlayManager>();
        if (menuOverlayManager != null)
        {
            menuOverlayManager.OpenGameOverMenu();
        }
        else
        {
            Debug.LogError("MenuOverlayManager not found");
        }

    }



    public void GainLife()
    {
        currentLives++;
        uiManager?.UpdateHealth(currentLives);
    }

    public void ResetState()
    {
        currentLives = maxLives;
        uiManager?.UpdateHealth(currentLives);

        FollowPlayer followPlayer = FindAnyObjectByType<FollowPlayer>();

        lastCheckpoint = followPlayer.startingPosition;


        isPlayerInvulnerable = false;

        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.enabled = true; // Ensure visibility
        }

        // Move the player to the starting position
        if (playerObject != null)
        {
            playerObject.transform.position = lastCheckpoint; // Reset player position
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
