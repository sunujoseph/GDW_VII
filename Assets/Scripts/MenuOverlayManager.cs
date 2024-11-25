using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuOverlayManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;

    private bool isPaused = false;


    private void Start()
    {
        // Disable menus at Start
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
    }


    private void Update()
    {

        // Pause Menu Toggle
        if (Input.GetKeyDown(KeyCode.Escape) && gameOverMenu.activeSelf == false)
        {
            TogglePause();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current level
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        //SceneManager.LoadScene("MainMenu"); 
    }

}
