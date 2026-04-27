using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] float speed = 10f;    //mejora variable serializadas
    [SerializeField] float lifeTime = 2f;  //agregar variable tiempo vida shot
    Rigidbody2D rb2d;

    //Variable Cacheada (clase)
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb2d.linearVelocity = Vector2.right * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignorar power-ups (para no moverlos ni destruirse al tocarlos)
        if (collision.collider.CompareTag("PowerUp")) return;

        //cuando el disparo toca algo a Player, no hace nada
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("BossShot"))
        {
            return;
        }
        Destroy(gameObject);        
    }

}
