using System;
using TMPro;
using UnityEngine;

public class LoadMenuManager : MonoBehaviour
{
    public GameObject LoadMenuUI;    // The save menu panel
    public GameObject pauseMenuUI;   // Reference back to pause menu
    public GameObject NoDataPopUp;
    public GameObject ErrorPopUp;
    public TextMeshProUGUI error;
    private string slotname;
    GameSaveManager gsm;

    void Start()
    {
        gsm = FindAnyObjectByType<GameSaveManager>();
        ErrorPopUp.SetActive(false);
        NoDataPopUp.SetActive(false);

        if(MainMenuManager.isLoaded)
        {
            LoadSlot(MainMenuManager.slotname);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ErrorPopUp.SetActive(false);
            LoadMenuUI.SetActive(false);
            pauseMenuUI.SetActive(false);
            NoDataPopUp.SetActive(false);
            Time.timeScale = 1f;
            AudioListener.pause = false; // unmute all audio
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PauseMenuManager.isPaused = false;
        }
    }

    // Called when "Back" is clicked
    public void BackToPauseMenu()
    {
        LoadMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void BacktoLoadMenu()
    {
        NoDataPopUp.SetActive(false);
    }

    // Example slot save functions
    public void LoadSlot(string slot)
    {
        slotname = slot;
        try
        {
            if(gsm.Load(slotname))
            {
                ErrorPopUp.SetActive(false);
                LoadMenuUI.SetActive(false);
                Time.timeScale = 1f;
                AudioListener.pause = false; // unmute all audio
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PauseMenuManager.isPaused = false;
            }
            else
            {
                NoDataPopUp.SetActive(true);
            }

        }
        catch (Exception e)
        {
            ErrorPopUp.SetActive(true);
            error.text = e.Message;
            Debug.Log(e.Message);
            Debug.Log(e);
        }
    }
}
