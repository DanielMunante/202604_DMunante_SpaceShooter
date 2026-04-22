using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText; //texto del score
    [SerializeField] TextMeshProUGUI livesText; //texto de vidas
    [SerializeField] GameObject gameOverPanel; //panel cuando se termine el juego
    [SerializeField] TextMeshProUGUI finalScoreText; //texto puntuacion final
    [SerializeField] Button restartButton; //boton para iniciar el juego nuevamente
    [SerializeField] Button menuButton; //boton menu principal

    private void Start()
    {
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(OnRestartClicked);
        menuButton.onClick.AddListener(OnMenuClicked);
    }

    //Cuando cambia el score o las vidas
    public void RefreshHUD()
    {
        scoreText.text = "Score: " + GameSession.Instance.score;
        livesText.text = "Vidas: " + GameSession.Instance.lives;
    }

    //Cuando el juego termina muestra la puntuacion final
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Puntuación final: " + GameSession.Instance.score;
    }

    //para volver a iniciar el juego
    void OnRestartClicked()
    {
        restartButton.interactable = false; 
        menuButton.interactable = false;
        GameSession.Instance.RestartSession();
    }

    //para volver al menu principal
    void OnMenuClicked()
    {
        restartButton.interactable = false;
        menuButton.interactable = false;
        GameSession.Instance.GoToMainMenu();
    }
}