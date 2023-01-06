using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Rigidbody rb;
    public Collider coll;
    public Vector2 minVelcoty, maxVelocity;
    public float startCooldown, gravity, lockOnRadius, lockOnAccel, lockOnSpeed, caughtRadius;

    Vector3 velocity;
    bool onCooldown, lockOn;

    public void Spawn(Vector3 position, Vector3 rotation)
    {
        transform.position = position;
        rb.velocity = Quaternion.Euler(rotation) * new Vector3(0f, Random.Range(minVelcoty.y, maxVelocity.y), Random.Range(minVelcoty.x, maxVelocity.x));
        coll.enabled = true;
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
        velocity = rb.velocity;
        if(!Move())
            velocity += Vector3.down * gravity * Time.deltaTime;
        
        transform.position += velocity * Time. deltaTime;
        rb.velocity = velocity;
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
            coll.enabled = false;
            lockOn = true;
        }

        if(lockOn)
        {
            velocity = Vector3.MoveTowards(velocity, difference.normalized * lockOnSpeed, lockOnAccel * Time.deltaTime);
            if(difference.sqrMagnitude <= caughtRadius * caughtRadius)
            {
                AddToPlayer();
                rb.velocity = Vector3.zero;
                gameObject.SetActive(false);
            }
            return true;
        }
        return false;
    }

    protected virtual void AddToPlayer(){}
}
