using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="AI", menuName ="NPC/AI")]
public class AIData : ScriptableObject
{
    public string name_;
    public float fireRate; // seconds/shot
    public float movementSpeed;
    public int shootingBurst; //Berapa banyak tembakan tiap kali serang
    public bool readyToFire;

    public float chaseRange;
    public float attackRange;
}
