using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : EnemyBehaviour
{
    public float MaxHeight, WalkSpeed, JumpHeight, JumpGravity, FallGravity, JumpDistance, MaxJumpDistance;
    public string JumpAnimation, LongJumpAnimation;
    public float JumpFadeTime, JumpDelay, LongJumpDelay, LongJumpTilt, TiltSpeed, TiltRecovery;
    public float groundDist, groundStunResistance, groundCheckCooldown, velocityStunMulti, idleTime;
    public int maxBacknForth = 1;
    public Vector3 transformOffset;
    public LayerMask GroundMask;
    public Audio footstepAudio;

    public PlatformAddress currentPlatform;
    public TransformPoint transformPosition;

    [HideInInspector] public bool isGrounded = true;

    protected virtual float jumpGrav{get => JumpGravity;}
    protected virtual float fallGrav{get => FallGravity;}
    protected virtual string jumpAnim{get => JumpAnimation;}

    protected PlatformAddress targetPlatform, lastPlatform;
    protected TransformPoint targetPoint, jumpPoint;
    protected Vector3 airVelocity, groundVel, tilt;
    protected RaycastHit groundInfo;
    protected float jumpDelay, groundCooldown;
    protected bool isJumping, longJump;
    protected int backnForth;
    
    public override void Activate()
    {
        transform.position = transformPosition.worldPoint + transformOffset;
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
        tilt = Vector3.up;
        airVelocity = Vector3.zero;
        targetPoint.transform = null;
        jumpPoint.transform = null;
        isJumping = false;
        groundCooldown = 0f;
        backnForth = 0;
        idleing = false;
        targetPlatform.lane = null;
        lastPlatform.lane = null;

        if(!isGrounded)
        {
            jumpPoint = currentPlatform.ClosestPointTo(transform.position);
            InitiateJump(JumpHeight);
            enemy.Animation.Play(jumpAnim, 0, JumpFadeTime);
        }
    }

    public override void Die()
    {
        EnemyRagdoll ragdoll = RagdollPool.GetObject(enemy.RagdollIndex);
        ragdoll.Init(enemy.Animation.spine[0], (isGrounded?groundVel:airVelocity));
    }

    protected virtual void Update()
    {
        if(Time.deltaTime == 0f)
            return;

        if(transform.position.y < -10f || Player.ActivePlayer.position.z - transform.position.z > 100f)
        {
            enemy.Die();
            return;
        }

        Tilt();
        if(isGrounded)
        {
            if(transformPosition.transform == null || !transformPosition.transform.gameObject.activeInHierarchy)
            {
                enemy.Die();
                return;
            }
            Simulate();
        }
        if(!isGrounded)
        {
            AirSimulate();
            return;
        }
        transform.position = transformPosition.worldPoint + transformOffset;
        if(GroundCheck())
        {
            transform.position = groundInfo.point + transformOffset;
        }
    }

    protected virtual bool GroundCheck() => Physics.Raycast(transform.position, Vector3.down, out groundInfo, groundDist, GroundMask);
    
    protected virtual void Simulate()
    {
        if(idleing || enemy.stunned)
            return;

        if(isJumping)
        {
            jumpDelay -= Time.deltaTime;
            if(jumpDelay <= 0)
            {
                isGrounded = false;
                transformPosition.transform = null;
                enemy.Animation.Play(jumpAnim, 0, JumpFadeTime);
                if(longJump)
                {
                    tilt = (Vector3.up + (jumpPoint.worldPoint - transform.position).normalized * LongJumpTilt).normalized;
                }
            }
            return;
        }
        
        if(targetPoint.transform == null)
        {
            FindNewPath();
            return;
        }
        
        Vector3 targetPos = targetPoint.point;
        targetPos.y = transformPosition.point.y;
        if(transformPosition.point == targetPos && transformPosition.transform == targetPoint.transform)
        {
            groundVel = Vector3.zero;
            enemy.Animation.SetMove(Vector3.zero);
            if(targetPlatform.lane == null || targetPlatform.platform == currentPlatform.platform || jumpPoint.transform == null)
            {
                StartCoroutine(Idleing());
                return;
            }
            
            Jump();
            return;
        }

        //movetowards logic
        groundVel = (targetPoint.point - transformPosition.point).normalized;
        enemy.Animation.SetMove(groundVel);
        groundVel *= WalkSpeed;
        footstepAudio.Play();
        transformPosition.point = Vector3.MoveTowards(transformPosition.point, targetPoint.point, WalkSpeed * Time.deltaTime);
    }

    protected virtual void Tilt()
    {
        if(tilt == Vector3.zero || transform.up == tilt)
            return;
        
        Vector3 forwardTilt = Vector3.Cross(Vector3.Cross(tilt, Vector3.forward), tilt);
        float delta = (tilt==Vector3.up?TiltRecovery:TiltSpeed) * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forwardTilt, tilt), delta);
    }

    protected virtual void AirSimulate()
    {
        if(isJumping)
        {
            Jumping();
        }

        airVelocity.y -= (airVelocity.y > 0?jumpGrav:fallGrav) * Time.deltaTime;
        if(airVelocity.y <= 0 && GroundCheck() && transform.position.y - groundInfo.point.y <= transformOffset.y)
        {
            if(groundCooldown <= 0f)
            {
                Land();
                return;
            }
            airVelocity.y = 0f;
        }
        if(groundCooldown > 0f)
            groundCooldown -= Time.deltaTime;

        transform.position += airVelocity * Time.deltaTime;
        if(airVelocity.sqrMagnitude > 1225f)
        {
            print(airVelocity + ", " + airVelocity.magnitude + ", " + Time.time + ", " + jumpTime);
        }
    }

    public Vector3 GetMoveDirection()
    {
        Vector3 dir = (isJumping?jumpPoint.worldPoint - transform.position:(isGrounded?targetPoint.worldPoint - transform.position:airVelocity));
        dir.y = 0f;
        return dir.normalized;
    }

    public override void Stun(Vector3 knockback)
    {
        groundVel = Vector3.zero;
        if(isGrounded)
        {
            knockback *= 1f - groundStunResistance;
            isGrounded = false;
            groundCooldown = groundCheckCooldown;
        }
        airVelocity *= velocityStunMulti;
        airVelocity += knockback;
        tilt = Vector3.up;
        isJumping = false;
        longJump = false;
    }

    bool idleing;
    IEnumerator Idleing()
    {
        idleing = true;
        yield return new WaitForSeconds(idleTime);
        idleing = false;
        if(backnForth > 0)
            backnForth--;
        targetPoint.transform = null;
    }

    protected virtual void FindNewPath()
    {
        List<PlatformAddress> neighbours = enemy.manager.RequestPlatformNeighbours(currentPlatform, JumpDistance);
        for(int i = 0; i < neighbours.Count; i++)
        {
            PlatformAddress plat = neighbours[i];
            if(plat.platform.height - currentPlatform.platform.height > MaxHeight
            ||(lastPlatform.lane != null && lastPlatform.platform == plat.platform && backnForth >= maxBacknForth))
            {
                neighbours.RemoveAt(i);
                i--;
                continue;
            }
            Vector3 closest = plat.ClosestPointTo(transform.position).worldPoint;
            Vector3 platDist = currentPlatform.ClosestPointTo(closest).worldPoint - closest;
            platDist.y = 0f;
            if(platDist.sqrMagnitude > MaxJumpDistance * MaxJumpDistance)
            {
                neighbours.RemoveAt(i);
                i--;
                continue;
            }
        }
        targetPlatform = enemy.Attack.PickPlatform(neighbours, currentPlatform);
        isGrounded = true;
        if(SetTargetPoint() && lastPlatform.lane != null && lastPlatform.vehicle == targetPlatform.vehicle && lastPlatform.platform == targetPlatform.platform)
        {
            backnForth++;
            return;
        }
        backnForth = 0;
    }

    protected virtual bool SetTargetPoint()
    {
        if(targetPlatform.lane == null || (targetPlatform.vehicle == currentPlatform.vehicle && targetPlatform.platform == currentPlatform.platform))
        {
            targetPoint = currentPlatform.RandomPoint();
            jumpPoint.transform = null;
            return false;
        }
        jumpPoint = targetPlatform.ClosestPointTo(transform.position);
        targetPoint = currentPlatform.ClosestPointTo(jumpPoint.worldPoint);
        return true;
    }

    protected virtual void Jump()
    {
        Vector3 jumpDirection = jumpPoint.worldPoint - transform.position;
        jumpDirection.y = 0f;
        float sqrDist = jumpDirection.sqrMagnitude;
        if(sqrDist > MaxJumpDistance * MaxJumpDistance)
        {
            targetPoint.transform = null;
            return;
        }
        
        InitiateJump(JumpHeight);
        targetPoint.transform = null;
        if(Mathf.Sqrt(sqrDist) / jumpTime > WalkSpeed)
        {
            longJump = true;
            jumpDelay = LongJumpDelay;
            enemy.Animation.Play(LongJumpAnimation, 0, JumpFadeTime);
        }
    }

    protected void InitiateJump(float jumpHeight)
    {
        float h1 = transform.position.y - transformOffset.y;
        float h2 = jumpPoint.worldPoint.y;
        float height1 = Mathf.Max(h1, h2) + jumpHeight - h1;
        float height2 = height1 + h1 - h2;
        airVelocity.y = Mathf.Sqrt(2f * jumpGrav * height1);
        jumpTime = (2f * height1 / airVelocity.y) + (2f * height2 / Mathf.Sqrt(2f * fallGrav * height2));

        isJumping = true;
        longJump = false;
        jumpDelay = JumpDelay;
        tilt = Vector3.up;
    }

    protected virtual void Land()
    {
        airVelocity = Vector3.zero;
        isGrounded = true;
        if(groundInfo.transform.gameObject.layer == 8)
        {
            enemy.Health.SetStunItems(true);
            transformPosition.transform = null;
            return;
        }
        
        lastPlatform = currentPlatform;
        currentPlatform = groundInfo.transform.GetComponent<Vehicle>().ClosestPlatform(transform.position);
        transformPosition.transform = groundInfo.transform;
        transformPosition.point = transform.position - transformOffset - transformPosition.transform.position;
        isJumping = false;
        longJump = false;
        tilt = Vector3.up;
        targetPoint.transform = null;

        enemy.Animation.Reset();
    }

    protected float jumpTime;
    protected virtual void Jumping()
    {
        if(longJump && transform.up == tilt)
            tilt = Vector3.up;

        if(jumpPoint.transform == null || !jumpPoint.transform.gameObject.activeInHierarchy)
        {
            isJumping = false;
            return;
        }

        if(jumpTime <= JumpFadeTime)
            enemy.Animation.PlayIdle(JumpFadeTime);
        
        if(jumpTime > 0.01f)
        {
            Vector3 distance = jumpPoint.worldPoint - transform.position;
            distance.y = 0f;
            airVelocity = distance / jumpTime + Vector3.up * airVelocity.y;
            jumpTime -= Time.deltaTime;
        }
    }
}