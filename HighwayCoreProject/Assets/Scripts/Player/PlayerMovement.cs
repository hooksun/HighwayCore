using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerBehaviour
{
    public Rigidbody rb;
    
    public float Speed, SpeedWhileVaulting, GroundAccel, AirAccel, JumpHeight, JumpYPosMulti, JumpGravity, FallGravity;
    public float JetpackSpeed, JetpackForce, JetpackDecel, JetpackFuel, FuelCost, AirJumpCost, RefuelRate, GroundRefuelRate, AirJumpAccel;
    public float WallRunSpeed, WallRunAccel, WallRunDecel, WallCheckDist, MinWallRunYSpeed, VaultSpeed, VaultDist, VaultDecel;
    public Vector3 WallCheckPoint, WallJumpForce, VaultJumpForce, VaultStart;
    public int AirJumpTime, WallRunCooldown, VaultStopDelay, GroundCheckCooldown, GravityCooldown;
    public float GroundCheckDist, GroundCheckRadius, NormalCheckDist, MaxSlope;
    public LayerMask GroundMask, HardGroundMask;

    Vector2 direction;
    Vector3 directionWorld;
    Vector3 velocity;
    [HideInInspector] public bool isGrounded;
    int groundCheckCooldown;
    int gravityCooldown;
    [HideInInspector] public RaycastHit groundInfo;

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

        Vault();

        WallRun();

        Jetpack();

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
            ChangeGround(groundInfo.collider.transform);
        }
    }

    void ChangeGround(Transform newGround)
    {
        if(groundTrans != newGround)
        {
            groundTrans = newGround;
            currentGround = groundTrans.GetComponent<IMovingGround>();
            groundVel = Vector3.zero;
        }
        if(currentGround != null)
        {
            groundVel = currentGround.velocity;
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
            float maxSpeed = (isVaulting?SpeedWhileVaulting:(isWallRunning?WallRunSpeed:Speed));
            float accel = (isWallRunning?WallRunAccel:(airJumping>0?AirJumpAccel:AirAccel));
            Vector3 targetVel = horizVel + directionWorld * accel * Time.fixedDeltaTime;
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
            if((velocity - Vector3.up * velocity.y).sqrMagnitude > 0 && groundInfo.normal.y > Mathf.Cos(Mathf.Deg2Rad * MaxSlope))
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
    Vector3 vaultDir;
    bool isVaulting{get => vaultDelay > 0;}
    void Vault()
    {
        if(!isGrounded && direction.sqrMagnitude > 0 && (velocity.y <= VaultSpeed))
        {
            Vector3 origin = transform.position + Quaternion.LookRotation(directionWorld) * VaultStart;

            RaycastHit hit;
            if(Physics.Raycast(origin, Vector3.down, out hit, VaultDist, GroundMask) && hit.normal.y > 0.7f ||
            Physics.Raycast(transform.position + Vector3.up * VaultStart.y, directionWorld, out hit, VaultStart.z, HardGroundMask))
            {
                ChangeGround(hit.collider.transform);
                vaultDir = directionWorld;
                vaultDelay = VaultStopDelay;
                velocity.y = 0f;
                if(velocity.sqrMagnitude > SpeedWhileVaulting * SpeedWhileVaulting)
                {
                    velocity -= velocity.normalized * VaultDecel * Time.fixedDeltaTime;
                }
                velocity.y = VaultSpeed;
                return;
            }
        }
        
        if(isVaulting)
        {
            velocity.y = Mathf.Max(VaultSpeed, velocity.y);
            vaultDelay--;
        }
    }

    bool isWallRunning;
    int wallRunCooldown;
    Vector3 wallDir;
    void WallRun()
    {
        isWallRunning = false;
        if(wallRunCooldown > 0)
        {
            wallRunCooldown--;
            return;
        }
        
        if(!isGrounded && !isVaulting && velocity.y <= MinWallRunYSpeed && direction.sqrMagnitude > 0)
        {
            RaycastHit hit = groundInfo;
            if((wallDir != Vector3.zero && Physics.Raycast(transform.position + WallCheckPoint, wallDir, out hit, WallCheckDist, GroundMask))
            || Physics.Raycast(transform.position + WallCheckPoint, transform.right, out hit, WallCheckDist, GroundMask)
            || Physics.Raycast(transform.position + WallCheckPoint, -transform.right, out hit, WallCheckDist, GroundMask))
            {
                isWallRunning = true;
                velocity.y = 0f;
                velocity = Vector3.ProjectOnPlane(velocity, hit.normal);
                if(velocity.sqrMagnitude > WallRunSpeed * WallRunSpeed)
                {
                    velocity -= velocity.normalized * WallRunDecel * Time.fixedDeltaTime;
                }
                wallDir = -hit.normal;
                ChangeGround(hit.collider.transform);
                return;
            }
        }

        wallDir = Vector3.zero;
    }

    public float currentFuel;
    int airJumping;
    void Jetpack()
    {
        if(airJumping > 0)
        {
            airJumping--;
        }
        if(isJumping && !isGrounded && !isVaulting && !isWallRunning && currentFuel > 0f && velocity.y <= JetpackSpeed)
        {
            float velY = Mathf.MoveTowards(velocity.y, JetpackSpeed, (JetpackForce + (velocity.y>0?JumpGravity:FallGravity)) * Time.fixedDeltaTime);
            velocity.y = 0f;
            velocity -= velocity.normalized * JetpackDecel * Time.fixedDeltaTime;
            velocity.y = velY;
            currentFuel -= FuelCost * Time.fixedDeltaTime;
            return;
        }
        
        currentFuel = Mathf.MoveTowards(currentFuel, JetpackFuel, (isGrounded?GroundRefuelRate:RefuelRate) * Time.fixedDeltaTime);
    }

    bool isJumping;
    public void Jump(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed)
            isJumping = ctx.started;
        if(!ctx.started)
            return;
        
        if(isGrounded)
        {
            AddForce(Vector3.up * Mathf.Sqrt(2f * JumpGravity * JumpHeight), JumpYPosMulti);
            return;
        }
        if(isVaulting)
        {
            AddForce(Quaternion.LookRotation(vaultDir) * VaultJumpForce, 0f);
            return;
        }
        if(isWallRunning)
        {
            Vector3 jumpForce = WallJumpForce;
            jumpForce.x = 0;
            AddForce(transform.rotation * jumpForce + -wallDir * WallJumpForce.x, 0f);
            wallRunCooldown = WallRunCooldown;
            return;
        }
        if(currentFuel >= AirJumpCost)
        {
            AddForce(Vector3.up * Mathf.Sqrt(2f * JumpGravity * JumpHeight), JumpYPosMulti);
            currentFuel -= AirJumpCost;
            airJumping = AirJumpTime;
            print("Air Jump");
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