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
    public float animationsLength, crossFadeDuration;

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
        currentAnim = "";
        Reset();
    }

    public void Play(string anim, int layer, float fadeTime = 0f)
    {
        if(layer != 0)
        {
            animator.CrossFadeInFixedTime(anim, fadeTime, layer);
            return;
        }
        playing = true;
        if(anim == currentAnim)
            return;
        animator.CrossFadeInFixedTime(anim, fadeTime, layer);
        currentAnim = anim;
    }

    public void PlayIdle(float fadeTime = -1f) => Play(idleAnim, 0, (fadeTime<0?crossFadeDuration:fadeTime));

    public void Reset()
    {
        playing = false;
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
        CalculateDirections();

        waistDir = Vector3.RotateTowards(waistDir, waistDirection, waistSpeed * Time.deltaTime, 1f);
        float cos = Mathf.Cos(Mathf.Deg2Rad * maxWaistAngle);
        if(Vector3.Dot(waistDir, horizLook) < cos)
        {
            Vector3 ortho = Vector3.Cross(Vector3.up, horizLook);
            if(Vector3.Dot(ortho, waistDir) < 0f)
                ortho *= -1f;
            
            waistDir = horizLook * cos + ortho * Mathf.Sin(Mathf.Deg2Rad * maxWaistAngle);
        }
        if(moveDirection == Vector3.zero && waistDir == waistDirection)
        {
            waistAnim = idleAnim;
            walkCycle = 0f;
        }
        if(moveDirection != Vector3.zero)
        {
            BestDirection(waistDir, moveDirection, out int i);
            waistAnim = MoveAnimations[i];
        }

        if(!playing && waistAnim != currentAnim)
        {
            currentAnim = waistAnim;
            animator.CrossFadeInFixedTime(waistAnim, crossFadeDuration, -1, walkCycle);
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

    void CalculateDirections()
    {
        waistAnim = MoveAnimations[0];
        if(moveDirection == Vector3.zero)
        {
            if(Vector3.Dot(waistDirection, horizLook) < Mathf.Cos(Mathf.Deg2Rad * maxWaistAngle))
                waistDirection = horizLook;
            return;
        }

        waistDirection = BestDirection(moveDirection, horizLook, out int i);
    }

    Vector3 BestDirection(Vector3 from, Vector3 to, out int index)
    {
        Vector3 dir = from;
        float dot = Vector3.Dot(to, dir);
        index = 0;
        for(int i = 1; i < 4; i++)
        {
            //rotate clockwise
            float buffer = dir.x;
            dir.x = dir.z;
            dir.z = -buffer;
            float newDot = Vector3.Dot(to, dir);
            if(newDot > dot)
            {
                dot = newDot;
                from = dir;
                index = i;
            }
        }
        return from;
    }
}