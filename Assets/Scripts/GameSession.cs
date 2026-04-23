using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance; //Variable global para que sea accesible desde cualquier lado

    [SerializeField] int startingLives = 3;
    [SerializeField] HudController hud;   // referencia al HUD, se arrastra desde el Inspector

    //desactivar el juego cuando es gameover
    [SerializeField] SpaceshipController spaceship;
    [SerializeField] EnemyGenerator enemyGenerator;

    public int score = 0;                 // pública para que el HUD la lea
    public int lives;                     // pública para que el HUD la lea
    bool isGameOver = false;

    [SerializeField] float timeToSpawnBoss = 60f;
    [SerializeField] GameObject boss;
    bool bossSpawned = false;
    float gameTimer = 0f;

    //"Singleton para el gamesession"
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lives = startingLives;
        //Time.timeScale = 1f;
        hud.RefreshHUD();
    }
    void Update()
    {
        if (isGameOver || bossSpawned) return;

        gameTimer += Time.deltaTime;
        if (gameTimer >= timeToSpawnBoss)
        {
            SpawnBoss();
        }
    }

    //Añadir score y que el Hubcontroller lo refresca
    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        hud.RefreshHUD();
    }

    //Añadir cuando pierde una vida y que el Hubcontroller lo refresca
    public void LoseLife()
    {
        if (isGameOver) return;
        lives--;
        hud.RefreshHUD();

        if (lives <= 0)
        {
            EndSession();
        }
    }

    //Para cuando el juego termine.
    void EndSession()
    {
        isGameOver = true;
        hud.ShowGameOver();

        //desactivar toda la nave
        if (spaceship != null)
        {
            spaceship.enabled = false;
            spaceship.gameObject.SetActive(false); //desparece la nave

        }
        //desactivar la generacion de enemigos
        if (enemyGenerator != null)
        {
            enemyGenerator.StopAllCoroutines();
            enemyGenerator.enabled = false;
        }

    }

    public void RestartSession()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnBossDefeated()
    {
        isGameOver = true;

        spaceship.enabled = false;
        StartCoroutine(VictorySequence());
    }

    IEnumerator VictorySequence()
    {
        Rigidbody2D rb = spaceship.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.right * 5f;
        yield return new WaitForSeconds(2f);

        spaceship.gameObject.SetActive(false);
        hud.ShowVictory();
    }

    void SpawnBoss()
    {
        bossSpawned = true;

        //cuando aparece el Boss, se tiene q detener la generación de enemigos
        enemyGenerator.StopAllCoroutines();
        enemyGenerator.enabled = false;
        
        //hacer que aparezca el Boss
        boss.SetActive(true);
    }

}