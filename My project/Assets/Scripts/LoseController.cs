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

    // Start is called before the first frame update
    void Start()
    {

    }

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
        //gameScript.RestartGame();
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
