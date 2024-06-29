using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class Menymanager : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();
    }
        public void Gotogame(){
        SceneManager.LoadScene(1); 
    }
}
