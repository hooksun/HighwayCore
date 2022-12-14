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
        shootTime = shootShotgunTime;
    }

    bool toggle;
    void Update()
    {
        if(shootTime <= 0f)
        {
            // if(toggle)
            //     anim.Play("metarig|forward1", .2f);
            // else
            //     StartCoroutine(JumpTest());
            shootTime = shootShotgunTime;
            test++;
            toggle = !toggle;
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

        //enemy.Head.rotation = Quaternion.LookRotation(lookDir, transform.up);
        anim.SetLook(lookDir);
        anim.SetMove(moveDir);
    }

    IEnumerator JumpTest()
    {
        anim.Play("metarig|Jump", 0.2f);
        yield return new WaitForSeconds(0.4f);
        anim.Play("Enemy Midair", 0.2f);
    }
}
