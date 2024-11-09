using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputController : MonoBehaviour
{
    //components
    protected Animator animator;
    private Rigidbody2D rb;
    public static PlayerInputController instance;

    // Ground check variables
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.05f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask enemyLayer;

    // Var movement and jump
    private Vector2 moveInput;
    float horizontal;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;


    //[SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDistance = 5f; // Dash distance in units
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    bool isFacingRight = true;
    public bool isGrounded;
    private bool isDashing = false;
    private bool canDash = true;
    private bool dashResetOnEnemyHit = false;

    // For jumping
    private bool jumpPressed;
    //private float dashTimer = 0f;
    //private float cooldownTimer = 0f;

    [Header("Health/Lives")]
    [SerializeField] public int maxLives = 3; // Max Lives player starts in game            
    [SerializeField] public int currentLives; // Current amount of lives the player has
    // Invulnerability settings
    [SerializeField] private float invulnerabilityTime = 5f;
    private bool isPlayerInvulnerable = false;
    private UIManager uiManager;

    [Header("Parry")]
    [SerializeField] private float parryDuration = 0.5f;
    [SerializeField] private float parryCooldown = 1f;
    [SerializeField] private float parryProjectileSpeedMod = 10f;
    [SerializeField] private Transform parryHitbox;
    private bool isParrying = false;
    private bool canParry = true;

    [Header("Platform Parenting")]
    private int platformCount = 0;
    private Transform playerTransform;

    [Header("Animation States")]
    public bool wasHit;
    public bool isAlive;
    public Vector2 movement;
    public bool inAir;
    public bool blocking;
    public bool attacking;
    public int attackNumber;

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
        // Keep this object persistent across scenes
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set parry Hitbox not active
        parryHitbox.gameObject.SetActive(false);

        //help with parenting to a platform
        playerTransform = transform;

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        //animation states
        animator = GetComponent<Animator>();
        wasHit = false;
        isAlive = true;
        inAir = true;
        blocking = false;
        attacking = false;
        attackNumber = 0;

        // Initialize player lives at game start
        currentLives = maxLives;

        uiManager = FindObjectOfType<UIManager>();

        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentLives);
        }

        //determine initial animation
        HandleAnimations();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            ResetDash();
            //canDash = true;
        }

        // Handle jumping
        if (jumpPressed && isGrounded)
        {
            Jump();
        }

        if (isGrounded) // use the ground check 
        {
            canAttackInAir = true;
            comboStep = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)  // Disable regular movement during dash
        {
            SetPlayerVelocity();
        }
    }

    //different kinds of collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        //parry attack
        if (isParrying)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("EnemyProjectile"))
            {
                Debug.Log("Parry Successful!");

                if (other.CompareTag("EnemyProjectile"))
                {
                    ReflectProjectile(other.gameObject);  // Reflect projectile 
                }

                // Kill enemy
                //Destroy(other.gameObject);
            }
        }

        //multi dash
        if (isDashing && other.CompareTag("Enemy"))
        {
            Debug.Log("RESET DASH");
            dashResetOnEnemyHit = true;
            ResetDash();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            platformCount++;
            playerTransform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            platformCount--;

            if (platformCount <= 0)
            {
                playerTransform.parent = null;
                platformCount = 0;
            }
        }
    }

    // Called when player takes damage
    public void TakeDamage(int damage)
    {
        if (!isPlayerInvulnerable)
        {
            currentLives -= damage;

            // Update UI when health changes
            if (uiManager != null)
            {
                uiManager.UpdateHealth(currentLives);
            }

            if (currentLives <= 0)
            {
                GameOver();
            }
            else
            {
                StartCoroutine(HandleInvulnerability());
            }
        }
    }

    // Handle invulnerability period after taking damage
    private IEnumerator HandleInvulnerability()
    {
        isPlayerInvulnerable = true;  // Enable invulnerability
        yield return new WaitForSeconds(invulnerabilityTime);  // Wait for the invulnerability period
        isPlayerInvulnerable = false;  // Disable invulnerability
    }

    // Game over called when player dies
    private void GameOver()
    {
        Debug.Log("Game Over!");

        // Reset lives
        currentLives = maxLives;

        // Restart the game
        SceneManager.LoadScene(0);
    }

    // If the player picks up a life pickup
    public void GainLife()
    {
        currentLives++;

        // Update UI when health changes
        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentLives);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        horizontal = moveInput.x;

        if (horizontal > 0 && !isFacingRight)
        {
            Flip();
        }

        else if (horizontal < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void SetPlayerVelocity()
    {
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }

        if (context.canceled)
        {
            jumpPressed = false;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpPressed = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            Debug.Log("Dash Triggered");
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash Started");

        isDashing = true;
        canDash = false;
        dashResetOnEnemyHit = false;


        // Set the player to the "Invulnerable" layer for damage immunity
        // Dash will act as a dodge of sorts.
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), true);

        Vector2 dashDirection;
        if (moveInput != Vector2.zero)
        {
            dashDirection = moveInput.normalized;
        }
        else
        {
            if (isFacingRight)
            {
                dashDirection = Vector2.right;  // Dash to the right
            }
            else
            {
                dashDirection = Vector2.left;  // Dash to the left
            }
        }

        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + dashDirection * dashDistance; // Calculate dash target

        float elapsedTime = 0f;

        // Smooth the distance travel and Lerp
        while (elapsedTime < dashDuration)
        {
            rb.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.position = targetPosition; // Ensure the player reach target 

        Debug.Log("Dash Ended");
        isDashing = false;
        gameObject.layer = LayerMask.NameToLayer("Player"); // Set back to the regular layer
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), false);

        if (!dashResetOnEnemyHit)
        {
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }

        // Start cooldown before dash can be used again
        yield return new WaitForSeconds(dashCooldown);
        //canDash = true;

        Debug.Log("Dash Cooldown Complete");

    }

    private void ResetDash()
    {
        canDash = true;
        dashResetOnEnemyHit = false;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if (context.performed && canParry)
        {
            StartCoroutine(Parry());
        }
    }

    private IEnumerator Parry()
    {
        Debug.Log("Parry Activated!");

        // Enable the parry hitbox
        // Set parry active
        isParrying = true;
        canParry = false;
        parryHitbox.gameObject.SetActive(true);

        // Parry Window Duration
        yield return new WaitForSeconds(parryDuration);

        // Disable the parry hitbox
        // End Parry State
        isParrying = false;
        parryHitbox.gameObject.SetActive(false);

        // Start Parry Cooldown
        // Can parry set to true
        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }

    //used mainly for the parry
    private void ReflectProjectile(GameObject projectile)
    {
        Debug.Log("Projectile Reflected");

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Reflection Direction
            Vector2 reflectDirection = -rb.velocity.normalized;

            // Apply the new velocity to the projectile
            // Increase Projectile
            rb.velocity = reflectDirection * parryProjectileSpeedMod;
        }

        // Change the tag so it doesn't hurt the player again
        projectile.tag = "PlayerProjectile";

    }

    private void HandleAnimations()
    {
        animator.SetFloat("xSpeed", Mathf.Abs(movement.x));
        animator.SetFloat("ySpeed", Mathf.Abs(movement.y));
        animator.SetBool("inAir", inAir);
        animator.SetBool("wasHit", wasHit);
        animator.SetBool("isAlive", isAlive);
        animator.SetBool("isDashing", isDashing);
        animator.SetBool("isBlocking", blocking);
        animator.SetBool("isParry", isParrying);
        animator.SetBool("isAttacking", attacking);
        animator.SetInteger("attackNumber", attackNumber);
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


        if (isGrounded) // Use the shared ground check
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


                if (!isGrounded)
                {
                    Bounce();
                    StartCoroutine(MidAirHitInvulnerability());
                }
            }
            else if (hazardLayers == (hazardLayers | (1 << target.gameObject.layer)))
            {
                Debug.Log("Hit Hazard: " + target.name);
                StartCoroutine(Hitstop()); // Trigger hitstop effect

                if (!isGrounded)
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

    //hitbox debug display
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
