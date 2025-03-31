using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D meleeTarget)
    {

        if (meleeTarget.CompareTag("Parry"))
        {
            Debug.Log("PARRY");
        }

        // On player contact
        if (meleeTarget.CompareTag("Player"))
        {
            Debug.Log("Player hit by Melee!");           

        }
    }


}
