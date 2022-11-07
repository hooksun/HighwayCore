using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement Movement;
    public PlayerAim Aim;
    public PlayerStatus Status;
    public Transform Head;
    public GunScript Guns;
    public PickUpWeapon PickUp;
    public PlayerDamageSource Damage;
    public WeaponSwitching WeaponSwitch;

    public float TrailTime;
    public Vector3 positionOffset;
    [HideInInspector] public Vector3 trailPosition;
    public Vector3 position{get => transform.position + positionOffset;}

    void Awake()
    {
        Aim.player = this;
        Movement.player = this;
        Status.player = this;
        Guns.player = this;
        PickUp.player = this;
        Damage.player = this;
        WeaponSwitch.player = this;
    }
    
    void Update()
    {
        StartCoroutine(Trail(position));
    }

    IEnumerator Trail(Vector3 pos)
    {
        yield return new WaitForSeconds(TrailTime);
        trailPosition = pos;
    }

    public void Die()
    {
        Aim.Die();
        Movement.Die();
        Status.Die();
        Guns.Die();
        PickUp.Die();
        Damage.Die();
        WeaponSwitch.Die();
    }
}

public abstract class PlayerBehaviour : MonoBehaviour
{
    [HideInInspector] public Player player;

    public virtual void Die(){}
}