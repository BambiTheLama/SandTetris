using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa kontroluj¹ca zachowanie po przegranej.
/// </summary>
public class LoseController : MonoBehaviour
{
    /// <summary>
    /// TextMeshProUGUI do wyœwietlania czasu gry.
    /// </summary>
    public TextMeshProUGUI timerText;

    /// <summary>
    /// TextMeshProUGUI do wyœwietlania iloœci zdobytych  punktów.
    /// </summary>
    public TextMeshProUGUI pointsText;

    /// <summary>
    /// GameObject reprezentuj¹cy ekran przegranej.
    /// </summary>
    public GameObject LoseScreen;

    /// <summary>
    /// Referencja do GameScript do zarz¹dzania gr¹.
    /// </summary>
    public GameScript gameScript;

    /// <summary>
    /// Referencja do StatsController do zarz¹dzania statystykami gry.
    /// </summary>
    public StatsController statsController;

    /// <summary>
    /// Metoda Update wywo³ywana raz na klatkê.
    /// Sprawdza, czy gra siê zakoñczy³a i wyœwietla ekran przegranej ze statystykami.
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
    /// Ponowne rozpoczêcie gry po klikniêciu przycisku restart.
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
    /// Powrót do menu g³ównego po klikniêciu przycisku wyjœcia.
    /// </summary>
    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
