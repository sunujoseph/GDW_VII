using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] private float agroRange = 8f;   // The boss room range
    [SerializeField] private Vector2 agroSize = new Vector2(10f, 5f);
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackWindUpTime = 1f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float movementSpeed = 2f;

    [Header("Boss Stats")]
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] public float bossHealth = 100f;

    [Header("Components")]
    [SerializeField] private Transform attackHitbox;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject controllerAOE;

    private Transform playerTransform;
    private bool isBlinking = false;

    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        //base.Update();

        ChasePlayer();
    }

    private void PerpareLaserAttack()
    {

    }

    private void PerpareChargeAttack()
    {

    }

    private void PerpareAOEattack()
    {

    }

    private void CheckAgro()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, agroSize, 0, playerLayer);
        //return hit != null; // Returns true if the player is inside the agro box
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

    private void HandleBossAnimations()
    {

    }

    public override void TakeDamage(int amount, float knockbackForce, float breakDamage)
    {
        //base.TakeDamage(amount, knockbackForce, breakDamage);

        if (isStunned)
        {
            // double damage
        }
        else
        {
            // normal
        }

        health -= amount;
        if (health <= 0)
        {
            Die(knockbackForce);
            SoundManager.instance.Play(deathSound, transform, 1.0f);
        }
        else
        {
            SoundManager.instance.Play(damageSound, transform, 1.0f);


            // Start blinking effect
            if (!isBlinking)
            {
                StartCoroutine(BlinkRed());
            }
            

        }
    }

    private IEnumerator BlinkRed()
    {
        isBlinking = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        canDamagePlayer = false; // Temporarily disable damage

        float blinkDuration = 1.5f; // Time enemy stays invulnerable
        float blinkInterval = 0.1f; // Time between each blink

        Color originalColor = spriteRenderer.color;
        Color blinkColor = Color.red;

        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);

            elapsedTime += blinkInterval * 2;
        }

        canDamagePlayer = true; // Re-enable damage after blinking
        isBlinking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agroRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


}
