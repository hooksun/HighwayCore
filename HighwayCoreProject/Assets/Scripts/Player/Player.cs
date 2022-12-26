using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerMovement Movement;
    public PlayerAim Aim;
    public PlayerStatus Status;
    public Transform Head;
    public Animator animator;
    public GunScript Guns;
    public PickUpWeapon PickUp;
    public PlayerDamageSource Damage;
    public WeaponSwitching WeaponSwitch;
    public PlayerMelee Melee;
    public WeaponAnim weaponAnim;
    public PlayerInput input;

    public static Player ActivePlayer;
    
    public float TrailTime, LookAtEnemyAngle;
    public Vector3 positionOffset;
    [HideInInspector] public Vector3 trailPosition;
    [HideInInspector] public bool abilityCooldown, usingAbility, usingWeapon;
    public Vector3 position{get => transform.position + positionOffset;}

    public bool LookingAt(Vector3 pos) => Vector3.Dot(Head.forward, (pos - Head.position).normalized) >= Mathf.Cos(Mathf.Deg2Rad*LookAtEnemyAngle);

    void Awake()
    {
        Aim.player = this;
        Movement.player = this;
        Status.player = this;
        Guns.player = this;
        PickUp.player = this;
        Damage.player = this;
        WeaponSwitch.player = this;
        Melee.player = this;
        //weaponAnim.player = this;
        EnableInput(true);
    }

    void OnEnable() => ActivePlayer = this;
    void OnDisable() => ActivePlayer = null;
    
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
        Melee.Die();
        //weaponAnim.Die();
    }

    public void EnableInput(bool yes)
    {
        if(!yes)
        {
            input.enabled = false;
            return;
        }
        StartCoroutine(EnableDelay());
    }
    IEnumerator EnableDelay()
    {
        yield return null;
        input.enabled = true;
    }
}

public abstract class PlayerBehaviour : MonoBehaviour
{
    [HideInInspector] public Player player;

    public virtual void Die(){}
}