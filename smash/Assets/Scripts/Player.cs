using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 8f;
    public float acceleration = 15f;
    public float airControl = 0.5f;

    [Header("Salto")]
    public float jumpForce = 10f;
    public int maxJumps = 2;
    private int jumpCount;

    [Header("Dash (opcional)")]
    public float dashForce = 15f;
    public float dashCooldown = 1f;
    private float lastDashTime;

    [Header("Detección de suelo")]
    public Transform groundCheck;
    public float groundRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Controles personalizados")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode dashKey = KeyCode.LeftShift;

    private Rigidbody rb;
    private bool isGrounded;
    private bool facingRight = true;
    private float inputX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleInput();
        HandleJump();
        HandleDash();
        CheckGround();
        HandleFlip();
    }

    void FixedUpdate()
    {
        Move();
    }

    void HandleInput()
    {
        inputX = 0f;

        if (Input.GetKey(leftKey)) inputX = -1f;
        if (Input.GetKey(rightKey)) inputX = 1f;
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            if (jumpCount < maxJumps)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpCount++;
            }
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(dashKey) && Time.time - lastDashTime >= dashCooldown)
        {
            float dir = inputX != 0 ? Mathf.Sign(inputX) : (facingRight ? 1f : -1f);
            rb.AddForce(new Vector3(dir * dashForce, 0, 0), ForceMode.VelocityChange);
            lastDashTime = Time.time;
        }
    }

    void Move()
    {
        float targetSpeed = inputX * moveSpeed;
        float control = isGrounded ? 1f : airControl;
        float speedX = Mathf.Lerp(rb.velocity.x, targetSpeed, acceleration * control * Time.fixedDeltaTime);
        rb.velocity = new Vector3(speedX, rb.velocity.y, 0);
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0;
        }
    }

    void HandleFlip()
    {
        if (inputX > 0) facingRight = true;
        else if (inputX < 0) facingRight = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
