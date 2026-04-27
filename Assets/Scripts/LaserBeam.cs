using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] float lifeTime = 0.2f;
    [SerializeField] float speed = 20f;

    private void Start()
    {
        //para que no acumule en memoria
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        //movimiento hacia la derecha
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Destruye enemigos al contacto
        if (other.CompareTag("EnemyShipBlue"))
        {
            GameSession.Instance.AddScore(10);
            Destroy(other.gameObject);
        }

        //destruir el disparo del boss
        if (other.CompareTag("BossShot"))
        {
            Destroy(other.gameObject);
        }
    }
}