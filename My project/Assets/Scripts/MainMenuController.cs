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
    AudioSource audioSource;
    public AudioClip StartGameAudio, ButtonAudio;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void BackToMainMenu()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        PlayScreen.SetActive(false);
        SettingsScreen.SetActive(false);
        MainMenu.SetActive(true);
    }
    public void GoToPlayScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        PlayScreen.SetActive(true);
    }

    public void GoToSettingsScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        SettingsScreen.SetActive(true);
    }

    public void PlaySandMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound());
    }

    public void PlayClassicMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound());
    }

    private IEnumerator LoadGameSceneAfterSound()
    {
        yield return new WaitForSeconds(StartGameAudio.length);
        SceneManager.LoadScene("Game");
    }
    public void OnExitButtonClick()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        Application.Quit();
    }
}
