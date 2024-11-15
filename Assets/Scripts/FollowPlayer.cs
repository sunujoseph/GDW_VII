using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class FollowPlayer : MonoBehaviour
{
    // Reference to the Cinemachine Virtual Camera
    private CinemachineVirtualCamera virtualCamera;

    // Reference to the player's transform
    [SerializeField] Transform playerTransform;
    [SerializeField] float followSpeed = 5f;
    [SerializeField] float screenThresholdX = 0.5f;
    [SerializeField] float screenThresholdY = 0.5f; 

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player"); // Look for the player by tag
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player object not found in the scene.");
            }
        }
    }

    void Update()
    {
        // Check if the playerTransform exists
        if (playerTransform == null)
        {
            TryReassignPlayerTransform();

            if (playerTransform == null)
            {
                Debug.LogWarning("PlayerTransform is missing or destroyed.");
                return;
            }
        }

        Vector3 playerScreenPos = Camera.main.WorldToViewportPoint(playerTransform.position);
        Vector3 targetPosition = transform.position;

        if (playerScreenPos.x > screenThresholdX && playerTransform.position.x > transform.position.x)
        {
            targetPosition.x = playerTransform.position.x;
        }

        // Update Y-axis for both upward and downward movement
        if (playerScreenPos.y > screenThresholdY || playerScreenPos.y < (1 - screenThresholdY))
        {
            targetPosition.y = playerTransform.position.y;
        }

        // Smoothly move the camera to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }


    private void TryReassignPlayerTransform()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log("PlayerTransform reassigned successfully.");
        }
        else
        {
            Debug.LogWarning("Player object not found");
        }
    }


}
