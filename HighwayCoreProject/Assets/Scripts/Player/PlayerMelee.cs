using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : PlayerBehaviour
{
    public float cooldown, globalCooldown;
    public float delay, damage, knockback;
    public Vector3 HitboxCenter, HalfExtent;
    public LayerMask EnemyMask;

    bool onCooldown;
    
    public void Input(InputAction.CallbackContext ctx)
    {
        if(!ctx.started || onCooldown || player.usingAbility || player.abilityCooldown)
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
        
        player.abilityCooldown = true;
        yield return new WaitForSeconds(globalCooldown);
        player.abilityCooldown = false;
    }
    
    public virtual void Activate()
    {
        player.animator.Play("Player Melee test", 0, 0);
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
                print(enemy.transform.name);
                enemy.Stun(player.Head.forward * knockback);
                enemy.TakeDamage(damage);
            }
        }
    }
}