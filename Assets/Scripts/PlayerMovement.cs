using UnityEngine;
using DG.Tweening;

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
    [SerializeField] private KeyCode slideKey = KeyCode.LeftControl;

    private bool isGrounded;
    private bool isSliding;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        CheckGrounded();
        HandleJump();
        HandleSlide();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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
        if (Input.GetKey(slideKey) && isGrounded && !isSliding)
        {
            StartSlide();
        }
        else if ((!Input.GetKey(slideKey) || !isGrounded) && isSliding)
        {
            StopSlide();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        transform.DOScaleY(originalScale.y * 0.5f, 0.1f);
    }

    private void StopSlide()
    {
        isSliding = false;
        transform.DOScaleY(originalScale.y, 0.2f);
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
