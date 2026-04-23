using UnityEngine;

public class BossShot : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 3f;  //agregar variable tiempo vida shot
    Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Los proyectiles van hacia la izquierda (hacia el jugador)
        rb2d.linearVelocity = Vector2.left * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //cuando el disparo toca a Player, le va quitando vidas
        if (collision.collider.CompareTag("Player"))
        {
            GameSession.Instance.LoseLife();
            Destroy(gameObject);
        }
    }
}