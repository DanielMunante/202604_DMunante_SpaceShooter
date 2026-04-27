using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipController : MonoBehaviour
{

    AudioSource audioSource;

    [SerializeField] float linearVelocity = 3f;
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference shoot;
    [SerializeField] GameObject prefabShot;
    [SerializeField] Transform shootingPoint;
    [SerializeField] float margenCam = 0.1f;

    //Modo Berserker
    [SerializeField] GameObject prefabLaser;
    [SerializeField] float berserkerDuration = 5f;
    [SerializeField] float berserkerShootInterval = 0.1f;
    [SerializeField] float berserkerScale = 1.3f;
    [SerializeField] Color berserkerColor = Color.red;
    [SerializeField] AudioClip berserkerActivateSound;

    Rigidbody2D rb2d;
    Camera cam; // referencia a la cámara principal para establecer limites
    Vector2 rawMove = Vector2.zero;
    Vector3 originalScale;
    SpriteRenderer sr;
    Color originalColor;
    bool isBerserkerActive = false;
    float lastAutoShootTime = 0f;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        move.action.started += OnMove;
        move.action.canceled += OnMove;
        move.action.performed += OnMove;
        shoot.action.started += OnShoot;

        audioSource = GetComponent<AudioSource>();

        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        originalScale = transform.localScale;
        if (sr != null) originalColor = sr.color;
    }

    private void OnEnable()
    {
        move.action.Enable();
        shoot.action.Enable();
    }

    private void OnDisable()
    {
        move.action.Disable();
        shoot.action.Disable();

        // Desuscribir eventos para evitar MissingReferenceException
        move.action.started -= OnMove;
        move.action.canceled -= OnMove;
        move.action.performed -= OnMove;
        shoot.action.started -= OnShoot;
    }
    private void OnDestroy()
    {
        move.action.started -= OnMove;
        move.action.canceled -= OnMove;
        move.action.performed -= OnMove;        
        shoot.action.started -= OnShoot;
    }

    private void Update()
    {
        rb2d.linearVelocity = rawMove * linearVelocity;
        ClampToScreen();//llamar al clamp cada frame

        if (isBerserkerActive && Time.time - lastAutoShootTime >= berserkerShootInterval)
        {
            ShootLaser();
            lastAutoShootTime = Time.time;
        }
    }

    // para establecer limites de camara y redimensione con otra resolucion
    void ClampToScreen()
    {
        if (cam == null) return;
        Vector3 pos = transform.position;
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
        pos.x = Mathf.Clamp(pos.x, min.x + margenCam, max.x - margenCam);
        pos.y = Mathf.Clamp(pos.y, min.y + margenCam, max.y - margenCam);
        transform.position = pos;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        rawMove = context.action.ReadValue<Vector2>();
    }
    private void OnShoot(InputAction.CallbackContext context)
    {
        if (isBerserkerActive) return;

        GameObject newShoot = Instantiate(prefabShot, shootingPoint.position, Quaternion.identity);
        audioSource.Play();
        Destroy(newShoot, 5f);
    }

    void ShootLaser()
    {
        Instantiate(prefabLaser, shootingPoint.position, Quaternion.identity);
        audioSource.Play();
    }

    public void ActivateBerserker()
    {
        if (isBerserkerActive) return;
        StartCoroutine(BerserkerCoroutine());
    }

    IEnumerator BerserkerCoroutine()
    {
        isBerserkerActive = true;

        transform.localScale = originalScale * berserkerScale;
        if (sr != null) sr.color = berserkerColor;

        if (berserkerActivateSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(berserkerActivateSound, 1f);
        }

        yield return new WaitForSeconds(berserkerDuration);

        transform.localScale = originalScale;
        if (sr != null) sr.color = originalColor;

        isBerserkerActive = false;
    }
}
