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
    [SerializeField]  Transform playerTransform;
    [SerializeField] float followSpeed = 5f;
    [SerializeField] float screenThresholdX = 0.5f;
    [SerializeField] float screenThresholdY = 0.5f; // Threshold for Y-axis follow




    // Update is called once per frame
    void Update()
    {
        Vector3 playerScreenPos = Camera.main.WorldToViewportPoint(playerTransform.position);
        Vector3 targetPosition = transform.position;


        if (playerScreenPos.x > screenThresholdX && playerTransform.position.x > transform.position.x)
        {

            targetPosition.x = playerTransform.position.x;

            //targetPosition = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);
            //transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        // Update Y-axis for both upward and downward movement
        if (playerScreenPos.y > screenThresholdY || playerScreenPos.y < (1 - screenThresholdY))
        {
            targetPosition.y = playerTransform.position.y;
        }

        // Smoothly move the camera to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);



    }
}
