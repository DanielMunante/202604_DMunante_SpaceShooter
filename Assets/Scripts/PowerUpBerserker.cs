using UnityEngine;

public class PowerUpBerserker : MonoBehaviour
{
    [SerializeField] float lifeTime = 3f;
    [SerializeField] AudioClip collectSound;
    [SerializeField] float collectVolume = 1f;

    [Header("Efectos visuales")]
    [SerializeField] float rotationSpeed = 180f;
    [SerializeField] float pulseSpeed = 4f;
    [SerializeField] float pulseAmount = 0.15f;

    Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * scaleFactor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpaceshipController spaceship = other.GetComponent<SpaceshipController>();
            if (spaceship != null)
            {
                spaceship.ActivateBerserker();
            }

            if (collectSound != null)
            {
                GameSession.Instance.PlaySound(collectSound, collectVolume);
            }

            Destroy(gameObject);
        }
    }
}