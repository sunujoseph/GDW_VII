using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlayerLeft : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float bufferDistance = 1f;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("MainCamera is not assigned or found");
            }
        }

    }


    // Update is called once per frame
    void Update()
    {

        // Check if the camera exists
        if (mainCamera == null)
        {
            Debug.LogWarning("MainCamera reference is missing. Attempting to reassign.");
            mainCamera = Camera.main; 
            if (mainCamera == null) return; 
        }


        // Set buffer distance from the left wall of the camera
        // Prevents player from walking backwards
        // Like a classic mario game
        Vector3 leftBoundary = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, mainCamera.nearClipPlane));
        leftBoundary.x += bufferDistance;

        // Clamp the position
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Max(clampedPosition.x, leftBoundary.x);

        transform.position = clampedPosition;
    }
}
