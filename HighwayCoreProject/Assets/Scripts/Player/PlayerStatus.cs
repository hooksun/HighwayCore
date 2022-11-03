using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    float health;
    public PlayerData playerData;
    public TextMeshProUGUI healthPointTxt;

    public Transform groundChecker;
    private bool fallen;
    bool takenDamage = false;
    void Start()
    {
        health = playerData.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerHealthCondition();
    }
    void PlayerHealthCondition(){
        healthPointTxt.SetText("HP : " + health);
        if(health <=0){
            //DED
        }
    }

    void PlayerTakeDamage(){
        health-=20f;
    }
    void ReadyToTakeDamage(){
        takenDamage = false;
    }
}
