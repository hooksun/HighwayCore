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
    void Start()
    {
        health = playerData.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(groundChecker.position, Vector3.down, out hit, .2f)){
            if(hit.collider.name == "Road"){
                fallen = true;
            }
            else{
                fallen = false;
            }
        }
        PlayerHealthCondition();
        PlayerFell();
    }
    void PlayerHealthCondition(){
        healthPointTxt.SetText("HP : " + health);
        if(health <=0){
            //DED
        }
    }

    bool takenDamage = false;
    void PlayerFell(){
        if(fallen && !takenDamage){
            PlayerTakeDamage();
            takenDamage = true;

            Invoke("ReadyToTakeDamage", 1f);
        }
    }

    void PlayerTakeDamage(){
        health-=20f;
    }
    void ReadyToTakeDamage(){
        takenDamage = false;
    }
}
