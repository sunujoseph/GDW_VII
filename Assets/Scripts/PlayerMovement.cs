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
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    bool isFacingRight = true;

    // Check if the player is grounded
    private bool isGrounded;

    private bool isDashing = false;
    private bool canDash = true;

    // Ground check variables
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.05f;
    [SerializeField] LayerMask groundLayer;

    // For jumping
    private bool jumpPressed;

    private float dashTimer = 0f;
    private float cooldownTimer = 0f;


    private void Awake()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        // Check if the player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            canDash = true;
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
        canDash = false;  // Prevent repeated dashes

        float originalGravity = rb.gravityScale;  // Save gravity scale
        rb.gravityScale = 0f;  // Disable gravity during dash

        // Determine dash direction: use move input or default to right
        Vector2 dashDirection = moveInput != Vector2.zero ? moveInput.normalized : (isFacingRight ? Vector2.right : Vector2.left);
        rb.velocity = dashDirection * dashSpeed;  // Apply dash velocity

        yield return new WaitForSeconds(dashDuration);  // Wait for dash to complete

        rb.gravityScale = originalGravity;  // Restore gravity
        isDashing = false;  // End dash state

        Debug.Log("Dash Ended");

        // Start cooldown for next dash
        yield return new WaitForSeconds(dashCooldown);
        Debug.Log("Dash Cooldown Complete");


        //  dash on ground or mid air
        if (isGrounded) 
        {
            canDash = true;
        }
            
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }


}
