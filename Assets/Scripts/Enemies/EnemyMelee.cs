using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [SerializeField] private FighterEnemy fighterEnemy;
    private Collider2D myCollider;

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D meleeTarget)
    {

        if (meleeTarget.CompareTag("Parry"))
        {
            GetParry();
        }

        // On player contact
        if (meleeTarget.IsTouching(myCollider))
        {
            DoDamage();
        }


    }

    void GetParry()
    {
        Debug.Log("PARRY this!");
        fighterEnemy.TakeBreakDmage(1);
    }

    void DoDamage()
    {
        Debug.Log("Player hit by Melee!");
    }
}
