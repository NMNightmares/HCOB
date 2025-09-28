using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class pause_handler : MonoBehaviour
{
    [Header("References (drag from Hierarchy)")]
    [SerializeField] private GameObject pauseMenu;        // PauseMenu (Canvas) - keep disabled at start
    [SerializeField] private GameObject firstSelected;     // e.g., ResumeButton
    [SerializeField] private Button hudPauseButton;        // your small HUD "Pause" button (optional)

    [Header("Options")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private bool unlockCursorWhilePaused = true;

    public static bool IsPaused { get; private set; }

    private void Awake()
    {
        if (pauseMenu) pauseMenu.SetActive(false); // start hidden
        Time.timeScale = 1f;
        IsPaused = false;

        if (hudPauseButton) hudPauseButton.onClick.AddListener(TogglePause);
    }

    private void OnDestroy()
    {
        if (hudPauseButton) hudPauseButton.onClick.RemoveListener(TogglePause);
    }

    private void Update()
    {
        // Allow Esc/Start to toggle pause as well
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause() => SetPaused(!IsPaused);
    public void OnResumeClicked() => SetPaused(false);

    public void OnMainMenuClicked()
    {
        SetPaused(false);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnQuitClicked()
    {
        // Before loading, unpause so time scale is restored
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");   // Make sure MainMenu is added to Build Settings
    }

    private void SetPaused(bool pause)
    {
        if (IsPaused == pause) return;
        IsPaused = pause;

        if (pauseMenu) pauseMenu.SetActive(pause);

        Time.timeScale = pause ? 0f : 1f;

        if (unlockCursorWhilePaused)
        {
            Cursor.visible = pause;
            Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (EventSystem.current)
            EventSystem.current.SetSelectedGameObject(pause ? firstSelected : null);
    }
}

