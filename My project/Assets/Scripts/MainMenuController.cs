using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [Header("Ekrany")]
    public GameObject MainMenu;
    public GameObject PlayScreen;
    public GameObject SettingsScreen;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip StartGameAudio, ButtonAudio;

    [Header("Ustawienia")]
    public AudioMixer audioMixer;
    public Toggle FXToogle, MusicToogle;
    public Image FXImage, MusicImage;
    public Sprite OnImage, OffImage;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        float valueOfFX;
        float valueOfMusic;
        audioMixer.GetFloat("FX_Volume", out valueOfFX);
        audioMixer.GetFloat("Music_Volume", out valueOfMusic);

        if (Mathf.Approximately(valueOfFX, -80.0f))
        {
            FXToogle.isOn = false;
        }
        if (Mathf.Approximately(valueOfMusic, -80.0f))
        {
            MusicToogle.isOn = false;
        }
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

    public void MuteMusic()
    {
        if (MusicToogle.isOn)
        {
            MusicImage.sprite = OnImage;
            audioMixer.SetFloat("Music_Volume", 0);
        }

        else
        { 
            MusicImage.sprite = OffImage;
            audioMixer.SetFloat("Music_Volume",-80f);
        }
    }
    public void MuteFX()
    {
        if (FXToogle.isOn)
        {
            FXImage.sprite = OnImage;
            audioMixer.SetFloat("FX_Volume", 0);
        }
        else
        {
            FXImage.sprite = OffImage;
            audioMixer.SetFloat("FX_Volume", -80f);
        }
    }
}


