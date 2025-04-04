using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TestTools;

public class FighterEnemy : Enemy
{
    [Header("Fighter Settings")]
    [SerializeField] private Vector2 agroSize = new Vector2(5f, 3f);  // Width & Height of the agro range
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackWindUpTime = 0.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private Transform attackHitbox;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] Animator animatorFighter;

    [Header("Collider Settings")]
    [SerializeField] private EnemyMelee enemyMelee;

    [Header("Sword Settings")]
    [SerializeField] private Transform swordTransform;
    [SerializeField] private Vector3 swordIdlePosition;
    [SerializeField] private Vector3 swordRaisedPosition;
    [SerializeField] private Vector3 swordAttackPosition;

    //[SerializeField] private Quaternion swordIdleRotation;
    [SerializeField] private Quaternion swordRaisedRotation;
    //[SerializeField] private Quaternion swordAttackRotation;
    [SerializeField] private Vector3 swordAttackEuler;
    private Vector3 swordIdleEuler;


    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isCharge = false;
    private float lastAttackTime = 0f;
    private string originalTag;
    private int originalLayer;


    protected override void Start()
    {
        base.Start();

        if (swordTransform != null)
        {
            swordIdlePosition = swordTransform.localPosition;
            swordIdleEuler = swordTransform.localEulerAngles;
        }

        HandleFighterAnimations();
        originalTag = gameObject.tag;
        originalLayer = gameObject.layer;

    }

    protected override void Update()
    {
        base.Update();

        if (isStunned) return; // Don't do anything while stunned

        bool playerInAgro = CheckAgroRange();

        if (playerInAgro && !isChasing)
        {
            isChasing = true;
            isPatrolActive = false; // Stop patrolling
        }
        else if (!playerInAgro)
        {
            isChasing = false;
            isPatrolActive = true; // Resume patrolling
        }

        if (isChasing && !isAttacking)
        {
            ChasePlayer();
        }


        if (isChasing && !isAttacking && Vector2.Distance(transform.position, playerFindTransform.position) <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformAttack());     

        }

        ApplyManualGravity();

        //ActivateTrigger();
    }

    private bool CheckAgroRange()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, agroSize, 0, playerLayer);
        return hit != null; // Returns true if the player is inside the agro box
    }

    private void ChasePlayer()
    {
        if (playerFindTransform == null) return;

        // Stop at platform edges
        //if (IsAtEdge()) return;

        float playerDistance = Vector2.Distance(transform.position, playerFindTransform.position);

        // Move towards player and keep within range
        if (playerDistance > attackRange)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.MoveTowards(transform.position.x, playerFindTransform.position.x, patrolSpeed * Time.deltaTime);
            transform.position = newPosition;
        }

    }


    private void ChargePlayer()
    {
        if (playerFindTransform == null) return;


            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.MoveTowards(transform.position.x, playerFindTransform.position.x, patrolSpeed * Time.deltaTime);
            transform.position = newPosition;
    }

    private bool IsAtEdge()
    {
        float checkDistance = 0.6f; // Adjust based on tile size

        Vector2 groundCheckPosition = transform.position + new Vector3(isFacingRight ? checkDistance : -checkDistance, -1f, 0);
        RaycastHit2D groundCheck = Physics2D.Raycast(groundCheckPosition, Vector2.down, 1f, groundLayer);

        return groundCheck.collider == null; // If no ground, return true
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Disable attack hitbox at start
        attackHitbox.gameObject.SetActive(false);

        // STEP 1: Raise sword
        if (swordTransform != null)
        {
            swordTransform.localPosition = swordRaisedPosition;
            swordTransform.localRotation = swordRaisedRotation;
        }

        Debug.Log("Hitbox Off");

        animatorFighter.SetFloat("attackTime", attackWindUpTime);

        // Attack wind-up delay (preparing attack, hitbox inactive)
        yield return new WaitForSeconds(attackWindUpTime);

        animatorFighter.SetBool("isAttacking", isAttacking);

        // STEP 2: Attack - Slash sword
        if (swordTransform != null)
        {
            swordTransform.localPosition = swordAttackPosition;
            swordTransform.localEulerAngles = swordAttackEuler;
            
        }

        //this.gameObject.GetComponent<BoxCollider2D>().enabled = false;

        // Enable attack hitbox
        attackHitbox.gameObject.SetActive(true);
        gameObject.layer = LayerMask.NameToLayer("Hazard");
        gameObject.tag = "Hazard";

        Debug.Log("Hitbox ON");

        // HERE
        // move hitbox slightly back and forth
        Vector3 originalPosition = attackHitbox.localPosition;
        float moveAmount = 0.05f; // Tiny nudge
        float moveSpeed = 20f;    // Fast oscillation
        float elapsed = 0f;
        float duration = 0.2f;    // Time to do this "jiggle"

        while (elapsed < duration)
        {
            float offset = Mathf.Sin(Time.time * moveSpeed) * moveAmount;
            attackHitbox.localPosition = originalPosition + new Vector3(offset, 0, 0);
            elapsed += Time.deltaTime;
            ChargePlayer();
            yield return null;
        }

        //this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        // Reset position
        attackHitbox.localPosition = originalPosition;


        // Hitbox stays active for a short time
        yield return new WaitForSeconds(2f);

        // Disable attack hitbox after attack is completed
        attackHitbox.gameObject.SetActive(false);
        isCharge = false;
        gameObject.layer = LayerMask.NameToLayer("Enemies");
        gameObject.tag = "Enemy";

        if (swordTransform != null)
        {
            swordTransform.localPosition = swordIdlePosition;
            swordTransform.localEulerAngles = swordIdleEuler;
        }

        Debug.Log("Hitbox COOLDOWN");

        // Properly apply attack cooldown AFTER the attack sequence finishes
        lastAttackTime = Time.time;

        // Attack is now on cooldown
        isAttacking = false;
        animatorFighter.SetBool("isAttacking", isAttacking);
    }

    public override void TakeBreakDmage(float breakDamage)
    {
        toughness -= breakDamage;
        if (toughness <= 0)
        {
            StartCoroutine(StunRoutine());
        }
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;

        // Toughness bar depleted
        // Stun effect on top of enemmy plays


        yield return new WaitForSeconds(2f);
        toughness = 3f; // Reset toughness after stun
        isStunned = false;
    }

    private void ApplyManualGravity()
    {
        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        if (groundCheck.collider == null)
        {
            // If not on the ground, move down
            transform.position += Vector3.down * 5f * Time.deltaTime; // Simulated gravity
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, agroSize); // Draw a box for agro range

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (isPatrolActive)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA, pointB);
        }
    }

    
   private void HandleFighterAnimations()
    {
        animatorFighter.SetFloat("xSpeed", Mathf.Abs(patrolSpeed));
        animatorFighter.SetFloat("attackTime", attackWindUpTime);
        animatorFighter.SetBool("isAttacking", isAttacking);
    }
    


}
