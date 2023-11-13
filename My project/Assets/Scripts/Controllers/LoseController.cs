using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

/// <summary>
/// Klasa kontrolująca zachowanie po przegranej.
/// </summary>
public class LoseController : MonoBehaviour
{
    /// <summary>
    /// TextMeshProUGUI do wyświetlania czasu gry.
    /// </summary>
    public TextMeshProUGUI timerText;

    /// <summary>
    /// TextMeshProUGUI do wyświetlania ilości zdobytych  punktów.
    /// </summary>
    public TextMeshProUGUI pointsText;

    /// <summary>
    /// GameObject reprezentujący ekran przegranej.
    /// </summary>
    public GameObject LoseScreen;

    /// <summary>
    /// Referencja do GameScript do zarządzania grą.
    /// </summary>
    public GameScript sandScript;
    public NormalTetrisScript classicScript;
    public ElementalsTetrisScript elementalScript;

    /// <summary>
    /// Referencja do StatsController do zarządzania statystykami gry.
    /// </summary>
    public StatsController statsController;
    readonly string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "best_scores.json");
    public bool hasSavedScore = false; // Flaga oznaczająca, czy wynik został już zapisany.


    /// <summary>
    /// Metoda Update wywoływana raz na klatkę.
    /// Sprawdza, czy gra się zakończyła i wyświetla ekran przegranej ze statystykami.
    /// </summary>
    private void Update()
    {
        if (sandScript)
        {
            if (sandScript.EndGame)
            {
                LoseScreen.SetActive(true);
                timerText.text = "Czas gry: " + statsController.timerText.text;
                pointsText.text = "Wynik: " + statsController.pointsText.text;
                if (!hasSavedScore)
                {
                    // Odczytaj najlepsze wyniki z pliku (jeśli istnieją).
                    BestScores bestScores = LoadBestScores();


                    // Dodaj nowy wynik do listy najlepszych wyników.
                    bestScores.scores.Add(new ScoreData(statsController.pointsText.text, statsController.timerText.text, "Sand"));

                    // Sortuj wyniki od najlepszego do najgorszego.
                    bestScores.scores.Sort((x, y) => y.Score.CompareTo(x.Score));

                    // Ogranicz listę do np. 10 najlepszych wyników (lub dowolnej liczby).
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

        if (classicScript)
        {
            if (classicScript.EndGame)
            {
                LoseScreen.SetActive(true);
                timerText.text = "Czas gry: " + statsController.timerText.text;
                pointsText.text = "Wynik: " + statsController.pointsText.text;
                if (!hasSavedScore)
                {
                    // Odczytaj najlepsze wyniki z pliku (jeśli istnieją).
                    BestScores bestScores = LoadBestScores();


                    // Dodaj nowy wynik do listy najlepszych wyników.
                    bestScores.scores.Add(new ScoreData(statsController.pointsText.text, statsController.timerText.text, "Classic"));

                    // Sortuj wyniki od najlepszego do najgorszego.
                    bestScores.scores.Sort((x, y) => y.Score.CompareTo(x.Score));

                    // Ogranicz listę do np. 10 najlepszych wyników (lub dowolnej liczby).
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

        if (elementalScript)
        {
            if (elementalScript.EndGame)
            {
                LoseScreen.SetActive(true);
                timerText.text = "Czas gry: " + statsController.timerText.text;
                pointsText.text = "Wynik: " + statsController.pointsText.text;

                if (!hasSavedScore)
                {
                    // Odczytaj najlepsze wyniki z pliku (jeśli istnieją).
                    BestScores bestScores = LoadBestScores();


                    // Dodaj nowy wynik do listy najlepszych wyników.
                    bestScores.scores.Add(new ScoreData(statsController.pointsText.text, statsController.timerText.text, "Elementals"));

                    // Sortuj wyniki od najlepszego do najgorszego.
                    bestScores.scores.Sort((x, y) => y.Score.CompareTo(x.Score));

                    // Ogranicz listę do np. 10 najlepszych wyników (lub dowolnej liczby).
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
    }

    /// <summary>
    /// Odczyt najlepszych wyników z pliku (lub utworzenie nowego obiektu, jeśli plik nie istnieje).
    /// </summary>
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

    /// <summary>
    /// Zapis najlepszych wyników do pliku.
    /// </summary>
    private void SaveBestScores(BestScores bestScores)
    {
        string json = JsonUtility.ToJson(bestScores);
        File.WriteAllText(saveFilePath, json);
    }

    /// <summary>
    /// Ponowne rozpoczęcie gry po kliknięciu przycisku restart.
    /// Resetuje licznik czasu i punkty oraz ukrywa ekran przegranej.
    /// </summary>
    public void RestartButton()
    {
        if(sandScript)
            sandScript.RestartGame();
        if(classicScript)
            classicScript.RestartGame();
        if (elementalScript)
            elementalScript.RestartGame();
        statsController.ResetTimer();
        statsController.ResetPoints();
        LoseScreen.SetActive(false);
    }

    /// <summary>
    /// Powrót do menu głównego po kliknięciu przycisku wyjścia.
    /// </summary>
    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    } 
}

/// <summary>
/// Klasa przechowująca dane o wynikach.
/// </summary>
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

    /// <summary>
    /// Przelicznik wyników
    /// </summary>
    private float CalculateScore(string points, string time)
    {
        if (float.TryParse(points, out float scoreValue))
        {
            string timeString = time;
            int seconds = 0;

            if (timeString.Contains(":"))
            {
                string[] timeParts = timeString.Split(':');
                if (timeParts.Length == 2)
                {
                    int minutes = 0;
                    if (int.TryParse(timeParts[0], out minutes) && int.TryParse(timeParts[1], out seconds))
                    {
                        seconds += minutes * 60;
                        return seconds > 0 ? scoreValue / seconds : 0f;
                    }
                }
            }
        }

        // Obsługa błędnego formatu danych wejściowych.
        return 0f;
    }

}

/// <summary>
/// Klasa przechowująca listę najlepszych wyników.
/// </summary>
[System.Serializable]
public class BestScores
{
    public List<ScoreData> scores = new();
}