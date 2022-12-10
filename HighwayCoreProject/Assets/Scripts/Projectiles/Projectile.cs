using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 position, offset;
    float speed, approachRate, activeTime;
    LayerMask hitMask;
    IProjectileSpawner spawner;

    const float ActiveTime = 5f;

    public void Initiate(Vector3 startPosition, Quaternion rotation, Vector3 transformPosition, float sped, LayerMask layerMask, IProjectileSpawner spawnr)
    {
        position = startPosition;
        offset = transformPosition - startPosition;
        speed = sped;
        hitMask = layerMask;
        spawner = spawnr;
        transform.position = transformPosition;
        transform.rotation = rotation;
        activeTime = ActiveTime;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if(activeTime <= 0f)
        {
            spawner.OnTargetNotFound();
            gameObject.SetActive(false);
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(position, transform.forward, out hit, speed * Time.deltaTime, hitMask))
        {
            spawner.OnTargetHit(hit);
            gameObject.SetActive(false);
            return;
        }

        position += transform.forward * speed * Time.deltaTime;
        offset *= 1f - approachRate * Time.deltaTime;
        transform.position = position + offset;
        activeTime -= Time.deltaTime;
    }

    public static Quaternion RandomSpread(float spread)
    {
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        return new Quaternion(x, y, z, 1);
    }
}

public interface IHurtBox
{
    void TakeDamage(float amount);
    bool crit{get;}
}