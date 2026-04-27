using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    //Audios
    [SerializeField] AudioClip hitSound;//sonido cuando es atacado el Boss
    [SerializeField] AudioClip deathSound;//sonido cuando muere el Boss
    [SerializeField] float hitVolume = 0.7f;
    [SerializeField] float deathVolume = 1f;

    //Vidas
    [SerializeField] int maxHealth = 200; //vida del Boss
    [SerializeField] int scoreOnDeath = 1000; //score cuando muere el Boss
    [SerializeField] int scoreOnHit = 10;

    //Movimientos
    [SerializeField] float minSpeed = 2.5f; //mejora velocidad cuando tiene vida completa el Boss
    [SerializeField] float maxSpeed = 6f; //mejora velocidad cuando tiene poca vida el Boss
    [SerializeField] Transform topRight; 
    [SerializeField] Transform bottomLeft;
    [SerializeField] float changeDirectionInterval = 2f; //cada cuanto segundos cambia el destino el Boos

    //Ataques
    [SerializeField] GameObject bossShotPrefab;
    [SerializeField] Transform[] shootingPoints; 
    [SerializeField] float shootInterval = 3f; //cada 3 segundos dispara el Boss
    [SerializeField] int minShotsPerBurst = 5;  //cantidad de disparos cuando tiene vida completa el Boss
    [SerializeField] int maxShotsPerBurst = 20; //cantidad de disparos cuando tiene poco vida el Boss
    [SerializeField] float timeBetweenShots = 0.2f; //cada cuanto dispara

    //Berserker
    [SerializeField] int damageTresholdForBerserker = 50;
    int nextBerserkerThreshold = 50;

    int currentHealth; //vida actual de Boss
    Vector3 targetPosition; //para marcar el movimiento destino aleatorio del Boss

    //Escudo Boss
    [SerializeField] Color shieldColor = Color.green; //escudo del Boss
    SpriteRenderer sr;
    Color originalColor;
    bool isShielded = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void Start()
    {
        currentHealth = maxHealth;
        targetPosition = Vector3.Lerp(topRight.position, bottomLeft.position, Random.Range(0f, 1f));
        InvokeRepeating("ChangeDirection", changeDirectionInterval, changeDirectionInterval);
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        //velocidad dinámica según vida
        float currentSpeed = CalculateCurrentSpeed();
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
    }

    void ChangeDirection()
    {
        targetPosition = Vector3.Lerp(topRight.position, bottomLeft.position, Random.Range(0f, 1f));
    }

    float CalculateCurrentSpeed()
    {
        float healthPercent = (float)currentHealth / maxHealth;           
        float dangerLevel = 1f - healthPercent;                           
        return Mathf.Lerp(minSpeed, maxSpeed, dangerLevel);
    }

    int CalculateCurrentShotsPerBurst()
    {
        float healthPercent = (float)currentHealth / maxHealth;
        float dangerLevel = 1f - healthPercent;
        return Mathf.RoundToInt(Mathf.Lerp(minShotsPerBurst, maxShotsPerBurst, dangerLevel));
    }


    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);

            // objetivo cuando va a atacar aparezca un escudo que lo haga invencible al jefe
            isShielded = true;
            sr.color = shieldColor;

            int currentShots = CalculateCurrentShotsPerBurst();

            for (int i = 0; i < currentShots; i++)
            {
                // Disparar desde todos los puntos a la vez
                foreach (Transform point in shootingPoints)
                {
                    Instantiate(bossShotPrefab, point.position, Quaternion.identity);
                }
                yield return new WaitForSeconds(timeBetweenShots);
            }

            // cuando termine de disparar , el escudo se desactiva
            isShielded = false;
            sr.color = originalColor;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PlayerShot"))
        {
            Destroy(collision.collider.gameObject);

            if (isShielded) return; //cuando tiene escudo no pasa nada
            currentHealth--; //cuando el player dispara al Boss reduce su vida            
            GameSession.Instance.AddScore(scoreOnHit); //agregar score por disparo
            GameSession.Instance.PlaySound(hitSound, hitVolume); //sonido cuando es atacado

            int damageTaken = maxHealth - currentHealth;
            if (damageTaken >= nextBerserkerThreshold)
            {
                nextBerserkerThreshold += damageTresholdForBerserker;
                SpawnBerserkerNearBoss();
            }

            if (currentHealth == 0)
            {
                GameSession.Instance.AddScore(scoreOnDeath);//1000 puntos de score cuando muere el Boss
                GameSession.Instance.PlaySound(deathSound, deathVolume);//sonido cuando muerte el Boss
                GameSession.Instance.OnBossDefeated();//se inicia la secuencia de victoria
                Destroy(gameObject);
            }
        }
        else if (collision.collider.CompareTag("Player"))
        {
            GameSession.Instance.LoseLife();//player reduce vida cuando se choca con el BOSS
        }
    }

    void SpawnBerserkerNearBoss()
    {
        float spawnX = transform.position.x - 3f;
        float spawnY = transform.position.y;
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        GameSession.Instance.SpawnBerserkerPowerUp(spawnPos);
    }
}