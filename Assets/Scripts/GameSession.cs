using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    //Jefe final
    [SerializeField] float timeToSpawnBoss = 60f;
    [SerializeField] GameObject boss;
    bool bossSpawned = false;
    float gameTimer = 0f;

    //Sistema de vidas y score
    [SerializeField] int startingLives = 3;
    public int score = 0; //pública para que el HUD la lea
    public int lives; //pública para que el HUD la lea
    public bool isGameOver = false;

    //Power-Up Vida Extra
    [SerializeField] GameObject healthPowerUpPrefab;
    [SerializeField] int scoreThresholdForHealth = 500;
    int nextHealthThreshold = 500;
    [SerializeField] float spawnX = -1.5f;
    [SerializeField] float spawnMinY = -0.8f;
    [SerializeField] float spawnMaxY = 0.8f;

    //Power-Up Berserker
    [SerializeField] GameObject berserkerPowerUpPrefab;

    //Audio
    AudioSource audioSource;
    [SerializeField] AudioClip playerHitSound;
    [SerializeField] float playerHitVolume = 0.8f; 
    [SerializeField] AudioClip victorySound;
    [SerializeField] float victoryVolume = 1f;
    [SerializeField] AudioSource musicPlayer;

    public static GameSession Instance; //Variable global para que sea accesible desde cualquier lado

    [SerializeField] HudController hud;   // referencia al HUD, se arrastra desde el Inspector
    [SerializeField] SpaceshipController spaceship;
    [SerializeField] EnemyGenerator enemyGenerator;
        
    //Sistema de Racha
    [SerializeField] int killsToStartCombo = 10;//kills para iniciar racha
    [SerializeField] float timeWindowForCombo = 10f;//tiempo para la racha
    [SerializeField] float comboDuration = 5f;//duración de la racha activa
    [SerializeField] int scoreMultiplier = 2;//multiplicador durante racha
    
    int killStreakCount = 0;
    float firstKillTime = 0f;
    public bool isComboActive = false;
    Vector3 lastEnemyKilledPosition;//posicion del último enemigo muerto para que aparezca powerup

    //"Singleton para el gamesession"
    private void Awake()
    {
        Instance = this; 
        audioSource = GetComponent<AudioSource>();
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

        if (isComboActive)
        {
            amount *= scoreMultiplier;
        }

        score += amount;
        hud.RefreshHUD();

        if (score >= nextHealthThreshold)
        {
            SpawnHealthPowerUp();
            nextHealthThreshold += scoreThresholdForHealth;
        }
    }
    
    //Añadir cuando pierde una vida y que el Hubcontroller lo refresca
    public void LoseLife()
    {
        if (isGameOver) return;
        lives--;
        hud.RefreshHUD();
        PlaySound(playerHitSound, playerHitVolume);

        if (lives <= 0)
        {
            EndSession();
        }
    }

    public void AddLife()
    {
        if (isGameOver) return;
        lives++;
        hud.RefreshHUD();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //recarga la escena actual
    }

    public void OnBossDefeated()
    {
        isGameOver = true;
        musicPlayer.Stop(); //detener musica de fondo
        PlaySound(victorySound, victoryVolume); //activar musica de victoria

        spaceship.enabled = false;
        StartCoroutine(VictorySequence());
    }

    IEnumerator VictorySequence()
    {
        //poner un delay cuando se gana para luego entrar a la secuencia de victoria
        yield return new WaitForSeconds(2f);

        Rigidbody2D rb = spaceship.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.right * 5f;
        
        yield return new WaitForSeconds(2f);

        spaceship.gameObject.SetActive(false);
        hud.ShowVictory();
    }
    void SpawnHealthPowerUp()
    {
        if (healthPowerUpPrefab == null) return;

        float randomY = Random.Range(spawnMinY, spawnMaxY);
        Vector3 spawnPosition = new Vector3(spawnX, randomY, 0);

        Instantiate(healthPowerUpPrefab, spawnPosition, Quaternion.identity);
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

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void RegisterKill(Vector3 enemyPosition)
    {
        if (isGameOver || isComboActive) return;
        lastEnemyKilledPosition = enemyPosition; //para guardar posiction de ultimo enemigo muerto

        //registra el tiempo desde el primer kill
        if (killStreakCount == 0)
        {
            firstKillTime = Time.time;
        }
        killStreakCount++; 
        
        //reiniciara contador de kill si pasa el tiempo para activar la racha
        if (Time.time - firstKillTime > timeWindowForCombo)
        {
            killStreakCount = 1;
            firstKillTime = Time.time;
            return;
        }

        //cuando alcanza la racha se activa el combo
        if (killStreakCount >= killsToStartCombo)
        {
            StartCombo();
        }
    }
    void StartCombo()
    {
        isComboActive = true;
        killStreakCount = 0;
        SpawnBerserkerPowerUp();
        StartCoroutine(EndComboAfterDelay());
    }

    IEnumerator EndComboAfterDelay()
    {
        yield return new WaitForSeconds(comboDuration);
        isComboActive = false;
    }

    void SpawnBerserkerPowerUp()
    {
        Instantiate(berserkerPowerUpPrefab, lastEnemyKilledPosition, Quaternion.identity);
    }

    public void SpawnBerserkerPowerUp(Vector3 position)
    {
        if (berserkerPowerUpPrefab == null) return;
        Instantiate(berserkerPowerUpPrefab, position, Quaternion.identity);
    }

}