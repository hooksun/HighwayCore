using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : PlayerBehaviour
{
    float health;
    public PlayerData playerData;
    public float animationAnimLength = 1f;

    
    void Start()
    {
        health = playerData.maxHealth;
        UIManager.SetHealth(health);
    }

    public void TakeDamage(float damage){
        health-=damage;
        health = Mathf.Clamp(health, 0f, playerData.maxHealth);
        UIManager.SetHealth(health);
        if(player.dead && health > 0f)
            player.Die(false);
        if(health ==0){
            player.Die();
        }
    }

    public void Die()
    {
        //ded
        Invoke("ShowDeathMenu", animationAnimLength);
    }

    public void ShowDeathMenu(){
        if(!player.dead)
            return;
        player.SetScore();
        UIManager.SetDeathButtons(true);
        PauseMenu.FreezeTime(true);
    }

    void showCursor(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
