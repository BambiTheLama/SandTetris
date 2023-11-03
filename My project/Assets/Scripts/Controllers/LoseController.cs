using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

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
    public GameScript sandScript;
    public NormalTetrisScript classicScript;

    /// <summary>
    /// Referencja do StatsController do zarz¹dzania statystykami gry.
    /// </summary>
    public StatsController statsController;
    readonly string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "best_scores.json");
    private bool hasSavedScore = false; // Flaga oznaczaj¹ca, czy wynik zosta³ ju¿ zapisany.


    /// <summary>
    /// Metoda Update wywo³ywana raz na klatkê.
    /// Sprawdza, czy gra siê zakoñczy³a i wyœwietla ekran przegranej ze statystykami.
    /// </summary>
    private void Update()
    {
        if (sandScript)
        {
            if (sandScript.EndGame && !hasSavedScore)
            {
                LoseScreen.SetActive(true);
                timerText.text = "Czas gry: " + statsController.timerText.text;
                pointsText.text = "Wynik: " + statsController.pointsText.text;

                // Odczytaj najlepsze wyniki z pliku (jeœli istniej¹).
                BestScores bestScores = LoadBestScores();


                // Dodaj nowy wynik do listy najlepszych wyników.
                bestScores.scores.Add(new ScoreData(statsController.pointsText.text, statsController.timerText.text, "Sand"));

                // Sortuj wyniki od najlepszego do najgorszego.
                bestScores.scores.Sort((x, y) => y.Score.CompareTo(x.Score));

                // Ogranicz listê do np. 10 najlepszych wyników (lub dowolnej liczby).
                if (bestScores.scores.Count > 10)
                {
                    bestScores.scores.RemoveRange(10, bestScores.scores.Count - 10);
                }

                // Zapisz zaktualizowane wyniki do pliku.
                SaveBestScores(bestScores);

                hasSavedScore = true;
            }
        }

        if (classicScript)
        {
            if (classicScript.EndGame && !hasSavedScore)
            {
                LoseScreen.SetActive(true);
                timerText.text = "Czas gry: " + statsController.timerText.text;
                pointsText.text = "Wynik: " + statsController.pointsText.text;

                // Odczytaj najlepsze wyniki z pliku (jeœli istniej¹).
                BestScores bestScores = LoadBestScores();


                // Dodaj nowy wynik do listy najlepszych wyników.
                bestScores.scores.Add(new ScoreData(statsController.pointsText.text, statsController.timerText.text, "Classic"));

                // Sortuj wyniki od najlepszego do najgorszego.
                bestScores.scores.Sort((x, y) => y.Score.CompareTo(x.Score));

                // Ogranicz listê do np. 10 najlepszych wyników (lub dowolnej liczby).
                if (bestScores.scores.Count > 10)
                {
                    bestScores.scores.RemoveRange(10, bestScores.scores.Count - 10);
                }

                // Zapisz zaktualizowane wyniki do pliku.
                SaveBestScores(bestScores);

                hasSavedScore = true;
            }
        }
    }

    // Odczyt najlepszych wyników z pliku (lub utworzenie nowego obiektu, jeœli plik nie istnieje).
    private BestScores LoadBestScores()
    {
        BestScores bestScores = new();
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            bestScores = JsonUtility.FromJson<BestScores>(json);
        }
        return bestScores;
    }

    // Zapis najlepszych wyników do pliku.
    private void SaveBestScores(BestScores bestScores)
    {
        string json = JsonUtility.ToJson(bestScores);
        File.WriteAllText(saveFilePath, json);
    }

    /// <summary>
    /// Ponowne rozpoczêcie gry po klikniêciu przycisku restart.
    /// Resetuje licznik czasu i punkty oraz ukrywa ekran przegranej.
    /// </summary>
    public void RestartButton()
    {
        if(sandScript)
            sandScript.RestartGame();
        if(classicScript)
            classicScript.RestartGame();
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

// Klasa przechowuj¹ca dane o wynikach.
[System.Serializable]
public class ScoreData
{
    public string Points;
    public string Time;
    public string Mode;
    public float Score;


    public ScoreData(string points, string time, string mode)
    {
        Points = points;
        Time = time;
        Mode = mode;
        Score = CalculateScore(points, time);
    }

    private float CalculateScore(string points, string time)
    {
        float scoreValue = float.Parse(points);
        string timeString = time;
        int seconds = 0;

        if (timeString.Contains(":"))
        {
            string[] timeParts = timeString.Split(':');
            if (timeParts.Length == 2)
            {
                int minutes = int.Parse(timeParts[0]);
                seconds = int.Parse(timeParts[1]);
                seconds += minutes * 60;
            }
        }

        return seconds > 0 ? scoreValue / seconds : 0f;
    }
}

// Klasa przechowuj¹ca listê najlepszych wyników.
[System.Serializable]
public class BestScores
{
    public List<ScoreData> scores = new();
}