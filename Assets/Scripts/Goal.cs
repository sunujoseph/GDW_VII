using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private int SceneNumber;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Goal!");
           NextLevel();
        }
    }

    void NextLevel()
    {
        // Intermission Screen?
        // Level up/Upgrade Screen?
        // Level Summary menu pops up?
        SceneManager.LoadScene(SceneNumber);
        Debug.Log("LOADING NEXT LEVEL?");

        // Load next scene
        //SceneManager.LoadScene(+1);
    }
}
