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

    [Header("Dash")]
    public float dashForce = 15f;
    public float dashCooldown = 1f;
    private float lastDashTime;

    [Header("Detección de suelo")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Ataque")]
    public float attackRange = 1f;
    public float attackDamage = 5f;
    public LayerMask playerLayer;

    [Header("Controles personalizados")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode attackKey = KeyCode.L;

    [Header("Muerte (opcional)")]
    public GameObject deathEffect;        // opcional: prefab de partículas
    public float destroyDelay = 0.2f;    // tiempo antes de destruir el objeto

    private Rigidbody rb;
    private bool isGrounded;
    private bool facingRight = true;
    private float inputX;

    public float health = 100f;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Si ya murió, no procesamos entrada
        if (isDead) return;

        HandleInput();
        HandleJump();
        HandleDash();
        CheckGround();
        HandleFlip();
        HandleAttack();

        // Verificar muerte
        if (health <= 0f)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
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
        if (Input.GetKeyDown(jumpKey) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(dashKey) && Time.time - lastDashTime >= dashCooldown)
        {
            float dir = facingRight ? 1f : -1f;
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

    void HandleAttack()
    {
        if (Input.GetKeyDown(attackKey))
        {
            Vector3 attackPos = transform.position + (facingRight ? Vector3.right : Vector3.left) * 0.7f;
            Collider[] hits = Physics.OverlapSphere(attackPos, attackRange, playerLayer, QueryTriggerInteraction.Collide);

            foreach (Collider hit in hits)
            {
                if (hit.gameObject != gameObject && hit.CompareTag("Player"))
                {
                    PlayerController enemy = hit.GetComponent<PlayerController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(attackDamage);
                        Debug.Log($"{name} atacó a {enemy.name}, vida restante: {enemy.health}");
                    }
                }
            }
        }
    }

    // Método público para recibir daño (uso recomendado)
    public void TakeDamage(float amount)
    {
        if (isDead) return;
        health -= amount;
        // Aquí puedes agregar efectos de daño, UI, sonido, etc.
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{name} ha muerto.");

        // Instanciar efecto de muerte si se asignó
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Opcional: desactivar colisionador para que no siga interactuando
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Opcional: desactivar rigidbody physics para evitar movimientos después de morir
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Destruir el objeto tras un pequeño retraso para que el efecto/sonido se reproduzca
        Destroy(gameObject, destroyDelay);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        Gizmos.color = Color.yellow;
        Vector3 attackPos = transform.position + (facingRight ? Vector3.right : Vector3.left) * 0.7f;
        Gizmos.DrawWireSphere(attackPos, attackRange);
    }
}
