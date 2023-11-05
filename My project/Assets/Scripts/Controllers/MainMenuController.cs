using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Klasa kontroluj¹ca zachowanie menu gry.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Ekrany")]
    public GameObject MainMenu; // Ekran g³ównego menu
    public GameObject PlayScreen; // Ekran wyboru trybu gry
    public GameObject SettingsScreen; // Ekran ustawieñ gry
    public GameObject ScoreboardScreen; // Ekran tabeli wyników

    [Header("Audio")]
    AudioSource audioSource; // Komponent audio
    public AudioClip StartGameAudio; // DŸwiêk rozpoczêcia gry
    public AudioClip ButtonAudio; // DŸwiêk przycisku

    [Header("Ustawienia")]
    public AudioMixer audioMixer; // Mixer dŸwiêku
    public Toggle FXToogle, MusicToogle; // Prze³¹czniki wyciszenia efektów dŸwiêkowych i muzyki
    public Image FXImage, MusicImage; // Obrazek do wyœwietlania stanu efektów dŸwiêkowych i muzyki
    public Sprite OnImage, OffImage; // Grafika do stanu w³¹czonego i wy³¹czonego


    /// <summary>
    /// Inicjalizacja komponentów audio i ustawieñ dŸwiêku przy starcie.
    /// </summary>
    private void Start()
    {
        audioSource = GetComponent <AudioSource>();

        audioMixer.GetFloat("FX_Volume", out float valueOfFX);
        audioMixer.GetFloat("Music_Volume", out float valueOfMusic);

        if (Mathf.Approximately(valueOfFX, -80.0f))
        {
            FXToogle.isOn = false;
        }
        if (Mathf.Approximately(valueOfMusic, -80.0f))
        {
            MusicToogle.isOn = false;
        }
    }

    /// <summary>
    /// Powrót do menu g³ównego.
    /// </summary>
    public void BackToMainMenu()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        PlayScreen.SetActive(false);
        SettingsScreen.SetActive(false);
        ScoreboardScreen.SetActive(false);
        MainMenu.SetActive(true);
    }

    /// <summary>
    /// Przejœcie do ekranu rozpoczêcia gry.
    /// </summary>
    public void GoToPlayScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        PlayScreen.SetActive(true);
    }

    /// <summary>
    /// Przejœcie do ekranu ustawieñ.
    /// </summary>
    public void GoToSettingsScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        SettingsScreen.SetActive(true);
    }

    /// <summary>
    /// Przejœcie do ekranu wyników.
    /// </summary>
    public void GoToScoreboardScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        ScoreboardScreen.SetActive(true);
    }

    /// <summary>
    /// Rozpoczêcie trybu gry "Sand Mode".
    /// </summary>
    public void PlaySandMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound("SandModeGame"));
    }

    /// <summary>
    /// Rozpoczêcie trybu gry "Classic Mode".
    /// </summary>
    public void PlayClassicMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound("ClassicModeGame"));
    }

    /// <summary>
    /// Rozpoczêcie trybu gry "Elemental Mode".
    /// </summary>
    public void PlayElementalMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound("ElementalModeGame"));
    }

    /// <summary>
    /// Asynchroniczne ³adowanie sceny gry po zakoñczeniu dŸwiêku rozpoczêcia gry.
    /// </summary>
    private IEnumerator LoadGameSceneAfterSound(string SceneName)
    {
        yield return new WaitForSeconds(StartGameAudio.length);
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// Obs³uga przycisku wyjœcia z gry.
    /// </summary>
    public void OnExitButtonClick()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        Application.Quit();
    }

    /// <summary>
    /// Wyciszenie/w³¹czenie muzyki.
    /// </summary>
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
            audioMixer.SetFloat("Music_Volume", -80f);
        }
    }

    /// <summary>
    /// Wyciszenie/w³¹czenie efektów dŸwiêkowych.
    /// </summary>
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
