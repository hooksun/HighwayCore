using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : EnemyBehaviour
{
    public Animator animator;
    public Transform Waist;
    public Transform[] Spine;
    public string walkAnim;
    public string[] MoveAnimations;

    Vector3 MoveDirection, LookDirection, WaistDirection;
    string waistAnim;

    public override void Activate()
    {
        MoveDirection = Vector3.forward;
        LookDirection = Vector3.forward;
        WaistDirection = Vector3.forward;
        //animator.Play(walkAnim);
    }

    public void SetLook(Vector3 dir)
    {
        dir.y = 0f;
        LookDirection = dir.normalized;
    }
    public void SetMove(Vector3 dir)
    {
        dir.y = 0f;
        MoveDirection = dir.normalized;
    }

    void CalculateDirection()
    {
        Vector3 dir = MoveDirection;
        WaistDirection = dir;
        float dot = Vector3.Dot(LookDirection, dir);
        waistAnim = MoveAnimations[0];
        for(int i = 1; i < 4; i++)
        {
            //rotate clockwise
            float buffer = dir.x;
            dir.x = dir.z;
            dir.z = -buffer;
            float newDot = Vector3.Dot(LookDirection, dir);
            if(newDot > dot)
            {
                dot = newDot;
                WaistDirection = dir;
                waistAnim = MoveAnimations[i];
            }
        }
    }
}
