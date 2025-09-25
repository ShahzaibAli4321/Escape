using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject SaveMenuUI;
    public GameObject LoadMenuUI;
    public GameObject OptionsMenuUI;
    public KeyCode pauseKey = KeyCode.Escape;
    public static bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        SaveMenuUI.SetActive(false);
        LoadMenuUI.SetActive(false);
        OptionsMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false; // unmute all audio
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    public void Restart()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false; // unmute all audio
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    public void Save()
    {
        pauseMenuUI.SetActive(false);
        SaveMenuUI.SetActive(true);
    }

    public void Load()
    {
        pauseMenuUI.SetActive(false);
        LoadMenuUI.SetActive(true);
    }

    public void Options()
    {
        pauseMenuUI.SetActive(false);
        OptionsMenuUI.SetActive(true);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true; // mute all audio
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }
}
