using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using TMPro;

public class ScoreBoardController : MonoBehaviour
{
    public GameObject wynikTemplatePrefab; // Prefab obiektu zawieraj¹cego dane wyniku.
    public Transform wynikiListParent; // Obiekt nadrzêdny dla wyników.
    readonly string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "best_scores.json");
    public Button wynikiButton;

    private void Start()
    {
        LoadScoresFromFile();
    }

    private void LoadScoresFromFile()
    {
        if (File.Exists(saveFilePath))
        {
            wynikiButton.interactable = true;
            // Odczytaj dane z pliku.
            string json = File.ReadAllText(saveFilePath);
            BestScores bestScores = JsonUtility.FromJson<BestScores>(json);


            // Wyœwietl wyniki w UI.
            int position = 1;
            float offsetY = 0f; // Inicjalizacja odstêpu.

            foreach (ScoreData scoreData in bestScores.scores)
            {
                GameObject wynikTemplate = Instantiate(wynikTemplatePrefab, wynikiListParent);
                wynikTemplate.SetActive(true); // Upewnij siê, ¿e jest aktywny.

                // Przypisz teksty do odpowiednich pól.
                TextMeshProUGUI placeText = wynikTemplate.transform.Find("PlaceText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI pointsText = wynikTemplate.transform.Find("PointsText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI timeText = wynikTemplate.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = wynikTemplate.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI modeText = wynikTemplate.transform.Find("ModeText").GetComponent<TextMeshProUGUI>();

                // Ustaw pozycjê wyniku z odstêpem 100 pikseli.
                RectTransform rectTransform = wynikTemplate.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, offsetY);

                // Przypisanie wartoœci "Score" i "Time" do zmiennych liczbowych.
                float scoreValue = float.Parse(scoreData.Points);
                string timeString = scoreData.Time;
                int seconds = 0;

                // Próba konwersji ci¹gu "Time" na liczbê ca³kowit¹ reprezentuj¹c¹ sekundy.
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
