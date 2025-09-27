using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject LoadMenuUI;
    public GameObject OptionsMenuUI;
    public GameObject NoDataPopUp;
    public GameObject ErrorPopUp;
    public GameObject WarningPopUp;
    public Texture2D cursorTexture;
    public TextMeshProUGUI error;
    public static string slotname;
    GameSaveManager gsm1;
    public static bool isLoaded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadMenuUI.SetActive(false);
        OptionsMenuUI.SetActive(false);
        NoDataPopUp.SetActive(false);
        ErrorPopUp.SetActive(false);
        WarningPopUp.SetActive(false);
        gsm1 = FindAnyObjectByType<GameSaveManager>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // set the custom cursor again
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        AudioListener.pause = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Options()
    {
        OptionsMenuUI.SetActive(true);
    }

    public void Ok()
    {
        ErrorPopUp.SetActive(false);
        NoDataPopUp.SetActive(false);
        WarningPopUp.SetActive(false);
    }

    public void TryNewGame()
    {
        WarningPopUp.SetActive(true);
    }

    public void NewGame()
    {
        try
        {
            isLoaded = false;
            SaveSystem.DeleteAllSaves();
            SceneManager.LoadScene(1);
        }
        catch (Exception e)
        {
            ErrorPopUp.SetActive(true);
            error.text = e.Message;
            Debug.Log(e.Message);
            Debug.Log(e);
        }
    }

    public void LoadGame()
    {
        LoadMenuUI.SetActive(true);
    }

    public void BacktoMainMenu()
    {
        LoadMenuUI.SetActive(false);
        OptionsMenuUI.SetActive(false);
        NoDataPopUp.SetActive(false);
        ErrorPopUp.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadSlot(string slot)
    {
        slotname = slot;
        try
        {
            if (gsm1.Load(slotname))
            {
                isLoaded = true;
                SceneManager.LoadScene(1);
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
