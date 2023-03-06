using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IHurtBox
{
    public PlayerSettings Settings;
    public PlayerMovement Movement;
    public PlayerAim Aim;
    public PlayerStatus Status;
    public Transform Head;
    public Animator animator;
    public GunScript Guns;
    public WeaponSwitching WeaponSwitch;
    public PlayerMelee Melee;
    public WeaponAnim weaponAnim;
    public PlayerInput input;
    public Audio HitAudio;
    public AudioClip NormalHit, CritHit;

    public static Player ActivePlayer;
    
    public float TrailTime, LookAtEnemyAngle, DamageResistance, deathFloor;
    public Vector3 positionOffset;
    [HideInInspector] public Vector3 trailPosition;
    [HideInInspector] public float score;
    [HideInInspector] public bool abilityCooldown, usingAbility, usingWeapon, freezeScore, dead;
    public Vector3 position{get => transform.position + positionOffset;}

    public bool LookingAt(Vector3 pos) => Vector3.Dot(Head.forward, (pos - Head.position).normalized) >= Mathf.Cos(Mathf.Deg2Rad*LookAtEnemyAngle);

    void Awake()
    {
        Settings.Load();
        SaveSystem.settings = Settings;

        Aim.player = this;
        Movement.player = this;
        Status.player = this;
        Guns.player = this;
        WeaponSwitch.player = this;
        Melee.player = this;
        //weaponAnim.player = this;
        EnableInput(true);
    }

    void OnEnable() => ActivePlayer = this;
    void OnDisable() => ActivePlayer = null;
    
    void Update()
    {
        if(Time.deltaTime == 0f)
            return;
        
        StartCoroutine(Trail(position));
        if(!freezeScore && !dead)
            score = Mathf.Max(transform.position.z, score);
        if(transform.position.y < deathFloor)
            Die();
    }

    IEnumerator Trail(Vector3 pos)
    {
        yield return new WaitForSeconds(TrailTime);
        trailPosition = pos;
    }

    public bool crit{get => false;}
    public void TakeDamage(float amount)
    {
        Status.TakeDamage(amount * (1 - DamageResistance));
    }

    public void Die(bool die = true)
    {
        EnableInput(!die);
        dead = die;
        if(dead)
            Status.Die();
        UIManager.SetDeathMenu(die);
    }

    public void SetScore()
    {
        score -= score % 1f;
        UIManager.SetScore(score, Settings.settings.highscore);
        Settings.settings.highscore = Mathf.Max(Settings.settings.highscore, score);
        Settings.Save();
    }

    public void PlayHitAudio(bool crit)
    {
        if(!crit)
            return;

        HitAudio.clip = (crit?CritHit:NormalHit);
        HitAudio.Play();
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
}