using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScripts : MonoBehaviour
{
    public Slider slider;
    public PlayerMovement playerMovement;
    
    void Update()
    {
        JetpackFuelUI();
    }
    void JetpackFuelUI(){
        slider.value = playerMovement.currentFuel;
    }
}
