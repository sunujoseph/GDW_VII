using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;





public class MenuOverlayManager : MonoBehaviour
{
    public static MenuOverlayManager instance;


    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private PlayerInputController playerObject;

    private bool isPaused = false;

    [SerializeField] private UnityEngine.UI.Button pauseFirstButton;
    [SerializeField] private UnityEngine.UI.Button gameOverFirstButton;

    private void Start()
    {
        // Disable menus at Start
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
        if (isPaused == true) isPaused = false;
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
            playerObject = FindAnyObjectByType<PlayerInputController>();
        }

        // Pause Menu Toggle
        if ( (Input.GetKeyDown(KeyCode.Escape) || Gamepad.current?.startButton.wasPressedThisFrame == true)  && gameOverMenu.activeSelf == false)
        {
            TogglePause();
        }

        if (isPaused)
        {
            //playerObject.isBlockActive = false;
        }
        else if (!isPaused)
        {
            //playerObject.isBlockActive = true;
        }

    }

    private void SetSelectedButton(UnityEngine.UI.Button button)
    {
        EventSystem.current.SetSelectedGameObject(null); // Clear current selection
        EventSystem.current.SetSelectedGameObject(button.gameObject); // Select new button
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused && pauseFirstButton != null)
        {
            //SetSelectedButton(pauseFirstButton);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstButton.gameObject);
        }
    }

    public void OpenGameOverMenu()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
            Time.timeScale = 0f; // Pause 

            if (gameOverFirstButton != null)
            {
                //SetSelectedButton(gameOverFirstButton);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(gameOverFirstButton.gameObject);
            }

        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        UICooldownManager uICooldownManager = FindAnyObjectByType<UICooldownManager>();
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
        if (isPaused == true) isPaused = false;

    }

}
