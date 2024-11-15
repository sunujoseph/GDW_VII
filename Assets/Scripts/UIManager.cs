using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {

        TryReassignHealthText();


        

    }


    private void Update()
    {
        if (healthText == null)
        {
            TryReassignHealthText(); // Reassign if missing
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        healthText.text = "HEALTH: " + currentHealth.ToString();
    }


    private void TryReassignHealthText()
    {

        GameObject healthTextObj = GameObject.Find("PlayerHealthText");
        if (healthTextObj != null)
        {
            healthText = healthTextObj.GetComponent<TextMeshProUGUI>();
            if (healthText != null)
            {
                Debug.Log("healthText reassigned successfully.");
            }
            else
            {
                Debug.LogError("TextMeshProUGUI PlayerHealthText not found");
            }
        }
        else
        {
            Debug.LogWarning("PlayerHealthText GameObject not found.");
        }

        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            UpdateHealth(playerHealth.currentLives);
        }
        else
        {
            Debug.LogWarning("PlayerHealth not found.");
        }

    }

}
