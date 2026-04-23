using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


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
        // Pausar el juego INMEDIATAMENTE (antes que cualquier Start de otros scripts)
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
        if (!mainMenuPanel.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame)
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

    void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}