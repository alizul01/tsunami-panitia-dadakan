using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Slide Settings")]
    [SerializeField] private float slideSpeed = 10f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideCooldown = 1f;
    [SerializeField] private KeyCode slideKey = KeyCode.LeftControl;

    private bool isGrounded;
    private bool isSliding;
    private float slideTimer;
    private float slideCooldownTimer;

    private void Update()
    {
        CheckGrounded();
        HandleJump();
        HandleSlide();
        UpdateSlideTimers();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        Debug.Log("Is Grounded: " + isGrounded);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleSlide()
    {
        if (Input.GetKeyDown(slideKey) && isGrounded && !isSliding && slideCooldownTimer <= 0f)
        {
            StartSlide();
        }

        if (isSliding)
        {
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * slideSpeed, rb.linearVelocity.y);
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
    }

    private void StopSlide()
    {
        isSliding = false;
        slideCooldownTimer = slideCooldown;
    }

    private void UpdateSlideTimers()
    {
        // Update slide duration timer
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                StopSlide();
            }
        }

        // Update slide cooldown timer
        if (slideCooldownTimer > 0f)
        {
            slideCooldownTimer -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
