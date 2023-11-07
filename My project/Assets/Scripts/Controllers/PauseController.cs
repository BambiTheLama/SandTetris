using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa kontrolująca zachowanie pauzy.
/// </summary>
public class PauseController : MonoBehaviour
{
    [Header("Ekrany")]
    public GameObject PauseScreen;
    public bool paused;

    /// <summary>
    /// Metoda Update wywoływana raz na klatkę.
    /// Obsługuje wstrzymywanie i wznawianie gry po naciśnięciu klawisza Escape.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                paused = true;
                PauseScreen.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
                PauseScreen.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    /// <summary>
    /// Kontynuowanie gry po kliknięciu przycisku "Kontynuuj".
    /// </summary>
    public void ContinueButton()
    {
        paused = false;
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Wyjście do menu głównego po kliknięciu przycisku "Wyjście".
    /// </summary>
    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
