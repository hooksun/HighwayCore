using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatus : PlayerBehaviour
{
    float health;
    public PlayerData playerData;
    public TextMeshProUGUI healthPointTxt;

    public Transform groundChecker;
    private bool fallen;
    bool takenDamage;
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
            player.Die();
        }
    }

    void PlayerTakeDamage(){
        health-=20f;
    }
    void ReadyToTakeDamage(){
        //takenDamage = false;
    }

    public override void Die()
    {
        //ded
    }
}
