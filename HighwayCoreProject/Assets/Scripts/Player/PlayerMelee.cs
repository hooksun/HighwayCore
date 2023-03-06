using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : PlayerBehaviour
{
    public float cooldown, globalCooldown;
    public float animDelay = .4f, delay, damage, knockback;
    public Vector3 HitboxCenter, HalfExtent;
    public LayerMask EnemyMask;

    bool onCooldown;
    public bool isPunching, punchCooldown;
    
    public void Input(InputAction.CallbackContext ctx)
    {
        if(!ctx.started || onCooldown || player.usingAbility || player.abilityCooldown || player.usingWeapon)
            return;
        Activate();
    }

    IEnumerator Cooldown()
    {
        if(cooldown <= 0)
            yield break;
        
        onCooldown = true;
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }
    IEnumerator GlobalCooldown()
    {
        if(globalCooldown <= 0)
            yield break;
        
        punchCooldown = true;
        player.abilityCooldown = true;
        yield return new WaitForSeconds(globalCooldown);
        player.abilityCooldown = false;
        punchCooldown = false;
    }
    
    public virtual void Activate()
    {
        player.animator.SetTrigger("unequip");
        isPunching = true;
        Invoke("AnimReset", animDelay);
        StartCoroutine(Melee());
        StartCoroutine(Cooldown());
        StartCoroutine(GlobalCooldown());
    }

    IEnumerator Melee()
    {
        yield return new WaitForSeconds(delay);
        Collider[] hits = Physics.OverlapBox(player.Head.position + player.Head.rotation * HitboxCenter, HalfExtent, player.Head.rotation, EnemyMask);
        if(hits != null)
        {
            foreach(Collider hit in hits)
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                enemy.Stun(player.Head.forward * knockback);
                enemy.TakeDamage(damage);
            }
        }
    }
    void AnimReset(){
        isPunching = false;
    }
}