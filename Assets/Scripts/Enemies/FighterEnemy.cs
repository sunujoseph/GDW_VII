using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool isChasing = false;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;

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

        if (isChasing && Vector2.Distance(transform.position, playerFindTransform.position) <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformAttack());
        }

        ApplyManualGravity();
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
        if (IsAtEdge()) return;

        float playerDistance = Vector2.Distance(transform.position, playerFindTransform.position);

        // Move towards player and keep within range
        if (playerDistance > attackRange)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.MoveTowards(transform.position.x, playerFindTransform.position.x, patrolSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
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

        // Attack wind-up delay (preparing attack, hitbox inactive)
        yield return new WaitForSeconds(attackWindUpTime);

        // Enable attack hitbox
        attackHitbox.gameObject.SetActive(true);

        if (attackHitbox.gameObject.GetComponent<Collider2D>().CompareTag("Parry"))
        {
            // toughness dmg
            // end attack
        }
        else if (attackHitbox.gameObject.GetComponent<Collider2D>().CompareTag("Player"))
        {
            // Damage Player
            // apply knockback to player
        }

        // Hitbox stays active for a short time
        yield return new WaitForSeconds(1f);

        // Disable attack hitbox after attack is completed
        attackHitbox.gameObject.SetActive(false);

        // Properly apply attack cooldown AFTER the attack sequence finishes
        lastAttackTime = Time.time;

        // Attack is now on cooldown
        isAttacking = false;
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
}
