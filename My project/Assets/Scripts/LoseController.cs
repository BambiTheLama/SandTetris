using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseController : MonoBehaviour
{

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI pointsText;
    public GameObject LoseScreen;
    public GameScript gameScript;
    public StatsController statsController;


    private void Update()
    {
        if (gameScript.endGame)
        { 
            LoseScreen.SetActive(true);
            
            timerText.text = "Czas gry: " + statsController.timerText.text;
            pointsText.text = "Wynik: " + statsController.pointsText.text;
        }
    }

    public void RestartButton()
    {
        gameScript.RestartGame();
        statsController.ResetTimer();
        statsController.ResetPoints();
        LoseScreen.SetActive(false);
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
