using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIScript : MonoBehaviour
{
    public AIData agentData;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask playerMask;
    private GameObject player;
    public GameObject projectile;
    public Transform projectileSpawner;
    public float ReadyTime;

    //PLAYER CHECKER
    private bool playerInChaseRange;
    private bool playerInAttackRange;
    
    void Start()
    {
        player = GameObject.Find("Player");
        agentData.readyToFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        playerInChaseRange = Physics.CheckSphere(transform.position, agentData.chaseRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, agentData.attackRange, playerMask);

        if(playerInChaseRange && !playerInAttackRange){
            chase();
        }
        else if(playerInAttackRange){
            standStill();

            attacking();
        }
    }

    void attacking(){
        //WRITE CODE
        if(agentData.readyToFire){
            Instantiate(projectile, projectileSpawner.transform.position, transform.rotation);
            agentData.readyToFire = false;

            StartCoroutine(Wait());
        }
    }
    IEnumerator Wait(){
        yield return new WaitForSeconds(agentData.fireRate);

        agentData.readyToFire = true;
    }

    void standStill()
    {
        agent.SetDestination(transform.position);    
        lookAtPlayer();
    }

    void chase(){
        agent.SetDestination(player.transform.position);
    }

    void lookAtPlayer(){
        Vector3 lookTo = (player.transform.position - transform.position).normalized;
        Quaternion rotate = Quaternion.LookRotation(new Vector3(lookTo.x, 0, lookTo.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime);
    }
}
