using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa kontroluj�ca zachowanie pauzy.
/// </summary>
public class PauseController : MonoBehaviour
{
    [Header("Ekrany")]
    public GameObject PauseScreen;
    public bool paused;

    /// <summary>
    /// Metoda Update wywo�ywana raz na klatk�.
    /// Obs�uguje wstrzymywanie i wznawianie gry po naci�ni�ciu klawisza Escape.
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
    /// Kontynuowanie gry po klikni�ciu przycisku "Kontynuuj".
    /// </summary>
    public void ContinueButton()
    {
        paused = false;
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Wyj�cie do menu g��wnego po klikni�ciu przycisku "Wyj�cie".
    /// </summary>
    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
