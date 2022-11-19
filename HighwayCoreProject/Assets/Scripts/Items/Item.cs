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
        Vector3 difference = Player.ActivePlayer.position - transform.position;
        if(!lockOn && !onCooldown && Player.ActivePlayer != null && difference.sqrMagnitude <= lockOnRadius * lockOnRadius)
        {
            lockOn = true;
        }

        if(lockOn && Player.ActivePlayer != null)
        {
            if(difference.sqrMagnitude <= caughtRadius * caughtRadius)
            {
                //Player.ActivePlayer.GiveItem(this);
                print("caught item");
                gameObject.SetActive(false);
                return;
            }
            velocity = Vector3.MoveTowards(velocity, difference.normalized * lockOnSpeed, lockOnAccel * Time.deltaTime);
        }
        else
            velocity += Vector3.down * gravity * Time.deltaTime;
        
        transform.position += velocity * Time. deltaTime;
        if(transform.position.y < -100f)
        {
            gameObject.SetActive(false);
        }
    }
}
