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
    public float animationAnimLength = 1f;

    
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
        if(health == 0f)
            return;
        health-=damage;
        health = Mathf.Clamp(health, 0f, playerData.maxHealth);
        UIManager.SetHealth(health);
        if(health ==0){
            player.Die();
        }
    }

    public override void Die()
    {
        //ded
        Invoke("ShowDeathMenu", animationAnimLength);
    }

    public void ShowDeathMenu(){
        UIManager.SetDeathMenu(true);
        showCursor();
        Time.timeScale = 0f;
    }

    void showCursor(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
