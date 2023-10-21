using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{

    [Header("Ekrany")]
    public GameObject PauseScreen;
    public bool paused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                paused = true;
                PauseScreen.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                paused= false;
                PauseScreen.SetActive(false); 
                Time.timeScale = 1;
            }

        }


    }
    public void ContinueButton()
    {
        paused = false;
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
    }
    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
