using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : EnemyBehaviour
{
    public Animator animator;
    public Transform[] spine;
    public float waistSpeed, maxWaistAngle;
    public string idleAnim;
    public string[] MoveAnimations;
    public float animationsLength;

    Vector3 moveDirection, lookDirection, horizLook, waistDirection, waistDir;
    string waistAnim, currentAnim;
    bool playing;
    float walkCycle;

    public override void Activate()
    {
        SetLook(Vector3.forward);
        SetMove(Vector3.zero);
        waistDirection = horizLook;
        waistDir = waistDirection;
        Reset();
    }

    public void Play(string anim, float fadeTime = 0f)
    {
        animator.CrossFadeInFixedTime(anim, fadeTime);
        playing = true;
    }

    public void Reset(float fadeTime = 0f)
    {
        playing = false;
        currentAnim = "";
        walkCycle = 0f;
    }

    public void SetLook(Vector3 dir)
    {
        if(dir == Vector3.zero)
            return;

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

        waistDir = Vector3.RotateTowards(waistDir, waistDirection, waistSpeed * Time.deltaTime, 1f);
        if(moveDirection == Vector3.zero && waistDir == waistDirection)
            waistAnim = idleAnim;

        if(!playing && waistAnim != currentAnim)
        {
            currentAnim = waistAnim;
            animator.CrossFadeInFixedTime(waistAnim, 0.2f);
        }
        walkCycle = (walkCycle + Time.deltaTime) % animationsLength;

        animator.Update(Time.deltaTime);
        float step = 1f/(spine.Length - 1);
        Quaternion[] rotateOffset = new Quaternion[spine.Length];
        Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);
        for(int i = 0; i < spine.Length; i++)
        {
            rotateOffset[i] = inverseRotation * spine[i].rotation;
        }
        for(int i = 0; i < spine.Length-1; i++)
        {
            Vector3 newRot = Vector3.Slerp(waistDir, horizLook, step * (float)i);
            spine[i].rotation = transform.rotation * Quaternion.LookRotation(newRot) * rotateOffset[i];
        }
        spine[spine.Length-1].rotation = Quaternion.LookRotation(lookDirection, transform.up) * rotateOffset[spine.Length-1];
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