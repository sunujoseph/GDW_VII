using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Reference to the Rigidbody2D component
    private Rigidbody2D rb;

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


    // Ground check variables
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.05f;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] LayerMask enemyLayer;

    // For jumping
    private bool jumpPressed;
    //private float dashTimer = 0f;
    //private float cooldownTimer = 0f;


    private void Awake()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
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

    }

    private void FixedUpdate()
    {
        if (!isDashing)  // Disable regular movement during dash
        {
            SetPlayerVelocity();
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

    private void OnTriggerEnter2D(Collider2D collisionEnemy)
    {
        if (isDashing && collisionEnemy.CompareTag("Enemy"))
        {
            Debug.Log("RESET DASH");
            dashResetOnEnemyHit = true;
            ResetDash();
        }
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


}
