using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestAnim : MonoBehaviour
{
    public EnemyAnimation anim;
    public Enemy enemy;

    public float moveSpeed, lookSpeed, vertSpeed, vertSwitchTime, shootShotgunTime;
    Vector3 moveDir, lookDir, vertTarget;
    float vertTime, shootTime;
    int test = 0;

    void OnEnable()
    {
        moveDir = Vector3.forward;
        lookDir = Vector3.forward;
        vertTime = vertSwitchTime * 0.5f;
        vertTarget = Vector3.up;
        anim.enemy = enemy;
        anim.Activate();
    }

    void Update()
    {
        if(shootTime <= 0f)
        {
            anim.Play("shoot_shotgun");
            shootTime = shootShotgunTime;
            test++;
        }
        shootTime -= Time.deltaTime;

        moveDir = Vector3.RotateTowards(moveDir, Vector3.Cross(moveDir, Vector3.up), moveSpeed * Time.deltaTime, 0f);
        lookDir = Vector3.RotateTowards(lookDir, Vector3.Cross(lookDir, Vector3.up), lookSpeed * Time.deltaTime, 0f);
        lookDir = Vector3.RotateTowards(lookDir, vertTarget, vertSpeed * Time.deltaTime, 0f);
        vertTime -= Time.deltaTime;
        if(vertTime <= 0)
        {
            vertTime = vertSwitchTime;
            vertTarget.y *= -1f;
        }

        enemy.Head.rotation = Quaternion.LookRotation(lookDir, transform.up);
        anim.SetLook(lookDir);
        anim.SetMove(moveDir);
    }
}
