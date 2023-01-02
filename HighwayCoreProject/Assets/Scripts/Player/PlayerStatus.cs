using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatus : PlayerBehaviour
{
    float health;
    public PlayerData playerData;
    public TextMeshProUGUI healthPointTxt;
    public bool isFallen = false;

    
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
        //healthPointTxt.SetText("HP : " + health);
        UIManager.SetHealth(health);
        if(health <=0){
            player.Die();
        }
        
    }

    public void TakeDamage(float damage){
        health-=damage;
    }

    public override void Die()
    {
        //ded
    }
}
