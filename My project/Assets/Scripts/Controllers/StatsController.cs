using UnityEngine;
using TMPro;

/// <summary>
/// Klasa kontroluj¹ca statystyki gry, w tym czas gry i punkty gracza.
/// </summary>
public class StatsController : MonoBehaviour
{
    /// <summary>
    /// TextMeshProUGUI do wyœwietlania czasu gry.
    /// </summary>
    public TextMeshProUGUI timerText;

    private float currentTime = 0.0f;
    private bool isCounting = false;

    /// <summary>
    /// TextMeshProUGUI do wyœwietlania iloœci punktów gracza.
    /// </summary>
    public TextMeshProUGUI pointsText;

    private int points = 0;

    /// <summary>
    /// Metoda Update wywo³ywana raz na klatkê.
    /// Aktualizuje czas gry i wyœwietlanie punktów gracza.
    /// </summary>
    private void Update()
    {
        if (isCounting)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
        UpdatePointsDisplay();
    }

    /// <summary>
    /// Rozpoczyna liczenie czasu.
    /// </summary>
    public void StartTimer()
    {
        isCounting = true;
    }

    /// <summary>
    /// Zatrzymuje liczenie czasu.
    /// </summary>
    public void StopTimer()
    {
        isCounting = false;
    }

    /// <summary>
    /// Resetuje licznik czasu.
    /// </summary>
    public void ResetTimer()
    {
        currentTime = 0.0f;
        UpdateTimerDisplay();
    }

    /// <summary>
    /// Aktualizuje wyœwietlanie czasu gry.
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(currentTime);
        }
    }

    /// <summary>
    /// Formatuje czas gry.
    /// </summary>
    /// <param name="timeInSeconds">Czas w sekundach.</param>
    /// <returns>Sformatowany czas w postaci "mm:ss".</returns>
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Dodaje punkty do punktacji gracza.
    /// </summary>
    /// <param name="amount">Iloœæ punktów do dodania.</param>
    public void AddPoints(int amount)
    {
        points += amount;
        UpdatePointsDisplay();
    }

    /// <summary>
    /// Aktualizuje wyœwietlanie iloœci punktów gracza.
    /// </summary>
    private void UpdatePointsDisplay()
    {
        if (pointsText != null)
        {
            pointsText.text = points.ToString();
        }
    }

    /// <summary>
    /// Resetuje iloœæ punktów gracza.
    /// </summary>
    public void ResetPoints()
    {
        points = 0;
    }
}
