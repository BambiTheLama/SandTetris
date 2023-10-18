using UnityEngine;
using TMPro;

public class StatsController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float currentTime = 0.0f;
    private bool isCounting = false;

    public TextMeshProUGUI pointsText;
    private int points = 0;

    private void Update()
    {
        if (isCounting)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
        UpdatePointsDisplay();

    }

    public void StartTimer()
    {
        isCounting = true;
    }

    public void StopTimer()
    {
        isCounting = false;
    }

    public void ResetTimer()
    {
        currentTime = 0.0f;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(currentTime);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddPoints(int amount)
    {
        points += amount;
        UpdatePointsDisplay();
    }

    private void UpdatePointsDisplay()
    {
        if (pointsText != null)
        {
            pointsText.text = points.ToString();
        }
    }
    public void ResetPoints()
    {
        points = 0; 
    }
}
