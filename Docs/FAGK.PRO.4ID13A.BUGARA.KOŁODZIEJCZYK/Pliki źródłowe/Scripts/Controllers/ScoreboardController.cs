using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using TMPro;


/// <summary>
/// Klasa kontrolująca wyświetlanie punktacji.
/// </summary>
public class ScoreBoardController : MonoBehaviour
{
    public GameObject wynikTemplatePrefab; // Prefab obiektu zawierającego dane wyniku.
    public Transform wynikiListParent; // Obiekt nadrzędny dla wyników.
    readonly string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "best_scores.json");
    public Button wynikiButton;

    private void Start()
    {
        LoadScoresFromFile();
    }


    /// <summary>
    /// Wczytywanie punktacji z pliku
    /// </summary>
    private void LoadScoresFromFile()
    {
        if (File.Exists(saveFilePath))
        {
            wynikiButton.interactable = true;
            // Odczytaj dane z pliku.
            string json = File.ReadAllText(saveFilePath);
            BestScores bestScores = JsonUtility.FromJson<BestScores>(json);


            // Wyświetl wyniki w UI.
            int position = 1;
            float offsetY = 0f; // Inicjalizacja odstępu.

            foreach (ScoreData scoreData in bestScores.scores)
            {
                GameObject wynikTemplate = Instantiate(wynikTemplatePrefab, wynikiListParent);
                wynikTemplate.SetActive(true); // Upewnij się, że jest aktywny.

                // Przypisz teksty do odpowiednich pól.
                TextMeshProUGUI placeText = wynikTemplate.transform.Find("PlaceText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI pointsText = wynikTemplate.transform.Find("PointsText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI timeText = wynikTemplate.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = wynikTemplate.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI modeText = wynikTemplate.transform.Find("ModeText").GetComponent<TextMeshProUGUI>();

                // Ustaw pozycję wyniku z odstępem 100 pikseli.
                RectTransform rectTransform = wynikTemplate.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, offsetY);

                // Przypisanie wartości "Score" i "Time" do zmiennych liczbowych.
                float scoreValue = float.Parse(scoreData.Points);
                string timeString = scoreData.Time;
                int seconds = 0;

                // Próba konwersji ciągu "Time" na liczbę całkowitą reprezentującą sekundy.
                if (timeString.Contains(":"))
                {
                    string[] timeParts = timeString.Split(':');
                    if (timeParts.Length == 2)
                    {
                        int minutes = int.Parse(timeParts[0]);
                        seconds = int.Parse(timeParts[1]);
                        seconds += minutes * 60; // Zamiana minut na sekundy.
                    }
                }
                float scorePerSecond = seconds > 0 ? scoreValue / seconds : 0f;
                float roundedScorePerSecond = (float)Math.Round(scorePerSecond, 2);


                placeText.text = position.ToString();
                pointsText.text = scoreData.Points;
                timeText.text = scoreData.Time;
                scoreText.text = roundedScorePerSecond.ToString();
                modeText.text = scoreData.Mode;

                position++;

                offsetY -= 80f;

            }
        }
        else
        {
            wynikiButton.interactable = false;
        }
    }
}
