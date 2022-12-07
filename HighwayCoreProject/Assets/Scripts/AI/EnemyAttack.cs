using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyBehaviour
{
    public float MaxRotateSpeed, Lead;

    void Update()
    {
        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        Vector3 targetDirection = enemy.targetPlayer.trailPosition + (enemy.targetPlayer.position - enemy.targetPlayer.trailPosition) * Lead - enemy.Head.position;
        enemy.Head.rotation = Quaternion.LookRotation(Vector3.RotateTowards(enemy.Head.forward, new Vector3(targetDirection.x, targetDirection.y, targetDirection.z), MaxRotateSpeed * Time.deltaTime, 0f));
    }

    void Rotate(){
        Vector3 targetDirection = enemy.targetPlayer.trailPosition + (enemy.targetPlayer.position - enemy.targetPlayer.trailPosition) * Lead - enemy.Head.position;

        Quaternion rotate = Quaternion.LookRotation(new Vector3(targetDirection.x, targetDirection.y, targetDirection.z));
        enemy.Head.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime*5f);
    }
}
