using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI healthText;


    void Start()
    {
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            UpdateHealth(playerHealth.currentLives);
        }

    }

    public void UpdateHealth(int currentHealth)
    {
        healthText.text = "HEALTH: " + currentHealth.ToString();
    }

}
