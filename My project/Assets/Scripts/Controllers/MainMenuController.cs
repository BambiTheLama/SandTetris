using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Klasa kontrolująca zachowanie menu gry.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Ekrany")]
    public GameObject MainMenu; // Ekran głównego menu
    public GameObject PlayScreen; // Ekran wyboru trybu gry
    public GameObject SettingsScreen; // Ekran ustawień gry
    public GameObject ScoreboardScreen; // Ekran tabeli wyników

    [Header("Audio")]
    AudioSource audioSource; // Komponent audio
    public AudioClip StartGameAudio; // Dźwięk rozpoczęcia gry
    public AudioClip ButtonAudio; // Dźwięk przycisku

    [Header("Ustawienia")]
    public AudioMixer audioMixer; // Mixer dźwięku
    public Toggle FXToogle, MusicToogle; // Przełączniki wyciszenia efektów dźwiękowych i muzyki
    public Image FXImage, MusicImage; // Obrazek do wyświetlania stanu efektów dźwiękowych i muzyki
    public Sprite OnImage, OffImage; // Grafika do stanu włączonego i wyłączonego


    /// <summary>
    /// Inicjalizacja komponentów audio i ustawień dźwięku przy starcie.
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
    /// Powrót do menu głównego.
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
    /// Przejście do ekranu rozpoczęcia gry.
    /// </summary>
    public void GoToPlayScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        PlayScreen.SetActive(true);
    }

    /// <summary>
    /// Przejście do ekranu ustawień.
    /// </summary>
    public void GoToSettingsScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        SettingsScreen.SetActive(true);
    }

    /// <summary>
    /// Przejście do ekranu wyników.
    /// </summary>
    public void GoToScoreboardScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        ScoreboardScreen.SetActive(true);
    }

    /// <summary>
    /// Rozpoczęcie trybu gry "Sand Mode".
    /// </summary>
    public void PlaySandMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound("SandModeGame"));
    }

    /// <summary>
    /// Rozpoczęcie trybu gry "Classic Mode".
    /// </summary>
    public void PlayClassicMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound("ClassicModeGame"));
    }

    /// <summary>
    /// Rozpoczęcie trybu gry "Elemental Mode".
    /// </summary>
    public void PlayElementalMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound("ElementalModeGame"));
    }

    /// <summary>
    /// Asynchroniczne ładowanie sceny gry po zakończeniu dźwięku rozpoczęcia gry.
    /// </summary>
    private IEnumerator LoadGameSceneAfterSound(string SceneName)
    {
        yield return new WaitForSeconds(StartGameAudio.length);
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// Obsługa przycisku wyjścia z gry.
    /// </summary>
    public void OnExitButtonClick()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        Application.Quit();
    }

    /// <summary>
    /// Wyciszenie/włączenie muzyki.
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
    /// Wyciszenie/włączenie efektów dźwiękowych.
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
