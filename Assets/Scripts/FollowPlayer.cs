using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowPlayer : MonoBehaviour
{
    // Reference to the Cinemachine Virtual Camera
    private CinemachineVirtualCamera virtualCamera;

    // Reference to the player's transform
    [SerializeField]  Transform playerTransform;
    [SerializeField] float followSpeed = 5f;
    [SerializeField] float screenThreshold = 0.5f;



    // Update is called once per frame
    void Update()
    {
        Vector3 playerScreenPos = Camera.main.WorldToViewportPoint(playerTransform.position);
        if(playerScreenPos.x > screenThreshold && playerTransform.position.x > transform.position.x)
        {
            Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        
    }
}
