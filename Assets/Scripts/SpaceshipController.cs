using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipController : MonoBehaviour
{

    [SerializeField] float linearVelocity = 3f;
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference shoot;
    [SerializeField] GameObject prefabShot;
    [SerializeField] Transform shootingPoint;
    [SerializeField] float margenCam = 0.1f;
    Rigidbody2D rb2d;
    Camera cam;// referencia a la cámara principal para establecer limites


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        move.action.started += OnMove;
        move.action.canceled += OnMove;
        move.action.performed += OnMove;
        shoot.action.started += OnShoot;
    }       

    private void OnEnable()
    {
        move.action.Enable();
        shoot.action.Enable();
    }

    private void Update()
    {
        rb2d.linearVelocity = rawMove * linearVelocity;
        ClampToScreen();//llamar al clamp cada frame
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


    private void OnDisable()
    {
        move.action.Disable(); 
        shoot.action.Disable();
    }

    Vector2 rawMove = Vector2.zero;
    void OnMove(InputAction.CallbackContext context)
    {
        rawMove = context.action.ReadValue<Vector2>();
    }
    private void OnShoot(InputAction.CallbackContext context)
    {
        GameObject newShoot = Instantiate(prefabShot, shootingPoint.position, Quaternion.identity);
        Destroy(newShoot, 5f);
    }
}
