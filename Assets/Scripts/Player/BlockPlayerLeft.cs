using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlayerLeft : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float bufferDistance = 1f;


    // Update is called once per frame
    void Update()
    {
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
