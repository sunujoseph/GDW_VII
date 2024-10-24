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
        canDash = false;

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

        // Start cooldown before dash can be used again
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

        Debug.Log("Dash Cooldown Complete");

    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }


}
