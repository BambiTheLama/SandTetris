using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa od splash screena
/// </summary>
public class SplashScreenController : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}