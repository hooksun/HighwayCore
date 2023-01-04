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
        Invoke("ShowDeathMenu", 2f);
    }

    public void ShowDeathMenu(){
        deathMenu.SetActive(true);
        showCursor();
        Time.timeScale = 0f;
    }

    void showCursor(){
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
