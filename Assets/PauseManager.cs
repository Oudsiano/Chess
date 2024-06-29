using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseManager : MonoBehaviour
{
    public void Gotomenyscene() {
        SceneManager.LoadScene(0);
    }
    public void Exitgame() {
        Application.Quit();
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // ��������� �������   
    }

    public void ResumeGame()
    {

        Time.timeScale = 1; // ������� ������� � ����������� �������

    }
}
