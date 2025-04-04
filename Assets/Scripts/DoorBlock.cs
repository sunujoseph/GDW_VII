using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBlock : MonoBehaviour
{
    [SerializeField] public GameObject objectToWatch;



    // Update is called once per frame
    void Update()
    {
        if (objectToWatch == null)
        {
            Destroy(gameObject); // Destroy this block
        }
    }
}
