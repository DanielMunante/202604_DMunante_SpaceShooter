using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject pausePanel;

    [SerializeField] Button playButton;
    [SerializeField] Button quitMainButton;
    [SerializeField] Button resumeButton;
    [SerializeField] Button quitPauseButton;

    bool isPaused = false;

    void Awake()
    {
        //Pausa el tiempo inmediatamente para que el juego no empiece solo
        Time.timeScale = 0f;
    }

    void Start()
    {
        mainMenuPanel.SetActive(true);
        pausePanel.SetActive(false);

        playButton.onClick.AddListener(PlayGame);
        quitMainButton.onClick.AddListener(QuitGame);
        resumeButton.onClick.AddListener(ResumeGame);
        quitPauseButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        //El menu pausa se puede realizar
        //cuando el menu principal esta activo y cuando el juego ha terminado, no hace nada y declaramos pausa = false
        //cuando se presiona el boton pause
        if (mainMenuPanel.activeSelf || GameSession.Instance.isGameOver) return;
        bool pausePressed = false;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            pausePressed = true;
        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
            pausePressed = true;
        if (pausePressed)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PlayGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    void QuitGame()
    {
        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
        
}