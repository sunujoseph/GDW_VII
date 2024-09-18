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

        Vector3 leftBoundary = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, mainCamera.nearClipPlane));
        leftBoundary.x += bufferDistance;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Max(clampedPosition.x, leftBoundary.x);

        transform.position = clampedPosition;
    }
}
