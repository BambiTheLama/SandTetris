using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Klasa kontroluj�ca zachowanie menu gry.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Ekrany")]
    public GameObject MainMenu; // Ekran g��wnego menu
    public GameObject PlayScreen; // Ekran wyboru trybu gry
    public GameObject SettingsScreen; // Ekran ustawie� gry
    public GameObject ScoreboardScreen; // Ekran tabeli wynik�w

    [Header("Audio")]
    AudioSource audioSource; // Komponent audio
    public AudioClip StartGameAudio; // D�wi�k rozpocz�cia gry
    public AudioClip ButtonAudio; // D�wi�k przycisku

    [Header("Ustawienia")]
    public AudioMixer audioMixer; // Mixer d�wi�ku
    public Toggle FXToogle, MusicToogle; // Prze��czniki wyciszenia efekt�w d�wi�kowych i muzyki
    public Image FXImage, MusicImage; // Obrazek do wy�wietlania stanu efekt�w d�wi�kowych i muzyki
    public Sprite OnImage, OffImage; // Grafika do stanu w��czonego i wy��czonego


    /// <summary>
    /// Inicjalizacja komponent�w audio i ustawie� d�wi�ku przy starcie.
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
    /// Powr�t do menu g��wnego.
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
    /// Przej�cie do ekranu rozpocz�cia gry.
    /// </summary>
    public void GoToPlayScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        PlayScreen.SetActive(true);
    }

    /// <summary>
    /// Przej�cie do ekranu ustawie�.
    /// </summary>
    public void GoToSettingsScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        SettingsScreen.SetActive(true);
    }

    /// <summary>
    /// Przej�cie do ekranu wynik�w.
    /// </summary>
    public void GoToScoreboardScreen()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        MainMenu.SetActive(false);
        ScoreboardScreen.SetActive(true);
    }

    /// <summary>
    /// Rozpocz�cie trybu gry "Sand Mode".
    /// </summary>
    public void PlaySandMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound());
    }

    /// <summary>
    /// Rozpocz�cie trybu gry "Classic Mode".
    /// </summary>
    public void PlayClassicMode()
    {
        audioSource.clip = StartGameAudio;
        audioSource.Play();
        StartCoroutine(LoadGameSceneAfterSound());
    }

    /// <summary>
    /// Asynchroniczne �adowanie sceny gry po zako�czeniu d�wi�ku rozpocz�cia gry.
    /// </summary>
    private IEnumerator LoadGameSceneAfterSound()
    {
        yield return new WaitForSeconds(StartGameAudio.length);
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Obs�uga przycisku wyj�cia z gry.
    /// </summary>
    public void OnExitButtonClick()
    {
        audioSource.clip = ButtonAudio;
        audioSource.Play();
        Application.Quit();
    }

    /// <summary>
    /// Wyciszenie/w��czenie muzyki.
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
    /// Wyciszenie/w��czenie efekt�w d�wi�kowych.
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
