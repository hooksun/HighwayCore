using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerBehaviour, IProjectileSpawner
{
    public Rigidbody rb;
    
    public float Speed, SpeedWhileVaulting, GroundAccel, AirAccel, JumpHeight, JumpYPosMulti, JumpGravity, FallGravity, JumpCooldown;
    public float JetpackSpeed, JetpackForce, JetpackDecel, JetpackFuel, FuelCost, AirJumpCost, RefuelRate, GroundRefuelRate, AirJumpAccel;
    public GrappleProjectile Grapple;
    public float GrappleSpeed, GrappleAccel, GrappleDrag, GrappleCooldown, GlobalGrappleCooldown;
    public float WallRunSpeed, WallRunAccel, WallRunDecel, WallRunTiltAngle, WallCheckDist, MinWallRunYSpeed, VaultSpeed, VaultDist, VaultDecel;
    public Vector3 WallCheckPoint, WallJumpForce, VaultJumpForce, VaultStart;
    public int AirJumpTime, WallRunCooldown, VaultStopDelay, GroundCheckCooldown, GravityCooldown;
    public float GroundCheckDist, GroundCheckRadius, MaxSlope;
    public bool HasJetpack, HasGrapple;
    public LayerMask GroundMask, HardGroundMask;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public RaycastHit groundInfo;
    Vector2 direction;
    Vector3 directionWorld;
    Vector3 velocity;
    int groundCheckCooldown;
    int gravityCooldown;

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

        Grappling();

        Vault();

        WallRun();

        if(HasJetpack)
            Jetpack();

        jumpCooldown = Mathf.Max(jumpCooldown - Time.fixedDeltaTime, 0f);
        
        rb.velocity = velocity + groundVel;
    }

    void GroundCheck()
    {
        if(groundCheckCooldown > 0)
        {
            groundCheckCooldown--;
            return;
        }
        if(isGrappling)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics.SphereCast(transform.position, GroundCheckRadius, Vector3.down, out groundInfo, GroundCheckDist - GroundCheckRadius, GroundMask);
        if(isGrounded)
        {
            ChangeGround(groundInfo.transform);
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
        if(!isGrappling)
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

    bool doGravity = true;
    void DoGravity()
    {
        if(!doGravity)
            return;

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

    bool isGrappling, grappleCooldown;
    TransformPoint grapplePoint;
    Enemy grappledEnemy;
    public void GrappleInput(InputAction.CallbackContext ctx)
    {
        if(!ctx.started || !HasGrapple || grappleCooldown)
            return;

        if(!isGrappling)
        {
            if(player.usingAbility || player.abilityCooldown)
                return;
            grappledEnemy = null;
            player.abilityCooldown = true;
            Grapple.Fire(player.Head.forward, player.Head.position, this);
        }
        else
        {
            StopGrapple();
        }
        grapplePoint.transform = null;
    }

    void Grappling()
    {
        if(!isGrappling)
            return;
        
        if(grapplePoint.transform == null || !grapplePoint.transform.gameObject.activeInHierarchy)
        {
            StopGrapple();
            return;
        }

        ChangeGround(grapplePoint.transform);

        velocity = Vector3.MoveTowards(velocity, (grapplePoint.worldPoint - transform.position).normalized * GrappleSpeed, GrappleAccel * Time.fixedDeltaTime);
        if(grappledEnemy != null)
        {
            grappledEnemy.Stun(Vector3.zero);
        }
    }

    void StopGrapple()
    {
        if(!isGrappling)
            return;

        Grapple.Retract();
        isGrappling = false;
        player.usingAbility = false;
        doGravity = true;
        StartCoroutine(GrapplingCooldown());
        StartCoroutine(GlobalCooldown(GlobalGrappleCooldown));
    }

    IEnumerator GrapplingCooldown()
    {
        if(GrappleCooldown <= 0)
        {
            grappleCooldown = false;
            yield break;
        }

        grappleCooldown = true;
        yield return new WaitForSeconds(GrappleCooldown);
        grappleCooldown = false;
    }
    IEnumerator GlobalCooldown(float cooldown)
    {
        if(cooldown <= 0)
        {
            player.abilityCooldown = false;
            yield break;
        }
        
        player.abilityCooldown = true;
        yield return new WaitForSeconds(cooldown);
        player.abilityCooldown = false;
    }

    public void SetHit(RaycastHit hit)
    {
        isGrappling = true;
        player.usingAbility = true;
        grapplePoint = new TransformPoint(hit.transform, hit.point - hit.transform.position);
        grappledEnemy = hit.transform.GetComponent<Enemy>();
        velocity *= GrappleDrag;
        doGravity = false;
    }
    public void OnTargetNotFound()
    {
        isGrappling = true;
        StopGrapple();
    }
    
    int vaultDelay;
    Vector3 vaultDir;
    bool isVaulting{get => vaultDelay > 0;}
    void Vault()
    {
        if(!isGrounded && !player.usingAbility && direction.sqrMagnitude > 0 && (velocity.y <= VaultSpeed))
        {
            Vector3 origin = transform.position + Quaternion.LookRotation(directionWorld) * VaultStart;

            RaycastHit hit;
            if(Physics.Raycast(origin, Vector3.down, out hit, VaultDist, GroundMask) && hit.normal.y > 0.7f ||
            Physics.Raycast(transform.position + Vector3.up * VaultStart.y, directionWorld, out hit, VaultStart.z, HardGroundMask))
            {
                ChangeGround(hit.transform);
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
        
        if(!isGrounded && !player.usingAbility && !isVaulting && velocity.y <= MinWallRunYSpeed && direction.sqrMagnitude > 0)
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
                ChangeGround(hit.transform);
                Vector3 cross = Vector3.Cross(wallDir, Vector3.up);
                float angle = Vector3.Dot(cross, transform.forward);
                player.Aim.RotateHead(Vector3.forward * angle * WallRunTiltAngle);
                return;
            }
        }

        player.Aim.RotateHead(Vector3.zero);
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
        if(isJumping && !isGrounded && !player.usingAbility && !isVaulting && !isWallRunning && currentFuel > 0f && velocity.y <= JetpackSpeed)
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
    float jumpCooldown;
    public void Jump(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed)
            isJumping = ctx.started;
        if(!ctx.started || jumpCooldown > 0)
            return;
        
        jumpCooldown = JumpCooldown;
        if(isGrappling)
        {
            StopGrapple();
            return;
        }
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
        if(HasJetpack && currentFuel >= AirJumpCost)
        {
            AddForce(Vector3.up * Mathf.Sqrt(2f * JumpGravity * JumpHeight), JumpYPosMulti);
            currentFuel -= AirJumpCost;
            airJumping = AirJumpTime;
            return;
        }
        jumpCooldown = 0f;
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