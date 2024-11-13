using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    //for when animations are added to the game
    public Animator animator;

    private PlayerMovement playerMovement;  // For ground check

    [Header("Attack Parameters")]
    public float attackCooldown = 0.5f; // Time between attacks
    //private bool isAttacking = false;  // Prevents multiple attacks during cooldown
    private float lastAttackTime = 0f;    // Tracks time of last attack
    private int comboStep = 0;            // Tracks current step in ground combo
    private bool canAttackInAir = true;   // Allows one air attack per jump
    public float bounceForce = 10f;


    [Header("Hitbox Ground Parameters")]
    public Transform origin;
    public float groundXOffset;
    public float groundYOffset;
    public float groundWidth = 1f;
    public float groundHeight = 1f;

    [Header("Hitbox Ground Parameters")]
    public float airXOffset;
    public float airYOffset;
    public float airWidth = 1f;
    public float airHeight = 2f;

    //public float xOffset;
    //public float yOffset;
    //public float angle = 0f;
    //public float width = 1f;
    //public float height = 1f;

    [Header("Hitbox Layers")]
    public LayerMask enemyLayers;
    public LayerMask hazardLayers;

    [Header("Hitstop Parameters")]
    public float hitstopDuration = 0.1f;

    [Header("Invulnerability Parameters")]
    public float invulnerabilityDuration = 0.5f; // Invulnerability after mid-air hit
    //private bool isInvulnerable = false;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.isGrounded) // Reuse the ground check from PlayerMovement
        {
            canAttackInAir = true;
            comboStep = 0;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {

        if (context.performed && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }


    private void Attack()
    {
        //play attack animation here

        //update hitbox away from the player's body
        //Vector3 hitBox = origin.position + new Vector3(xOffset, yOffset, 0);

        Vector3 hitBox;
        Vector2 hitBoxSize;


        if (playerMovement.isGrounded) // Use the shared ground check
        {
            comboStep = (comboStep + 1) % 3;
            hitBox = origin.position + new Vector3(groundXOffset, groundYOffset, 0);
            hitBoxSize = new Vector2(groundWidth, groundHeight);

            // PlayAnimation("AirAttack");
            // PlayAnimation("GroundAttack" + (comboStep + 1));

        }
        else if (canAttackInAir)
        {
            hitBox = origin.position + new Vector3(airXOffset, airYOffset, 0);
            hitBoxSize = new Vector2(airWidth, airHeight);
            
            canAttackInAir = false;

            // PlayAnimation("AirAttack");
        }
        else
        {
            return;
        }

        //look for an enemy in range of the attack (only checks object on the right layer)
        //Collider2D[] hitTargets = Physics2D.OverlapBoxAll(hitBox, new Vector2(width, height), angle, enemyLayers);
        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(hitBox, hitBoxSize, 0f, enemyLayers | hazardLayers);

        foreach (Collider2D target in hitTargets)
        {
            if (enemyLayers == (enemyLayers | (1 << target.gameObject.layer)))
            {
                Debug.Log("Hit Enemy: " + target.name);
                StartCoroutine(Hitstop()); // Trigger hitstop effect


                if (!playerMovement.isGrounded) 
                { 
                    Bounce();
                    StartCoroutine(MidAirHitInvulnerability());
                }
            }
            else if (hazardLayers == (hazardLayers | (1 << target.gameObject.layer)))
            {
                Debug.Log("Hit Hazard: " + target.name);
                StartCoroutine(Hitstop()); // Trigger hitstop effect

                if (!playerMovement.isGrounded) 
                { 
                    Bounce();
                    StartCoroutine(MidAirHitInvulnerability());
                }
            }
        }

        //debug info for hitbox
        //Debug.DrawLine(new Vector3(hitBox.x - width/2, hitBox.y - height/2, 0), new Vector3(hitBox.x + width / 2, hitBox.y - height/2, 0), Color.blue, 25f);
        //Debug.DrawLine(new Vector3(hitBox.x + width / 2, hitBox.y - height / 2, 0), new Vector3(hitBox.x + width / 2, hitBox.y + height / 2, 0), Color.blue, 25f);
        //Debug.DrawLine(new Vector3(hitBox.x + width / 2, hitBox.y + height / 2, 0), new Vector3(hitBox.x - width / 2, hitBox.y + height / 2, 0), Color.blue, 25f);
        //Debug.DrawLine(new Vector3(hitBox.x - width / 2, hitBox.y + height / 2, 0), new Vector3(hitBox.x - width / 2, hitBox.y - height / 2, 0), Color.blue, 25f);
        //Debug.Log(targets.Length);

        //apply damage to hit targets here
    }

    private void Bounce()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(rb.velocity.x, bounceForce);
        canAttackInAir = true;
        Debug.Log("BOUNCE!");
    }

 
    private void PlayAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }


    private IEnumerator Hitstop()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;  // Pause the game for a sec
        yield return new WaitForSecondsRealtime(hitstopDuration);
        Time.timeScale = originalTimeScale;  // Restore the original time scale
    }

    private IEnumerator MidAirHitInvulnerability()
    {
        int originalLayer = gameObject.layer;

        // Make player Invulnerable for hitting a bounce hit jump
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");

        yield return new WaitForSeconds(invulnerabilityDuration);

        // Restore Plyaer layer
        gameObject.layer = originalLayer;

    }


    private void OnDrawGizmosSelected()
    {
        if (origin == null) return;

        Vector3 groundHitboxPosition = origin.position + new Vector3(groundXOffset, groundYOffset, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundHitboxPosition, new Vector3(groundWidth, groundHeight, 0));

        Vector3 airHitboxPosition = origin.position + new Vector3(airXOffset, airYOffset, 0);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(airHitboxPosition, new Vector3(airWidth, airHeight, 0));
    }
}
