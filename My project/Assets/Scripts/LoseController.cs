using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa kontroluj�ca zachowanie po przegranej.
/// </summary>
public class LoseController : MonoBehaviour
{
    /// <summary>
    /// TextMeshProUGUI do wy�wietlania czasu gry.
    /// </summary>
    public TextMeshProUGUI timerText;

    /// <summary>
    /// TextMeshProUGUI do wy�wietlania ilo�ci zdobytych  punkt�w.
    /// </summary>
    public TextMeshProUGUI pointsText;

    /// <summary>
    /// GameObject reprezentuj�cy ekran przegranej.
    /// </summary>
    public GameObject LoseScreen;

    /// <summary>
    /// Referencja do GameScript do zarz�dzania gr�.
    /// </summary>
    public GameScript gameScript;

    /// <summary>
    /// Referencja do StatsController do zarz�dzania statystykami gry.
    /// </summary>
    public StatsController statsController;

    /// <summary>
    /// Metoda Update wywo�ywana raz na klatk�.
    /// Sprawdza, czy gra si� zako�czy�a i wy�wietla ekran przegranej ze statystykami.
    /// </summary>
    private void Update()
    {
        if (gameScript.EndGame)
        {
            LoseScreen.SetActive(true);
            timerText.text = "Czas gry: " + statsController.timerText.text;
            pointsText.text = "Wynik: " + statsController.pointsText.text;
        }
    }

    /// <summary>
    /// Ponowne rozpocz�cie gry po klikni�ciu przycisku restart.
    /// Resetuje licznik czasu i punkty oraz ukrywa ekran przegranej.
    /// </summary>
    public void RestartButton()
    {
        gameScript.RestartGame();
        statsController.ResetTimer();
        statsController.ResetPoints();
        LoseScreen.SetActive(false);
    }

    /// <summary>
    /// Powr�t do menu g��wnego po klikni�ciu przycisku wyj�cia.
    /// </summary>
    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
