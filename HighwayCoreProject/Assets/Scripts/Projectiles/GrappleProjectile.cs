using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleProjectile : MonoBehaviour
{
    public LineRenderer lRenderer;
    
    public float speed, retractSpeed, maxDistance, approachRate, castRadius;
    public LayerMask hitMask;

    public Audio ShootAudio, RetractAudio;

    IProjectileSpawner spawner;
    TransformPoint grapplePoint;
    Vector3 offset, velocity, projPos, grapplePos;
    float distance;
    bool isFiring, retracting;

    public void Fire(Vector3 direction, Vector3 spawnPoint, IProjectileSpawner spawnr)
    {
        if(isFiring)
            return;
        isFiring = true;
        retracting = false;
        offset = transform.position - spawnPoint;
        velocity = direction * speed;
        projPos = spawnPoint;
        distance = 0f;
        spawner = spawnr;
        lRenderer.enabled = true;
        enabled = true;
        ShootAudio.Play();
    }
    public void Retract()
    {
        if(!enabled)
            return;
        
        RetractAudio.Play();
        isFiring = false;
        retracting = true;
    }
    public void Disable()
    {
        isFiring = false;
        retracting = false;
        grapplePoint.transform = null;
        lRenderer.enabled = false;
        enabled = false;
    }

    void Update()
    {
        if(Time.deltaTime == 0f)
            return;
        
        Simulate();
        lRenderer.SetPosition(0, transform.position);
        lRenderer.SetPosition(1, grapplePos);
    }

    void Simulate()
    {
        if(retracting)
        {
            grapplePos = Vector3.MoveTowards(grapplePos, transform.position, retractSpeed * Time.deltaTime);
            if(grapplePos == transform.position)
            {
                spawner.OnReset();
                RetractAudio.Stop();
                Disable();
            }
            return;
        }
        if(isFiring)
        {
            RaycastHit hit;
            if(!Physics.SphereCast(projPos, castRadius, velocity, out hit, velocity.magnitude * Time.deltaTime, hitMask))
            {
                projPos += velocity * Time.deltaTime;
                offset *= 1f - approachRate * Time.deltaTime;
                grapplePos = projPos + offset;
                distance += speed * Time.deltaTime;
                if(distance > maxDistance)
                {
                    Retract();
                    spawner.OnTargetNotFound();
                }
                return;
            }

            isFiring = false;
            grapplePoint = new TransformPoint(hit.transform, hit.point - hit.transform.position);
            spawner.OnTargetHit(hit);
            RetractAudio.Play();
        }
        if(grapplePoint.transform == null || !grapplePoint.transform.gameObject.activeInHierarchy)
        {
            Retract();
            return;
        }
        grapplePos = Vector3.MoveTowards(grapplePos, grapplePoint.worldPoint, speed * Time.deltaTime);
    }
}

public interface IProjectileSpawner
{
    void OnTargetHit(RaycastHit hit);
    void OnTargetNotFound();
    void OnReset();
}