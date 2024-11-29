using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuOverlayManager : MonoBehaviour
{
    public static MenuOverlayManager instance;


    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private BlockPlayerLeft playerObject;

    private bool isPaused = false;


    private void Start()
    {
        // Disable menus at Start
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }


    private void Update()
    {

        if (playerObject == null)
        {
            playerObject = FindAnyObjectByType<BlockPlayerLeft>();
        }

        // Pause Menu Toggle
        if (Input.GetKeyDown(KeyCode.Escape) && gameOverMenu.activeSelf == false)
        {
            TogglePause();
        }

        if (isPaused)
        {
            playerObject.GetComponent<BlockPlayerLeft>().isBlockActive = false;
        }
        else if (!isPaused)
        {
            playerObject.GetComponent<BlockPlayerLeft>().isBlockActive = true;
        }

    }


    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OpenGameOverMenu()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
            Time.timeScale = 0f; // Pause 
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerHealth.ResetState();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current level

        // Find and reset the camera
        FollowPlayer followPlayer = FindObjectOfType<FollowPlayer>();
        if (followPlayer != null)
        {
            followPlayer.ResetCamera();
        }


        playerHealth.maxLives = 3;
        playerHealth.currentLives = playerHealth.maxLives;
        playerHealth.uiManager.UpdateHealth(playerHealth.currentLives);

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
        if (isPaused == true) isPaused = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }

}
