using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [Header("Ekrany")]
    public GameObject MainMenu;
    public GameObject PlayScreen;
    public GameObject SettingsScreen;


    public void BackToMainMenu()
    {
        PlayScreen.SetActive(false);
        SettingsScreen.SetActive(false);
        MainMenu.SetActive(true);
    }
    public void GoToPlayScreen()
    {
        MainMenu.SetActive(false);
        PlayScreen.SetActive(true);
    }

    public void GoToSettingsScreen()
    {
        MainMenu.SetActive(false);
        SettingsScreen.SetActive(true);
    }

    public void PlaySandMode()
    {
        SceneManager.LoadScene("Game");
    }

    public void PlayClassicMode()
    {
        SceneManager.LoadScene("Game");
    }
    public void OnExitButtonClick()
    {
        Application.Quit();
    }
}
