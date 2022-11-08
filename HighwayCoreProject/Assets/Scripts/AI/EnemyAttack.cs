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
        enemy.Head.rotation = Quaternion.LookRotation(Vector3.RotateTowards(enemy.Head.forward, targetDirection, MaxRotateSpeed * Time.deltaTime, 0f));
    }
}
