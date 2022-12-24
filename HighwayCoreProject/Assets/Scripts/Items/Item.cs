using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float startCooldown, gravity, lockOnRadius, lockOnAccel, lockOnSpeed, caughtRadius;

    Vector3 velocity;
    bool onCooldown, lockOn;

    public void Spawn(Vector3 position, Vector3 startVel)
    {
        transform.position = position;
        velocity = startVel;
        lockOn = false;
        StartCoroutine(StartCooldown());
    }
    IEnumerator StartCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(startCooldown);
        onCooldown = false;
    }

    void Update()
    {
        if(!Move())
            velocity += Vector3.down * gravity * Time.deltaTime;
        
        transform.position += velocity * Time. deltaTime;
        if(transform.position.y < -100f)
        {
            gameObject.SetActive(false);
        }
    }

    bool Move()
    {
        if(Player.ActivePlayer == null)
            return false;

        Vector3 difference = Player.ActivePlayer.position - transform.position;
        if(!lockOn && !onCooldown && difference.sqrMagnitude <= lockOnRadius * lockOnRadius)
        {
            lockOn = true;
        }

        if(lockOn)
        {
            velocity = Vector3.MoveTowards(velocity, difference.normalized * lockOnSpeed, lockOnAccel * Time.deltaTime);
            if(difference.sqrMagnitude <= caughtRadius * caughtRadius)
            {
                AddToPlayer();
                print("caught item");
                gameObject.SetActive(false);
            }
            return true;
        }
        return false;
    }

    protected virtual void AddToPlayer(){}
}
