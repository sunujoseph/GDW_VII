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

    // Camera offset
    [SerializeField] float verticalOffset = 2f;  // Vertical offset from the player's position
    [SerializeField] float horizontalOffset = 0f;  // Horizontal offset from the player's position

    public Vector3 startingPosition;


    private void Start()
    {

        startingPosition = new Vector3(playerTransform.position.x + horizontalOffset,
                                               playerTransform.position.y + verticalOffset,
                                               0);

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

        // Get the player's position in viewport space
        Vector3 playerScreenPos = Camera.main.WorldToViewportPoint(playerTransform.position);

        // Initialize the target position to the camera's current position
        Vector3 targetPosition = transform.position;

        // Apply screen threshold logic for the horizontal axis
        /*
        if (playerScreenPos.x > screenThresholdX && playerTransform.position.x > transform.position.x)
        {
            targetPosition.x = playerTransform.position.x + horizontalOffset;
        }
        */

        // Apply screen threshold logic for the vertical axis
        if (playerScreenPos.x > screenThresholdX || playerScreenPos.x < (1 - screenThresholdX))
        {
            targetPosition.x = playerTransform.position.x + horizontalOffset;
        }

        // Apply screen threshold logic for the vertical axis
        if (playerScreenPos.y > screenThresholdY || playerScreenPos.y < (1 - screenThresholdY))
        {
            targetPosition.y = playerTransform.position.y + verticalOffset;
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

    public void ResetCamera()
    {
        // Reset camera position to player's starting position
        if (playerTransform != null)
        {
            
            transform.position = startingPosition;
        }
    }


}
