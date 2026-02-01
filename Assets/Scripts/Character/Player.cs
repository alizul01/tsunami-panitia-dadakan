using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float acceleration = 1.2f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    [SerializeField] private AudioClip[] jumpSfx;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private bool _isGrounded = false;
    private Rigidbody2D _rb;
    private AudioSource _audioSource;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        speed += acceleration * Time.deltaTime;
        transform.Translate(speed * Time.deltaTime * Vector2.right);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            if (_audioSource != null && jumpSfx != null && jumpSfx.Length > 0)
            {
                _audioSource.pitch = Random.Range(minPitch, maxPitch);
                AudioClip clip = jumpSfx[Random.Range(0, jumpSfx.Length)];
                _audioSource.PlayOneShot(clip);
            }
        }
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            FindAnyObjectByType<Spawner>().SpawnGround();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GroundZone"))
        {
            Debug.Log("Exiting Ground Zone, destroying ground in 2 seconds.");
            Destroy(collision.transform.parent != null ? collision.transform.parent.gameObject : collision.gameObject, 2f);
        } else if (collision.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("Entered Death Zone, restarting level.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}