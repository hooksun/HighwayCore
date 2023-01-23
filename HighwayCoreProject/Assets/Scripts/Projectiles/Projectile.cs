using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 position, offset;
    float speed, activeTime, damage;
    bool hitPlayer;
    LayerMask hitMask;

    const float ActiveTime = 0.5f;
    const float approachRate = 3f;

    public void Initiate(Vector3 startPosition,Quaternion rotation,Vector3 transformPosition,float sped,float dmg,float spread,LayerMask layerMask,bool hit=false)
    {
        position = startPosition;
        offset = transformPosition - startPosition;
        speed = sped;
        damage = dmg;
        hitMask = layerMask;
        hitPlayer = hit;
        transform.position = transformPosition;
        transform.rotation = rotation * RandomSpread(spread);
        activeTime = ActiveTime;
    }

    void Update()
    {
        if(Time.deltaTime == 0f)
            return;

        if(activeTime <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(position, transform.forward, out hit, speed * Time.deltaTime * 2f, hitMask))
        {
            IHurtBox hurtBox = hit.transform.GetComponent<IHurtBox>();
            if(hurtBox != null)
            {
                hurtBox.TakeDamage(damage);
                if(hitPlayer)
                {
                    Vector3 dir = -transform.forward;
                    dir.y = 0f;
                    dir.Normalize();
                    UIManager.SetDamageDirection(dir);
                }
                
            }
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