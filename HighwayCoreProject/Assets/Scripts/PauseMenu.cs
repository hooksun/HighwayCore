using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    bool isPaused = false;
    public GameObject PauseUI;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                Resume();
            }
            else if(!isPaused){
                Pause();
            }
        }
    }

    public void Pause(){
        Time.timeScale = 0f;
        PauseUI.SetActive(true);
        isPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Resume(){
        Time.timeScale = 1f;
        PauseUI.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitToMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
}
