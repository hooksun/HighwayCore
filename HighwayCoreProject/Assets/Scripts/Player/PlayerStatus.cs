using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatus : PlayerBehaviour
{
    float health;
    public PlayerData playerData;
    public TextMeshProUGUI healthPointTxt;
    public GameObject deathMenu;
    public bool isFallen = false;
    float animationAnimLength = 4f;

    
    void Start()
    {
        health = playerData.maxHealth;
        UIManager.SetHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerHealthCondition();
    }
    void PlayerHealthCondition(){
        //healthPointTxt.SetText("HP : " + health);
        
    }

    public void TakeDamage(float damage){
        health-=damage;
        UIManager.SetHealth(health);
        if(health <=0){
            health = 0f;
            player.Die();
        }
    }

    public override void Die()
    {
        //ded
        Invoke("ShowDeathMenu", 2f);
    }

    public void ShowDeathMenu(){
        deathMenu.SetActive(true);
        showCursor();
        Time.timeScale = 0f;
    }

    void showCursor(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
