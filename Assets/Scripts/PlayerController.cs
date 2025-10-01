using UnityEngine;

/// <summary>
/// Handles the player's forward movement, jumping, and energy/ultimate logic.
/// Attach this to the runner character.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Energy System")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyPerJump = 10f;
    [SerializeField] private KeyCode ultimateKey = KeyCode.Space;
    [SerializeField] private int ultimateMouseButton = 1;

    [Header("Audio")]
    [SerializeField] private AudioSource jumpAudioSource;

    private Rigidbody2D rb;
    private GameManager gameManager;
    private float currentEnergy;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    gameManager = FindFirstObjectByType<GameManager>();
        currentEnergy = 0f;
    }

    private void Start()
    {
        UpdateGroundedState();
        gameManager?.UpdateEnergy(currentEnergy);
    }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        HandleInput();
    }

    private void FixedUpdate()
    {
        if (gameManager != null && gameManager.IsGameOver)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        MaintainForwardVelocity();
        UpdateGroundedState();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryJump();
        }

    bool ultimateRequested = Input.GetKeyDown(ultimateKey) || Input.GetMouseButtonDown(ultimateMouseButton);
    if (ultimateRequested && currentEnergy >= maxEnergy)
        {
            ActivateUltimate();
        }
    }

    private void MaintainForwardVelocity()
    {
    rb.linearVelocity = new Vector2(runSpeed, rb.linearVelocity.y);
    }

    private void UpdateGroundedState()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);
        }
        else
        {
            isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.05f;
        }
    }

    private void TryJump()
    {
        if (!isGrounded) return;

    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (jumpAudioSource != null)
        {
            jumpAudioSource.Play();
        }

        AddEnergy(energyPerJump);
    }

    private void ActivateUltimate()
    {
        if (gameManager == null) return;

        gameManager.TriggerUltimate(transform.position);
        ResetEnergy();
    }

    private void AddEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
        gameManager?.UpdateEnergy(currentEnergy);
    }

    public float GetEnergy() => currentEnergy;

    public void ResetEnergy()
    {
        currentEnergy = 0f;
        gameManager?.UpdateEnergy(currentEnergy);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameManager?.GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            gameManager?.AddScore(10);
            Destroy(other.gameObject);
        }
    }
}
