using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //for when animations are added to the game
    public Animator animator;

    [Header("Hitbox Parameters")]
    public Transform origin;
    public float xOffset;
    public float yOffset;
    public float angle = 0f;
    public float width = 1f;
    public float height = 1f;

    public LayerMask enemyLayers;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    void Attack()
    {
        //play attack animation here

        //update hitbox away from the player's body
        Vector3 hitBox = origin.position + new Vector3(xOffset, yOffset, 0);

        //look for an enemy in range of the attack (only checks object on the right layer)
        Collider2D[] targets = Physics2D.OverlapBoxAll(hitBox, new Vector2(width, height), angle, enemyLayers);

        //debug info for hitbox
        Debug.DrawLine(new Vector3(hitBox.x - width/2, hitBox.y - height/2, 0), new Vector3(hitBox.x + width / 2, hitBox.y - height/2, 0), Color.blue, 25f);
        Debug.DrawLine(new Vector3(hitBox.x + width / 2, hitBox.y - height / 2, 0), new Vector3(hitBox.x + width / 2, hitBox.y + height / 2, 0), Color.blue, 25f);
        Debug.DrawLine(new Vector3(hitBox.x + width / 2, hitBox.y + height / 2, 0), new Vector3(hitBox.x - width / 2, hitBox.y + height / 2, 0), Color.blue, 25f);
        Debug.DrawLine(new Vector3(hitBox.x - width / 2, hitBox.y + height / 2, 0), new Vector3(hitBox.x - width / 2, hitBox.y - height / 2, 0), Color.blue, 25f);
        Debug.Log(targets.Length);

        //apply damage to hit targets here
    }
}
