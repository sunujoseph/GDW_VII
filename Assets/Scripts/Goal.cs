using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private int SceneNumber;
    [SerializeField] public PlayerHealth playerHealth;

    [SerializeField] public bool thisFix = false;
    [SerializeField] public bool isDoneGame = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();


        if (collision.CompareTag("Player"))
        {
            Debug.Log("Goal!");

            if (thisFix)
            {
                collision.gameObject.transform.position = new Vector3(-4.15f, -2.95f, 0f);
            }
   
           NextLevel();
        }

        

    }

    void NextLevel()
    {

        if (!isDoneGame)
        {
            SceneManager.LoadScene(SceneNumber);
            //playerHealth.checkpoint = FindAnyObjectByType<Checkpoint>();
            Debug.Log("LOADING NEXT LEVEL?");
        }
        else
        {
            Application.Quit();
        }

        // Intermission Screen?
        // Level up/Upgrade Screen?
        // Level Summary menu pops up?
        

        // Load next scene
        //SceneManager.LoadScene(+1);
    }
}
