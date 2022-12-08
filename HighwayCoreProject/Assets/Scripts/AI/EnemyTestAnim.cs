using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestAnim : MonoBehaviour
{
    public EnemyAnimation animation;
    public Enemy enemy;

    public float moveSpeed, lookSpeed, vertSpeed, vertSwitchTime;
    Vector3 moveDir, lookDir, vertTarget;
    float vertTime;

    void Start()
    {
        moveDir = Vector3.forward;
        lookDir = Vector3.forward;
        vertTime = vertSwitchTime * 0.5f;
        vertTarget = Vector3.up;
        animation.enemy = enemy;
        animation.Activate();
    }

    void Update()
    {
        moveDir = Vector3.RotateTowards(moveDir, Vector3.Cross(moveDir, Vector3.up), moveSpeed * Time.deltaTime, 0f);
        lookDir = Vector3.RotateTowards(lookDir, Vector3.Cross(lookDir, Vector3.up), lookSpeed * Time.deltaTime, 0f);
        lookDir = Vector3.RotateTowards(lookDir, vertTarget, vertSpeed * Time.deltaTime, 0f);
        vertTime -= Time.deltaTime;
        if(vertTime <= 0)
        {
            vertTime = vertSwitchTime;
            vertTarget.y *= -1f;
        }

        enemy.Head.rotation = Quaternion.LookRotation(lookDir);
        animation.SetLook(lookDir);
        //animation.SetMove(moveDir);
    }
}
