using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : Ability
{
    public Vector3 HitboxCenter, HalfExtent;
    public float delay, damage, knockback;
    public LayerMask EnemyMask;
    
    public override void Activate()
    {
        player.animator.Play("Player Melee test", 0, 0);
        StartCoroutine(Melee());
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
                enemy.TakeDamage(damage);
                enemy.Stun(player.Head.forward * knockback);
            }
        }
    }
}

public abstract class Ability : PlayerBehaviour
{
    public float cooldown;
    protected bool onCooldown;
    
    public void Input(InputAction.CallbackContext ctx)
    {
        if(!ctx.started || onCooldown)
            return;
        Activate();
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }
    
    public virtual void Activate(){}
}