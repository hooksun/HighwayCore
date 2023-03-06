using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    public MainMenu mainMenu;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            mainMenu.PlayGame();
        }
    }
}
