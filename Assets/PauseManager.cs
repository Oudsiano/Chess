using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] public GameObject restartPanel;
    [SerializeField] public TMP_Text infoWinner;

    public void Gotomenyscene()
    {
        SceneManager.LoadScene(0);
    }

    public void Exitgame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // Pausing the game   
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // Resuming the game 
    }

    public void ReastartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DisplayEndGame(string winner)
    {
        infoWinner.text = winner;
        restartPanel.SetActive(true);
    }
}
