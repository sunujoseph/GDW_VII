using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] private float moveRange = 1f;   // MOVEMENT RANGE
    [SerializeField] private Vector2 agroSize = new Vector2(10f, 5f);
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackWindUpTime = 1f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float movementSpeed = 2f;

    [Header("Laser Stats")]
    [SerializeField] private float laserWarningDuration = 1f;
    [SerializeField] private Color laserWarningColor = new Color(1f, 0f, 0f, 0.3f); // Transparent red
    [SerializeField] private Color laserActiveColor = new Color(1f, 0f, 0f, 1f); // Fully visible red

    [Header("Charge Attack Stats")]
    [SerializeField] private float chargePrepareTime = 1f;
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 0.5f;

    

    [Header("Components")]
    [SerializeField] private Transform attackHitbox;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject controllerAOE;
    [SerializeField] private BossAOE bossAOEScript;

    private Transform playerTransform;
    private bool isBlinking2 = false;
    private bool isAttacking = false;
    private Vector3 originalPosition;
    private bool isExecutingAttackPattern = false;

    //unique anims
    private bool charging;


    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        bossAOEScript = controllerAOE.GetComponent<BossAOE>();

        charging = false;
    }

    protected override void Update()
    {
        //base.Update();

        //ChasePlayer();

        float playerDistance = Vector2.Distance(transform.position, playerFindTransform.position);

        if (!isExecutingAttackPattern && playerDistance <= attackRange)
        {
            StartCoroutine(AttackPattern());
            isExecutingAttackPattern = true;
        }

        if (!isAttacking)
        {
            ChasePlayer();
        }

        HandleAnimation();
        base.DeathDissolve();

    }

    protected override void HandleAnimation()
    {
        base.HandleAnimation();
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isCharging", charging);
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

    private IEnumerator PerformLaserAttack()
    {
        isAttacking = true;

        if (attackHitbox != null)
        {
            SpriteRenderer laserSprite = attackHitbox.GetComponent<SpriteRenderer>();
            Collider2D laserCollider = attackHitbox.GetComponent<Collider2D>();

            // Step 1: Show faded laser preview
            attackHitbox.gameObject.SetActive(true);
            laserSprite.color = laserWarningColor;
            laserCollider.enabled = false;

            yield return new WaitForSeconds(laserWarningDuration);

            // Step 2: Activate laser fully
            laserSprite.color = laserActiveColor;
            laserCollider.enabled = true;

            yield return new WaitForSeconds(1f); // Laser active duration

            // Step 3: Disable laser
            laserCollider.enabled = false;
            attackHitbox.gameObject.SetActive(false);
        }

        isAttacking = false;
    }

    private IEnumerator PerformChargeAttack()
    {
        isAttacking = true;

        // Step 1: Show a circular animation or warning (placeholder here)
        originalPosition = transform.position;

        float elapsed = 0f;
        float radius = 0.5f; // You can tweak this for a wider/smaller circle
        float speed = 2f; // Speed of the circle rotation

        // Step 1: Circle motion (for charge prepare)
        while (elapsed < chargePrepareTime)
        {
            float angle = elapsed * speed * Mathf.PI * 2f; // Full circle over time
            float offsetX = Mathf.Cos(angle) * radius;
            float offsetY = Mathf.Sin(angle) * radius;

            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset position after circle motion
        transform.position = originalPosition;



        Debug.Log("Charging...");

        //yield return new WaitForSeconds(chargePrepareTime);

        // Step 2: Charge
        float timer = 0f;
        Vector2 direction = (playerFindTransform.position.x > transform.position.x) ? Vector2.right : Vector2.left;

        while (timer < chargeDuration)
        {
            transform.Translate(direction * chargeSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Step 3: Return to original position or stop
        isAttacking = false;
        charging = false;
    }

    private IEnumerator AttackPattern()
    {
        isExecutingAttackPattern = true;

        int rand = Random.Range(0, 3); // 0 = laser, 1 = charge

        //rand = 2;

        if (rand == 0)
        {
            yield return StartCoroutine(PerformLaserAttack());
        }  
        else if (rand == 1)
        {
            charging = true;
            yield return StartCoroutine(PerformChargeAttack());
        }  
        else if (rand == 2)
        {
            charging = true;
            bossAOEScript.TriggerAOEAttack();
        }
            
        yield return new WaitForSeconds(attackCooldown);

        isExecutingAttackPattern = false;
    }

    private void FacePlayer()
    {
        if (playerFindTransform == null) return;

        // Check if the player is to the left or right of the enemy
        if ((playerFindTransform.position.x < transform.position.x && isFacingRight) ||
            (playerFindTransform.position.x > transform.position.x && !isFacingRight))
        {
            Flip();
        }
    }


    private void ChasePlayer()
    {
        if (playerFindTransform == null) return;

        FacePlayer();
        // Stop at platform edges
        //if (IsAtEdge()) return;

        float playerDistance = Vector2.Distance(transform.position, playerFindTransform.position);

        // Move towards player and keep within range
        if (playerDistance > moveRange)
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
            if (!isBlinking2)
            {
                StartCoroutine(BlinkRed());
            }
            

        }
    }

    private IEnumerator BlinkRed()
    {
        isBlinking2 = true;

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
        isBlinking2 = false;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight; // Toggle facing direction
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Invert the x scale to flip the sprite
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, moveRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


}
