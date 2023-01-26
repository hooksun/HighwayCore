using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public PlayerSettings settings;
    void Awake()
    {
        settings.Load();
        SaveSystem.settings = settings;
    }

    public void PlayGame(){
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1f;
    }

    public void QuitGame(){
        Application.Quit();
    }
}
