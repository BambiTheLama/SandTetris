using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa kontroluj¹ca zachowanie pauzy.
/// </summary>
public class PauseController : MonoBehaviour
{
    [Header("Ekrany")]
    public GameObject PauseScreen;
    public bool paused;

    /// <summary>
    /// Metoda Update wywo³ywana raz na klatkê.
    /// Obs³uguje wstrzymywanie i wznawianie gry po naciœniêciu klawisza Escape.
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
    /// Kontynuowanie gry po klikniêciu przycisku "Kontynuuj".
    /// </summary>
    public void ContinueButton()
    {
        paused = false;
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Wyjœcie do menu g³ównego po klikniêciu przycisku "Wyjœcie".
    /// </summary>
    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
