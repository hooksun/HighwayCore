using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    
    public float Speed, SpeedWhileVaulting, GroundAccel, AirAccel, JumpHeight, JumpYPosMulti, JumpGravity, FallGravity, VaultSpeed, VaultDist;
    public Vector3 VaultJumpForce, VaultStart;
    public int VaultStopDelay, GroundCheckCooldown, GravityCooldown;
    public float GroundCheckDist, GroundCheckRadius, NormalCheckDist;
    public LayerMask GroundMask, HardGroundMask;

    Vector2 direction;
    Vector3 directionWorld;
    Vector3 velocity;
    bool isGrounded;
    int groundCheckCooldown;
    int gravityCooldown;
    RaycastHit groundInfo;

    Vector3 groundVel;
    IMovingGround currentGround;
    Transform groundTrans;
    
    public void ChangeDirection(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();
        direction = dir;
    }
    
    void FixedUpdate()
    {
        velocity = rb.velocity;
        velocity -= groundVel;

        velocity.y *= (velocity.y > 0?posBuffer:negBuffer);
        velocity += forceBuffer;
        forceBuffer = Vector3.zero;
        posBuffer = 1f;
        negBuffer = 1f;

        GroundCheck();

        Move();

        DoGravity();

        //Vault();

        rb.velocity = velocity + groundVel;
    }

    void GroundCheck()
    {
        if(groundCheckCooldown > 0)
        {
            groundCheckCooldown--;
            return;
        }

        isGrounded = Physics.SphereCast(transform.position, GroundCheckRadius, Vector3.down, out groundInfo, GroundCheckDist - GroundCheckRadius, GroundMask);
        if(isGrounded)
        {
            if(groundTrans != groundInfo.collider.transform)
            {
                groundTrans = groundInfo.collider.transform;
                currentGround = groundTrans.GetComponent<IMovingGround>();
            }
            if(currentGround != null)
            {
                groundVel = currentGround.velocity;
            }
        }
    }

    Vector3 groundDir;
    void Move()
    {
        directionWorld = transform.rotation * new Vector3(direction.x, 0f, direction.y);
        Vector3 horizVel = new Vector3(velocity.x, 0f, velocity.z);
        
        if(isGrounded)
        {
            //velocity = Vector3.MoveTowards(Vector3.ClampMagnitude(horizVel, Speed), directionWorld * Speed, GroundAccel * Time.fixedDeltaTime);
            groundDir = Vector3.MoveTowards(groundDir, directionWorld * Speed, GroundAccel * Time.fixedDeltaTime);
            velocity = groundDir;
        }
        else
        {
            float maxSpeed = (isVaulting?SpeedWhileVaulting:Speed);
            Vector3 targetVel = horizVel + directionWorld * AirAccel * Time.fixedDeltaTime;
            float targetSqrSpeed = targetVel.sqrMagnitude;
            float SqrHorizSpeed = horizVel.sqrMagnitude;
            if(targetSqrSpeed > SqrHorizSpeed && targetSqrSpeed > maxSpeed * maxSpeed)
            {
                float speedThreshold = Mathf.Max(Mathf.Sqrt(SqrHorizSpeed), maxSpeed);

                targetVel = Vector3.ClampMagnitude(targetVel, speedThreshold);
            }
            velocity = targetVel + Vector3.up * velocity.y;

            groundDir = (SqrHorizSpeed <= Speed * Speed?horizVel:horizVel.normalized * Speed);
        }
    }

    void DoGravity()
    {
        if(isGrounded)
        {
            if(gravityCooldown > 0)
            {
                if(velocity.y > 0)
                    velocity.y = 0f;
                
                velocity.y -= FallGravity * Time.fixedDeltaTime;
                gravityCooldown--;
            }
            if((velocity - Vector3.up * velocity.y).sqrMagnitude > 0 && groundInfo.normal.y > .7f)
            {
                velocity.y = 0f;
                float newVel = ((velocity.x * groundInfo.normal.x) + (velocity.z * groundInfo.normal.z)) / -groundInfo.normal.y;
                if(newVel < 0)
                    velocity.y = newVel;
                else
                    velocity -= newVel * (groundInfo.normal - Vector3.up * groundInfo.normal.y);
                
                velocity.y -= FallGravity * Time.fixedDeltaTime;
            }
        }
        else
        {
            velocity.y -= ((velocity.y > 0)?JumpGravity:FallGravity) * Time.fixedDeltaTime;
            gravityCooldown = GravityCooldown;
        }
    }
    
    int vaultDelay;
    bool isVaulting{get => vaultDelay > 0;}
    void Vault()
    {
        if(!isGrounded && direction.sqrMagnitude > 0 && (velocity.y <= VaultSpeed))
        {
            Vector3 origin = transform.position + Quaternion.LookRotation(directionWorld) * VaultStart;

            RaycastHit hit;
            if(Physics.Raycast(origin, Vector3.down, out hit, VaultDist, GroundMask) && hit.normal.y > 0.7f)
            {
                if(!Physics.Raycast(transform.position + Vector3.up * VaultStart.y, directionWorld, VaultStart.z, HardGroundMask))
                {
                    vaultDelay = VaultStopDelay;
                    velocity.y = VaultSpeed;
                    return;
                }
            }
        }
        
        if(isVaulting)
        {
            velocity.y = Mathf.Max(VaultSpeed, velocity.y);
            vaultDelay--;
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        
        if(isGrounded)
        {
            AddForce(Vector3.up * Mathf.Sqrt(2f * JumpGravity * JumpHeight), JumpYPosMulti);
            return;
        }
        if(isVaulting)
        {
            AddForce(Quaternion.LookRotation(transform.rotation * new Vector3(direction.x, 0f, direction.y)) * VaultJumpForce, 0f);
            return;
        }
    }

    Vector3 forceBuffer;
    float posBuffer = 1f;
    float negBuffer = 1f;
    public void AddForce(Vector3 force, float yPosMulti = 1f, float yNegMulti = 0f)
    {
        forceBuffer += force;
        posBuffer *= yPosMulti;
        negBuffer *= yNegMulti;
        //Vector3 vel = rb.velocity;
        //vel.y *= (vel.y > 0?yPosMulti:yNegMulti);
        //rb.velocity = vel + force;
        if(isGrounded)
        {
            groundCheckCooldown = GroundCheckCooldown;
            isGrounded = false;
        }
    }
}

public interface IMovingGround
{
    Vector3 velocity{get;}
}