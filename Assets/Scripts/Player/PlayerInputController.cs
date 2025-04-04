using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputController : MonoBehaviour
{
    //components
    [SerializeField] Animator animator;
    [SerializeField] Animator attackAnimator;
    private Rigidbody2D rb;
    public static PlayerInputController instance;
    private float verticalLookInput = 0f;


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
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float groundedBufferTime = 0.5f;


    bool isFacingRight = true;
    public bool isGrounded;
    private bool isDashing = false;
    private bool canDash = true;
    private bool dashResetOnEnemyHit = false;
    private float coyoteTimeCounter;
    private Vector3 lastCheckpoint; // Tracks the last checkpoint position
    private float groundedBufferCounter = 0f;


    private Checkpoint checkpoint;

    // For jumping
    private bool jumpPressed;
    private bool canJump;
    //private float dashTimer = 0f;
    //private float cooldownTimer = 0f;

    [Header("Health/Lives")]
    [SerializeField] public int maxLives = 3; // Max Lives player starts in game            
    [SerializeField] public int currentLives; // Current amount of lives the player has
    // Invulnerability settings
    [SerializeField] private float invulnerabilityTime = 5f;
    private bool isPlayerInvulnerable = false;
    private float invulnerabilityTimer = 0f; // Tracks the remaining invulnerability time
    private float blinkTimer = 0f; // Tracks time for blinking
    [SerializeField] private float blinkInterval = 0.2f; // Time between each blink
    [SerializeField] private SpriteRenderer spriteRenderer; // Assign in Inspector or find dynamically


    private UIManager uiManager;
    private UICooldownManager uiCooldownManager;


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
    private int facingDirection = 1; // 1 for right, -1 for left


    [Header("Animation States")]
    public bool wasHit;
    public bool isAlive;
    public Vector2 movement;
    public bool inAir;
    public bool blocking;
    public bool reflect;
    public bool attacking;
    public bool hitBelow;
    public int attackNumber;
    public float direction;
    //states for attack projectile

    [Header("Attack Parameters")]
    public float attackCooldown = 0.5f; // Time between attacks
    //private bool isAttacking = false;  // Prevents multiple attacks during cooldown
    private float lastAttackTime = 0f;    // Tracks time of last attack
    private int comboStep = 0; // Tracks current step in ground combo
    private bool canAttackInAir = true;   // Allows one air attack per jump
    public float bounceForce = 10f;
    [SerializeField] private float knockbackForce = 1f;
    [SerializeField] private float breaknessDamage = 1f;
    [SerializeField] private Transform attackHitbox;
    [SerializeField] private Transform jumpAttackHitbox;

    [Header("Attack Combo Parameters")]
    [SerializeField] private float comboResetTime = 1f; // Time to reset combo
    private Coroutine comboResetCoroutine;

    [Header("Hitbox Layers")]
    public LayerMask enemyLayers;
    public LayerMask hazardLayers;

    [Header("Hitstop Parameters")]
    public float hitstopDuration = 0.1f;
    private bool isHitstopping = false;


    [Header("Invulnerability Parameters")]
    public float invulnerabilityDuration = 0.5f;

    //sounds
    [Header("Sound Effects")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioClip parrySound;
    [SerializeField] private AudioClip blockSound;
    [SerializeField] private AudioClip[] attackSounds;



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

        checkpoint = FindAnyObjectByType<Checkpoint>();
        this.gameObject.transform.position = checkpoint.transform.position;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        checkpoint = FindAnyObjectByType<Checkpoint>();
        this.gameObject.transform.position = checkpoint.transform.position;

        TryReassign();

        // Set parry Hitbox not active
        parryHitbox.gameObject.SetActive(false);

        //help with parenting to a platform
        playerTransform = transform;

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Set the initial position as the starting checkpoint
        // HERE 
        lastCheckpoint = transform.position;

        //animation states
        direction = 1;
        wasHit = false;
        isAlive = true;
        inAir = true;
        blocking = false;
        reflect = false;
        hitBelow = false;
        attacking = false;
        attackNumber = 0;
        canJump = true;

        // Initialize player lives at game start
        currentLives = maxLives;

        uiManager = FindObjectOfType<UIManager>();
        uiCooldownManager = FindObjectOfType<UICooldownManager>();

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
        if(uiManager == null || uiCooldownManager == null)
        {
            TryReassign();
        }
        

        // Check if the player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            canJump = true;
            if (!isDashing)
            {
                ResetDash();
            }
            coyoteTimeCounter = coyoteTime;
            canAttackInAir = true;
            comboStep = 0;
            groundedBufferCounter = groundedBufferTime;
            hitBelow = false;
            if (inAir)
            {
                attacking = false;
                SoundManager.instance.Play(landingSound, transform, 0.2f);
            }
            //canDash = true;
            uiCooldownManager?.UpdateJumpState(true);
        }
        else 
        {

            // Decrease the coyote time counter when not grounded
            groundedBufferCounter -= Time.deltaTime;
            coyoteTimeCounter -= Time.deltaTime;
            uiCooldownManager?.UpdateJumpState(coyoteTimeCounter > 0);
        }


        // Handle jumping
        if (jumpPressed && groundedBufferCounter > 0f && canJump == true)
        {
            Jump();
            canJump = false;
        }

        //change some animation variables 
        inAir = !isGrounded;
        if (moveInput.x > 0)
        {
            direction = 1f;
        }
        else if (moveInput.x < 0)
        {
            direction = -1f;
        }

        FollowPlayer followCam = FindObjectOfType<FollowPlayer>();
        if (followCam != null)
        {
            followCam.AdjustVerticalOffset(verticalLookInput);
        }


        HandleAnimations();

        //adjust hitbox to match current sprite
        //Vector2 s = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;

        //gameObject.GetComponent<BoxCollider2D>().size = s;
        //gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
    }

    private void FixedUpdate()
    {
        //Debug.Log("Time scale during Update: " + Time.timeScale);


        if (!isDashing)  // Disable regular movement during dash
        {
            SetPlayerVelocity();
        }


    }



    private void TryReassign()
    {
        if (uiManager == null)
        {
            Debug.LogWarning("Can't find uiManager");
            uiManager = FindObjectOfType<UIManager>();

            if (uiManager != null)
            {
                Debug.Log("uiManager found");
            }
            else
            {
                Debug.LogError("uiManager not found");
            }
        }

        if (uiCooldownManager == null)
        {
            Debug.LogWarning("Can't find uiManagerCooldown");
            uiCooldownManager = FindObjectOfType<UICooldownManager>();

            if (uiCooldownManager != null)
            {
                Debug.Log("uiCooldownManager found");
            }
            else
            {
                Debug.LogError("uiCooldownManager not found");
            }
        }

        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentLives);
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
                    SoundManager.instance.Play(parrySound, transform, 1f);
                    //isParrying = true;
                    ReflectProjectile(other.gameObject);  // Reflect projectile 
                    if (!isHitstopping) StartCoroutine(Hitstop());
                }

                // Kill enemy
                //Destroy(other.gameObject);
            }
        }

        //multi dash
        if (isDashing && other.CompareTag("Enemy"))
        {
            //Debug.Log("RESET DASH");
            dashResetOnEnemyHit = true;
            //ResetDash();

            //coyoteTimeCounter = coyoteTime; // Extend coyote time for floating
            if (!isHitstopping) StartCoroutine(Hitstop());
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







    




    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        horizontal = moveInput.x;

        verticalLookInput = context.ReadValue<Vector2>().y;

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
        //sound effect
        SoundManager.instance.Play(jumpSound, transform, 0.5f);

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        coyoteTimeCounter = 0f; // Reset to avoid double jump
        canJump = false;
        jumpPressed = false;
    }






    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            Debug.Log("Dash Triggered");
            StartCoroutine(Dash());
            if (uiCooldownManager != null) uiCooldownManager.StartDashCooldown(dashCooldown);

            if (!isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
        }
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash Started");

        //sound
        SoundManager.instance.Play(dashSound, transform, 1f);

        // Store original gravity to restore later
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; // Temporarily disable gravity

        isDashing = true;
        canDash = false;
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");


        // Dash Horizontal
        Vector2 dashDirection = isFacingRight ? Vector2.right : Vector2.left;
        rb.velocity = new Vector2(dashDirection.x * (dashDistance / dashDuration), 0);

        // Wait for the dash duration
        yield return new WaitForSeconds(dashDuration);

        // Restore gravity
        rb.gravityScale = originalGravity;

        

        Debug.Log("Dash Ended");
        isDashing = false;
        
        gameObject.layer = LayerMask.NameToLayer("Player");

     

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
        facingDirection = isFacingRight ? 1 : -1; // Update facing direction
        //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        transform.localScale = new Vector3(facingDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if (context.performed && canParry)
        {
            StartCoroutine(Parry());
            if (uiCooldownManager != null)
            {
                uiCooldownManager.StartParryCooldown(parryCooldown + parryDuration);
            }
            else
            {
                Debug.Log("AHHHHH");
            }
        }
    }

    private IEnumerator Parry()
    {
        Debug.Log("Parry Activated!");

        //sound to show parry is ready
        SoundManager.instance.Play(blockSound, transform, 1f);

        // Enable the parry hitbox
        // Set parry active
        isParrying = true;
        canParry = false;
        parryHitbox.gameObject.SetActive(true);

        // Player invlun so they dont take damage
        int originalLayer = gameObject.layer;

        // Make player Invulnerable during Parry
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");


        // Parry Window Duration
        yield return new WaitForSeconds(parryDuration);

        // Disable the parry hitbox
        // End Parry State
        isParrying = false;
        parryHitbox.gameObject.SetActive(false);

        // End player invlun
        // Restore Plyaer layer
        gameObject.layer = originalLayer;

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
            reflect = true;

            // Reflection Direction
            Vector2 reflectDirection = -rb.velocity.normalized;

            // Apply the new velocity to the projectile
            // Increase Projectile
            rb.velocity = reflectDirection * parryProjectileSpeedMod;
        }

        // Change the tag so it doesn't hurt the player again
        projectile.tag = "PlayerProjectile";

        //if (!isHitstopping) StartCoroutine(Hitstop());
    }

    private void HandleAnimations()
    {
        animator.SetFloat("xSpeed", Mathf.Abs(moveInput.x));
        animator.SetFloat("ySpeed", rb.velocity.y);
        animator.SetFloat("Direction", direction);
        animator.SetBool("inAir", inAir);
        animator.SetBool("wasHit", wasHit);
        animator.SetBool("isAlive", isAlive);
        animator.SetBool("isDashing", isDashing);
        animator.SetBool("isParry", isParrying);
        animator.SetBool("isAttacking", attacking);
        animator.SetBool("Reflect", reflect);
        animator.SetBool("hitBelow", hitBelow);
        //animator.SetInteger("attackNumber", attackNumber);
        attackAnimator.SetBool("onGround", !inAir);


        if (attacking)
        {
            animator.SetInteger("attackNumber", attackNumber); // Match the combo step
        }
        else
        {
            animator.SetInteger("attackNumber", 0); // Reset attack number when idle
        }

    }








    public void OnAttack(InputAction.CallbackContext context)
    {

        if (context.performed && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;

            uiCooldownManager?.StartAttackCooldown(attackNumber);

        }
    }



    private void Attack()
    {


        // Determine which hitbox to use
        Transform currentHitbox = isGrounded ? attackHitbox : jumpAttackHitbox;

        // Enable hitbox
        //currentHitbox.gameObject.SetActive(true);

        Debug.Log(attackNumber);

        // Increment combo step
        attacking = true;
        SoundManager.instance.Play(attackSounds[0], transform, 1.0f);
        attackNumber++;
        if (attackNumber > 3)
        {
            attackNumber = 1;
        }

        animator.SetInteger("attackNumber", attackNumber);
        attackAnimator.SetBool("isAttacking", attacking);

        // Reset the combo reset timer
        if (comboResetCoroutine != null) StopCoroutine(comboResetCoroutine);
        comboResetCoroutine = StartCoroutine(ResetComboAfterDelay());



        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(
            currentHitbox.position,
            currentHitbox.GetComponent<BoxCollider2D>().bounds.size,
            0f,
            enemyLayers | hazardLayers
        );

        bool hitEnemy = false;

        foreach (Collider2D target in hitTargets)
        {
            if (enemyLayers == (enemyLayers | (1 << target.gameObject.layer)))
            {
                Debug.Log("Hit Enemy: " + target.name);
                if (!isHitstopping) StartCoroutine(Hitstop());

                target.gameObject.GetComponent<Enemy>().TakeDamage(1, knockbackForce, breaknessDamage);
                hitEnemy = true;
            }
            else if (hazardLayers == (hazardLayers | (1 << target.gameObject.layer)))
            {
                Debug.Log("Hit Hazard: " + target.name);
                if (!isHitstopping) StartCoroutine(Hitstop());
                hitEnemy = true;
                SoundManager.instance.Play(attackSounds[4], transform, 1.0f);
            }
                
        }

        if (!isGrounded && hitEnemy)
        {
            Bounce();
            ResetDash();
            hitBelow = true;
            SoundManager.instance.Play(attackSounds[4], transform, 1.0f);
        }


    }

    public void StopBeam()
    {
        attackAnimator.SetBool("isAttacking", false);
    }

    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(comboResetTime);
        attackNumber = 0;
        animator.SetInteger("attackNumber", 0); // Reset animation state
        attacking = false; // Return to idle state
        attackAnimator.SetBool("isAttacking", false);
        uiCooldownManager.ResetAttackImageAfterDelay();
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
        Debug.Log("Hitstop started at time: " + Time.realtimeSinceStartup);


        if (isHitstopping) yield break; // Prevent overlapping hitstops
        isHitstopping = true;

        float originalTimeScale = Time.timeScale;

        Time.timeScale = 0f;  // Pause the game for a sec

        yield return new WaitForSecondsRealtime(hitstopDuration);
        Time.timeScale = originalTimeScale;  // Restore the original time scale
        isHitstopping = false;

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

    


    

    public void StopReflect()
    {
        reflect = false;
    }

    public void StopAttack()
    {
        hitBelow = false;
        attacking = false;
    }

    public void PlayAttackSound()
    {
        SoundManager.instance.Play(attackSounds[attackNumber], transform, 1.0f);
    }

}
