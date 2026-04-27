using UnityEngine;

public class EnemySpaceShipBlue : MonoBehaviour
{
    [SerializeField] AudioClip explosionSound;
    [SerializeField] float explosionVolume = 1f;

    [SerializeField] float speed = 3f;
    [SerializeField] float leftLimitCoordinate = -3f;
    [SerializeField] float rightScreenLimit = 3f;
    bool isGoingLeft = true;

    [SerializeField] int scoreValue = 10; //score cuando se mata enemigos

    void Update()
    {
        Vector2 direction = isGoingLeft ? Vector2.left : Vector2.right;
        transform.Translate(direction * speed * Time.deltaTime);

        if (isGoingLeft && (transform.position.x < leftLimitCoordinate))
        {
            isGoingLeft = false; 
            Destroy(gameObject);// para el enemigo muera cuando va al lado izquierdo ya no habria efecto rebote
        }

        //efecto rebote
        if (!isGoingLeft && (transform.position.x > rightScreenLimit))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PlayerShot"))
        {
            GameSession.Instance.AddScore(scoreValue); //agregar score cuando se mata los enemigos
            GameSession.Instance.RegisterKill(transform.position); //guardar los kills para la racha
            GameSession.Instance.PlaySound(explosionSound, explosionVolume); //reproducir sonido
            Destroy(collision.collider.gameObject);
            Destroy(gameObject);
        
        }
        else if (collision.collider.CompareTag("Player"))   // cuando la nave (player) se choca, pierda vida
        {
            GameSession.Instance.LoseLife();
            Destroy(gameObject);
        }
    }

}
