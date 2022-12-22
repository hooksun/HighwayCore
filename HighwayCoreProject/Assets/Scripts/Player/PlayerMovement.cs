using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerBehaviour, IProjectileSpawner
{
    public Rigidbody rb;
    
    public MoveState Walk, Air;
    public float JumpHeight, JumpYPosMulti, JumpGravity, FallGravity, JumpCooldown;
    public MoveState AirJump;
    public float JetpackSpeed, JetpackForce, JetpackDecel, JetpackFuel, FuelCost, AirJumpCost, RefuelRate, GroundRefuelRate;
    public MoveState Grapple;
    public GrappleProjectile GrappleObj;
    public float GrappleSpeed, GrappleAccel, GrappleDecel, GrappleDrag, GrappleRetractRange, GrappleRetractDelay, GrappleCooldown, GlobalGrappleCooldown;
    public MoveState WallRun, Vault;
    public float WallRunTiltAngle, WallCheckDist, MinWallRunYSpeed, VaultSpeed, VaultDist;
    public Vector3 WallCheckPoint, WallJumpForce, VaultJumpForce, VaultStart;
    public int WallRunCooldown, GroundCheckCooldown;
    public float GroundCheckDist, GroundCheckRadius, MaxSlope, GroundCenter;
    public bool HasJetpack, HasGrapple;
    public LayerMask GroundMask, HardGroundMask;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public RaycastHit groundInfo;
    MoveState current;
    Vector2 direction;
    Vector3 directionWorld;
    Vector3 velocity;
    int groundCheckCooldown;

    Vector3 groundVel;
    IMovingGround currentGround;
    Transform groundTrans;
    
    public void ChangeDirection(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();
        direction = dir.normalized;
    }

    void Start()
    {
        current = Air;
        UIManager.SetJetpackAirJumpCost(AirJumpCost / JetpackFuel);
    }
    
    void FixedUpdate()
    {
        GroundCheck();

        if(!isGrounded)
        {
            Vector3 newVel = rb.velocity - groundVel;
            if(newVel.y <= 0f)
                velocity.y = newVel.y;
            if(current != Air || direction == Vector2.zero)
                velocity = newVel;
        }

        velocity.y *= (velocity.y > 0?posBuffer:negBuffer);
        velocity += forceBuffer;
        forceBuffer = Vector3.zero;
        posBuffer = 1f;
        negBuffer = 1f;

        directionWorld = transform.rotation * new Vector3(direction.x, 0f, direction.y);
        
        Grappling();

        Move();

        DoGravity();

        Vaulting();

        WallRunning();

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
        if(!doGravity)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics.SphereCast(transform.position, GroundCheckRadius, Vector3.down, out groundInfo, GroundCheckDist - GroundCheckRadius, GroundMask);
        // {
        //     isGrounded = !isGrounded;
        //     if(!isGrounded)
        //     {
        //         //velocity.y = Mathf.Min(velocity.y, 0f);
        //     }
        // }
        if(isGrounded)
        {
            SetCurrentState(Walk);
            RaycastHit normalCorrected;
            if(groundInfo.collider.Raycast(new Ray(groundInfo.point + groundInfo.normal * 0.1f + Vector3.up*0.001f, -groundInfo.normal), out normalCorrected, 0.11f))
                groundInfo = normalCorrected;
            else
                groundInfo.normal = Vector3.up;
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

    void Move()
    {
        Vector3 horizVel = new Vector3(velocity.x, 0f, velocity.z);
        
        if(isGrounded)
        {
            if(current.maxSpeed >= current.speed)
            {
                horizVel = Vector3.ClampMagnitude(horizVel, current.maxSpeed);
            }
            float accel = (current.decel != 0f && horizVel.sqrMagnitude > current.speed * current.speed?current.decel:current.accel);
            velocity = Vector3.MoveTowards(horizVel, directionWorld * current.speed, accel * Time.fixedDeltaTime);
            SetCurrentState(Air);
            return;
        }

        Vector3 targetVel = horizVel + directionWorld * current.accel * Time.fixedDeltaTime;
        float SqrHorizSpeed = horizVel.sqrMagnitude;
        float speedThreshold = Mathf.Max(current.speed, Mathf.Sqrt(SqrHorizSpeed));
        if(current.maxSpeed >= current.speed)
        {
            speedThreshold = Mathf.Min(speedThreshold, current.maxSpeed);
        }
        targetVel = Vector3.ClampMagnitude(targetVel, speedThreshold);
        if(current.decel != 0f && targetVel.sqrMagnitude > current.speed * current.speed)
        {
            targetVel -= targetVel.normalized * current.decel * Time.fixedDeltaTime;
        }
        velocity = targetVel + Vector3.up * velocity.y;
        
        if(current.cooldown > 0)
        {
            current.cooldown--;
            return;
        }
        current = Air;
    }
    void SetCurrentState(MoveState newState)
    {
        current.cooldown = 0;
        current = newState;
        current.cooldown = current.coyote;
    }

    bool doGravity = true;
    void DoGravity()
    {
        if(!doGravity)
            return;

        if(!isGrounded)
        {
            velocity.y -= ((velocity.y > 0)?JumpGravity:FallGravity) * Time.fixedDeltaTime;
            return;
        }
        velocity.y = 0f;
        if((velocity - Vector3.up * velocity.y).sqrMagnitude > 0 && groundInfo.normal.y > Mathf.Cos(Mathf.Deg2Rad * MaxSlope))
        {
            float newVel = ((velocity.x * groundInfo.normal.x) + (velocity.z * groundInfo.normal.z)) / -groundInfo.normal.y;
            if(newVel < 0)
                velocity.y = newVel;
            else
                velocity -= newVel * (groundInfo.normal - Vector3.up * groundInfo.normal.y);
            
            velocity.y -= FallGravity * Time.fixedDeltaTime;
        }
    }

    float grappleDelay;
    bool isGrappling, grappleCooldown;
    TransformPoint grapplePoint;
    Enemy grappledEnemy;
    public void GrappleInput(InputAction.CallbackContext ctx)
    {
        if(!ctx.started || !HasGrapple || grappleCooldown)
            return;

        if(!isGrappling)
        {
            if(player.usingAbility || player.abilityCooldown || player.usingWeapon)
                return;
            grappledEnemy = null;
            player.abilityCooldown = true;
            GrappleObj.Fire(player.Head.forward, player.Head.position, this);
        }
        else
        {
            StopGrapple();
        }
        grapplePoint.transform = null;
    }

    float grappleSqrDist;
    void Grappling()
    {
        if(!isGrappling)
            return;
        
        if(grappleDelay < 0f || grapplePoint.transform == null || !grapplePoint.transform.gameObject.activeInHierarchy)
        {
            StopGrapple();
            return;
        }
        if((transform.position - grapplePoint.worldPoint).sqrMagnitude <= GrappleRetractRange * GrappleRetractRange)
        {
            grappleDelay -= Time.fixedDeltaTime;
        }

        ChangeGround(grapplePoint.transform);

        SetCurrentState(Grapple);
        Vector3 grappleDir = grapplePoint.worldPoint - transform.position;
        float newSqrDist = grappleDir.sqrMagnitude;
        grappleDir.Normalize();
        directionWorld -= grappleDir * Mathf.Min(0f, Vector3.Dot(directionWorld, grappleDir));
        velocity = Vector3.MoveTowards(velocity, grappleDir * GrappleSpeed, (newSqrDist<grappleSqrDist?GrappleAccel:GrappleDecel) * Time.fixedDeltaTime);
        grappleSqrDist = newSqrDist;
        if(grappledEnemy != null)
        {
            grappledEnemy.Stun(Vector3.zero);
        }
    }

    void StopGrapple()
    {
        if(!isGrappling)
            return;

        GrappleObj.Retract();
        player.usingAbility = false;
        isGrappling = false;
        doGravity = true;
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

    public void OnTargetHit(RaycastHit hit)
    {
        isGrappling = true;
        player.usingAbility = true;
        grappleDelay = GrappleRetractDelay;
        grapplePoint = new TransformPoint(hit.transform, hit.point - hit.transform.position);
        grappleSqrDist = (hit.point - transform.position).sqrMagnitude;
        grappledEnemy = hit.transform.GetComponent<Enemy>();
        velocity *= GrappleDrag;
        doGravity = false;
    }
    public void OnTargetNotFound()
    {
        isGrappling = true;
        StopGrapple();
    }
    public void OnReset()
    {
        StartCoroutine(GrapplingCooldown());
        StartCoroutine(GlobalCooldown(GlobalGrappleCooldown));
    }
    
    int vaultDelay;
    Vector3 vaultDir;
    bool isVaulting{get => Vault.cooldown > 0;}
    void Vaulting()
    {
        if(!isGrounded && !player.usingAbility && direction.sqrMagnitude > 0 && (velocity.y <= VaultSpeed))
        {
            Vector3 origin = transform.position + Quaternion.LookRotation(directionWorld) * VaultStart;

            RaycastHit hit;
            if(Physics.Raycast(origin, Vector3.down, out hit, VaultDist, GroundMask) && hit.normal.y > 0.7f ||
            Physics.Raycast(transform.position + Vector3.up * VaultStart.y, directionWorld, out hit, VaultStart.z, HardGroundMask))
            {
                ChangeGround(hit.transform);
                SetCurrentState(Vault);
                vaultDir = directionWorld;
            }
        }
        
        if(isVaulting)
        {
            velocity.y = Mathf.Max(VaultSpeed, velocity.y);
        }
    }

    bool isWallRunning;
    int wallRunCooldown;
    Vector3 wallDir;
    void WallRunning()
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
                SetCurrentState(WallRun);
                velocity.y = 0f;
                velocity = Vector3.ProjectOnPlane(velocity, hit.normal);
                wallDir = -hit.normal;
                ChangeGround(hit.transform);
                Vector3 cross = Vector3.Cross(wallDir, Vector3.up);
                float angle = Vector3.Dot(cross, transform.forward);
                player.Aim.RotateHead(Vector3.forward * angle * WallRunTiltAngle);
                return;
            }
        }

        player.Aim.RotateHead(Vector3.zero);
        if(WallRun.cooldown == 0)
            wallDir = Vector3.zero;
    }

    public float currentFuel;
    void Jetpack()
    {
        if(isJumping && !isGrounded && !player.usingAbility && !isVaulting && !isWallRunning && currentFuel > 0f && velocity.y <= JetpackSpeed)
        {
            float velY = Mathf.MoveTowards(velocity.y, JetpackSpeed, (JetpackForce + (velocity.y>0?JumpGravity:FallGravity)) * Time.fixedDeltaTime);
            velocity.y = 0f;
            velocity -= velocity.normalized * JetpackDecel * Time.fixedDeltaTime;
            velocity.y = velY;
            currentFuel -= FuelCost * Time.fixedDeltaTime;
            UIManager.SetJetpackFuel(currentFuel / JetpackFuel);
            return;
        }
        
        if(currentFuel != JetpackFuel)
        {
            currentFuel = Mathf.MoveTowards(currentFuel, JetpackFuel, (isGrounded?GroundRefuelRate:RefuelRate) * Time.fixedDeltaTime);
            UIManager.SetJetpackFuel(currentFuel / JetpackFuel);
        }
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
        if(Air.cooldown > 0)
        {
            AddForce(Vector3.up * Mathf.Sqrt(2f * JumpGravity * JumpHeight), JumpYPosMulti);
            Air.cooldown = 0;
            return;
        }
        if(Vault.cooldown > 0)
        {
            AddForce(Quaternion.LookRotation(vaultDir) * VaultJumpForce, 0f);
            Vault.cooldown = 0;
            return;
        }
        if(WallRun.cooldown > 0)
        {
            Vector3 jumpForce = WallJumpForce;
            jumpForce.x = 0;
            AddForce(transform.rotation * jumpForce + -wallDir * WallJumpForce.x, 0f);
            wallRunCooldown = WallRunCooldown;
            WallRun.cooldown = 0;
            return;
        }
        if(HasJetpack && currentFuel >= AirJumpCost)
        {
            AddForce(Vector3.up * Mathf.Sqrt(2f * JumpGravity * JumpHeight), JumpYPosMulti);
            currentFuel -= AirJumpCost;
            SetCurrentState(AirJump);
            UIManager.SetJetpackFuel(currentFuel / JetpackFuel);
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
            Air.cooldown = 0;
            isGrounded = false;
        }
    }
}

[System.Serializable]
public class MoveState
{
    public float speed, accel, decel, maxSpeed;
    public int coyote;
    [HideInInspector] public int cooldown;
}

public interface IMovingGround
{
    Vector3 velocity{get;}
}