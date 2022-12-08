using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : EnemyBehaviour
{
    public Animator animator;
    public Transform[] spine;
    public float waistSpeed, maxWaistAngle;
    public string walkAnim;
    public string idleAnim;
    public string[] MoveAnimations;

    Vector3 moveDirection, lookDirection, horizLook, waistDirection, waistDir;
    string waistAnim;

    public override void Activate()
    {
        SetLook(Vector3.forward);
        SetMove(Vector3.zero);
        waistDirection = horizLook;
        waistDir = waistDirection;
    }

    public void SetLook(Vector3 dir)
    {
        lookDirection = dir;
        dir.y = 0f;
        horizLook = dir.normalized;
    }
    public void SetMove(Vector3 dir)
    {
        dir.y = 0f;
        moveDirection = dir;
    }

    void Update()
    {
        CalculateDirection();

        waistDir = Vector3.RotateTowards(waistDir, waistDirection, waistSpeed * Time.deltaTime, 0f);
        if(moveDirection == Vector3.zero && waistDir == waistDirection)
            waistAnim = idleAnim;

        

        animator.Update(Time.deltaTime);
        float step = 1f/(spine.Length - 1);
        Vector3[] rotateOffset = new Vector3[spine.Length];
        for(int i = 0; i < spine.Length; i++)
        {
            rotateOffset[i] = spine[i].rotation.eulerAngles;
        }
        for(int i = 0; i < spine.Length; i++)
        {
            Vector3 newRot = (i==3?lookDirection:Vector3.Slerp(waistDir, horizLook, step * (float)i));
            spine[i].rotation = Quaternion.LookRotation(newRot);
            spine[i].Rotate(rotateOffset[i]);
        }
    }

    void CalculateDirection()
    {
        waistAnim = MoveAnimations[0];
        if(moveDirection == Vector3.zero)
        {
            if(Vector3.Dot(waistDirection, horizLook) < Mathf.Cos(Mathf.Deg2Rad * maxWaistAngle))
                waistDirection = horizLook;
            return;
        }

        Vector3 dir = moveDirection;
        waistDirection = dir;
        float dot = Vector3.Dot(horizLook, dir);
        for(int i = 1; i < 4; i++)
        {
            //rotate clockwise
            float buffer = dir.x;
            dir.x = dir.z;
            dir.z = -buffer;
            float newDot = Vector3.Dot(horizLook, dir);
            if(newDot > dot)
            {
                dot = newDot;
                waistDirection = dir;
                waistAnim = MoveAnimations[i];
            }
        }
    }
}