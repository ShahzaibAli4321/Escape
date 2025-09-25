using System;
using TMPro;
using UnityEngine;

public class SaveMenuManager : MonoBehaviour
{
    public GameObject saveMenuUI;    // The save menu panel
    public GameObject pauseMenuUI;   // Reference back to pause menu
    public GameObject WarningPopUp;
    public GameObject ErrorPopUp;
    public GameObject Saveconfirm;
    public GameObject OptionsMenu;
    public TextMeshProUGUI error;
    private string slotname;
    GameSaveManager gsm;

    void Start()
    {
        gsm = FindAnyObjectByType<GameSaveManager>();
        ErrorPopUp.SetActive(false);
        Saveconfirm.SetActive(false);
        OptionsMenu.SetActive(false);
        WarningPopUp.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            saveMenuUI.SetActive(false);
            WarningPopUp.SetActive(false);
            ErrorPopUp.SetActive(false);
            Saveconfirm.SetActive(false);
            pauseMenuUI.SetActive(false);
            OptionsMenu.SetActive(false);
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
        saveMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        WarningPopUp.SetActive(false);
        OptionsMenu.SetActive(false);
    }

    public void trySave(string slot)
    {
        WarningPopUp.SetActive(true);
        slotname = slot;
    }

    public void BackToSave()
    {
        WarningPopUp.SetActive(false);
        ErrorPopUp.SetActive(false);
        Saveconfirm.SetActive(false);
    }

    // Example slot save functions
    public void SaveSlot()
    {
        try
        {
            Debug.Log("Saving game to " + slotname);
            gsm.Save(slotname);
            WarningPopUp.SetActive(false);
            Saveconfirm.SetActive(true);
        } 
        catch(Exception e)
        {
            WarningPopUp.SetActive(false);
            ErrorPopUp.SetActive(true);
            error.text = e.Message;
            Debug.Log(e.Message);
            Debug.Log(e);
        }
    }

    public void Ok()
    {
        ErrorPopUp.SetActive(false);
    }
}
