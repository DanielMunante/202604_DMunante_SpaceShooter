using UnityEngine;

public class PowerUpHealth : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] float rightLimitX = 2f;
    [SerializeField] float topLimitY = 1.2f;
    [SerializeField] float bottomLimitY = -1.2f;
    [SerializeField] AudioClip collectSound;
    [SerializeField] float collectVolume = 0.8f;

    [Header("Efectos visuales")]
    [SerializeField] float rotationSpeed = 90f;
    [SerializeField] float pulseSpeed = 3f;
    [SerializeField] float pulseAmount = 0.1f;

    Rigidbody2D rb2d;
    Vector2 moveDirection;
    Vector3 originalScale;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        originalScale = transform.localScale;

        float yDir = Random.value < 0.5f ? 1f : -1f;
        moveDirection = new Vector2(0.3f, yDir).normalized;
        rb2d.linearVelocity = moveDirection * moveSpeed;
    }

    private void Update()
    {
        // Rebote vertical
        if (transform.position.y > topLimitY && moveDirection.y > 0)
        {
            moveDirection = new Vector2(moveDirection.x, -Mathf.Abs(moveDirection.y));
            rb2d.linearVelocity = moveDirection * moveSpeed;
        }
        else if (transform.position.y < bottomLimitY && moveDirection.y < 0)
        {
            moveDirection = new Vector2(moveDirection.x, Mathf.Abs(moveDirection.y));
            rb2d.linearVelocity = moveDirection * moveSpeed;
        }

        // Auto-destruir si sale por la derecha
        if (transform.position.x > rightLimitX)
        {
            Destroy(gameObject);
        }

        // Rotación continua
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Pulso de escala
        float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * scaleFactor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameSession.Instance.AddLife();

            if (collectSound != null)
            {
                GameSession.Instance.PlaySound(collectSound, collectVolume);
            }

            Destroy(gameObject);
        }
    }
}