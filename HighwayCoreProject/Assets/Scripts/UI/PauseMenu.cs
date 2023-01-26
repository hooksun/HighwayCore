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
        if(!Player.ActivePlayer.dead && Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                Resume();
            }
            else if(!isPaused){
                Pause();
            }
        }
    }

    public void Pause(){
        PauseUI.SetActive(true);
        isPaused = true;

        FreezeTime(true);
    }

    public void Resume(){
        PauseUI.SetActive(false);
        isPaused = false;

        FreezeTime(false);
    }

    public static void FreezeTime(bool freeze)
    {
        AudioPlayer.PauseAll(freeze);
        Player.ActivePlayer.EnableInput(!freeze);
        if(freeze)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitToMainMenu(){
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
}
