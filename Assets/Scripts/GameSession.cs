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

    //"Singleton para el gamesession"
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lives = startingLives;
        Time.timeScale = 1f;
        hud.RefreshHUD();
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

        //los componentes se desactivan por el gameover
        if (spaceship != null)
        {
            spaceship.enabled = false;

            //desactivar el Rigibody de la nave para que desaparezca en una
            Rigidbody2D rb = spaceship.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; //detener la nave
                rb.simulated = false;  // desactivar el rigibody
            }

        }
        if (enemyGenerator != null)
        {
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
}