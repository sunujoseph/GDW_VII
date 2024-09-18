using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Platform movement settings
    [SerializeField] Vector3 pointA; // Starting position
    [SerializeField] Vector3 pointB; // Target position
    [SerializeField] float speed = 2f; // Speed of the platform

    private Vector3 targetPosition; // Current target to move towards


    // Start is called before the first frame update
    void Start()
    {
        // Initialize the platform's target position
        targetPosition = pointB;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the platform towards the current target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // If the platform reaches the target position, swap the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Swap between pointA and pointB
            targetPosition = targetPosition == pointA ? pointB : pointA;
            /*
            if (targetPosition == pointA)
            {
                targetPosition = pointB;
            }
            else
            {
                targetPosition = pointA;
            }*/
        }
    }

    private void OnDrawGizmos()
    {
        // Draw lines between the two points for visualization in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
    }


}
