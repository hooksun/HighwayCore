using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : EnemyBehaviour
{
    public float DesiredDistance, MaxHeight, WalkSpeed, JumpHeight, JumpGravity, FallGravity, JumpDistance, MaxJumpDistance;
    public string JumpAnimation, LongJumpAnimation;
    public float JumpFadeTime, JumpDelay, LongJumpDelay, LongJumpTilt, TiltSpeed, TiltRecovery;
    public float groundDist, groundStunResistance, groundCheckCooldown, velocityStunMulti, idleTime;
    public int maxBacknForth = 1;
    public Vector3 transformOffset;
    public LayerMask GroundMask;

    public PlatformAddress currentPlatform;
    public TransformPoint transformPosition;

    protected virtual float jumpGrav{get => JumpGravity;}
    protected virtual float fallGrav{get => FallGravity;}

    protected PlatformAddress targetPlatform, lastPlatform;
    protected TransformPoint targetPoint, jumpPoint;
    protected Vector3 airVelocity, tilt;
    protected RaycastHit groundInfo;
    protected float jumpDelay;
    protected bool isGrounded = true;
    protected bool isJumping, groundCooldown, longJump;
    protected int backnForth;
    
    public override void Activate()
    {
        transform.position = transformPosition.worldPoint + transformOffset;
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
        isGrounded = true;
        isJumping = false;
        groundCooldown = false;
        backnForth = 0;
        idleing = false;
        targetPlatform.lane = null;
        lastPlatform.lane = null;
        FindNewPath();
    }

    protected virtual void Update()
    {
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
            if(transform.position.y < -100f || Player.ActivePlayer.position.z - transform.position.z > 160f)
            {
                enemy.Die();
                return;
            }
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
        if((transform.position - enemy.targetPlayer.position).sqrMagnitude < DesiredDistance * DesiredDistance)
            return;
        
        if(targetPoint.transform == null)
        {
            FindNewPath();
            return;
        }

        if(isJumping)
        {
            jumpDelay -= Time.deltaTime;
            if(jumpDelay <= 0)
            {
                isGrounded = false;
                enemy.Animation.Play(JumpAnimation, JumpFadeTime);
                if(longJump)
                {
                    tilt = (Vector3.up + (jumpPoint.worldPoint - transform.position).normalized * LongJumpTilt).normalized;
                }
            }
            return;
        }
        
        if(transformPosition.point == targetPoint.point && transformPosition.transform == targetPoint.transform)
        {
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
        enemy.Animation.SetMove((targetPoint.point - transformPosition.point).normalized);
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
            if(!groundCooldown)
            {
                Land();
                return;
            }
            airVelocity.y = 0f;
        }

        transform.position += airVelocity * Time.deltaTime;
    }

    public Vector3 GetMoveDirection()
    {
        Vector3 dir = (isJumping?jumpPoint.worldPoint - transform.position:(isGrounded?targetPoint.worldPoint - transform.position:airVelocity));
        dir.y = 0f;
        return dir.normalized;
    }

    public override void Stun(Vector3 knockback)
    {
        if(isGrounded)
        {
            knockback *= 1f - groundStunResistance;
            isGrounded = false;
            StartCoroutine(GroundCooldown());
        }
        airVelocity *= velocityStunMulti;
        airVelocity += knockback;
        tilt = Vector3.up;
        isJumping = false;
        longJump = false;
    }
    IEnumerator GroundCooldown()
    {
        groundCooldown = true;
        yield return new WaitForSeconds(groundCheckCooldown);
        groundCooldown = false;
    }

    bool idleing;
    IEnumerator Idleing()
    {
        idleing = true;
        yield return new WaitForSeconds(idleTime);
        idleing = false;
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
        if(SetTargetPoint() && lastPlatform.lane != null && lastPlatform.platform == targetPlatform.platform)
        {
            backnForth++;
            return;
        }
        backnForth = 0;
    }

    protected virtual bool SetTargetPoint()
    {
        if(targetPlatform.lane == null || targetPlatform.platform == currentPlatform.platform)
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
        if(Mathf.Sqrt(sqrDist) / jumpTime > WalkSpeed)
        {
            jumpDelay = LongJumpDelay;
            longJump = true;
            enemy.Animation.Play(LongJumpAnimation, JumpFadeTime);
        }
    }

    protected void InitiateJump(float jumpHeight)
    {
        float h1 = transform.position.y - transformOffset.y;
        float h2 = jumpPoint.worldPoint.y;
        float height1 = Mathf.Max(h1, h2) + jumpHeight - h1;
        float height2 = height1 + h1 - h2;
        airVelocity.y = Mathf.Sqrt(2f * jumpGrav * height1);
        float fallVelY = Mathf.Sqrt(2f * fallGrav * height2);
        jumpTime = (2f * height1 / airVelocity.y) + (2f * height2 / fallVelY);

        isJumping = true;
        longJump = false;
        jumpDelay = JumpDelay;
        tilt = Vector3.up;
    }

    protected virtual void Land()
    {
        if(groundInfo.transform.gameObject.layer == 8)
        {
            enemy.Die();
            return;
        }
        
        lastPlatform = currentPlatform;
        currentPlatform = (isJumping?targetPlatform:groundInfo.transform.GetComponent<Vehicle>().ClosestPlatform(transform.position));
        transformPosition.transform = groundInfo.transform;
        transformPosition.point = transform.position - transformOffset - transformPosition.transform.position;
        isJumping = false;
        isGrounded = true;
        longJump = false;
        tilt = Vector3.up;
        targetPoint.transform = null;

        enemy.Animation.Reset(JumpFadeTime);
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
        
        if(jumpTime > 0)
        {
            Vector3 distance = jumpPoint.worldPoint - transform.position;
            distance.y = 0f;
            airVelocity = distance / jumpTime + Vector3.up * airVelocity.y;
            jumpTime -= Time.deltaTime;
        }
    }
}

public struct PlatformAddress
{
    public Lane lane;
    public Vehicle vehicle;
    public int platformIndex;

    public PlatformAddress(Lane _lane, Vehicle veh, int platI)
    {
        lane = _lane;
        vehicle = veh;
        platformIndex = platI;
    }

    public int laneIndex{get => lane.transform.GetSiblingIndex();}
    public int vehicleIndex{get => vehicle.transform.GetSiblingIndex();}
    public Platform platform{get => vehicle.Platforms[platformIndex];}
    public bool enabled{get => lane != null && vehicle != null && vehicle.gameObject.activeInHierarchy;}

    public TransformPoint ClosestPointTo(Vector3 point)
    {
        point += vehicle.transformOffset - vehicle.transform.position;
        point.x = Mathf.Clamp(point.x, platform.BoundsStart.x, platform.BoundsEnd.x);
        point.z = Mathf.Clamp(point.z, platform.BoundsStart.y, platform.BoundsEnd.y);
        point -= vehicle.transformOffset;
        point.y = platform.height - vehicle.transformOffset.y;
        return new TransformPoint(vehicle.transform, point);
    }

    public TransformPoint RandomPoint()
    {
        Vector3 point = Vector3.zero;
        point.x = Random.Range(platform.BoundsStart.x, platform.BoundsEnd.x);
        point.z = Random.Range(platform.BoundsStart.y, platform.BoundsEnd.y);
        point -= vehicle.transformOffset;
        point.y = platform.height - vehicle.transformOffset.y;
        return new TransformPoint(vehicle.transform, point);
    }

    public float DistanceFrom(Vector3 point) => vehicle.DistanceToPlatform(point, platformIndex);
}

public struct TransformPoint
{
    public Transform transform;
    public Vector3 point;

    public TransformPoint(Transform _transform, Vector3 _point)
    {
        transform = _transform;
        point = _point;
    }

    public Vector3 worldPoint => (transform == null?point:transform.position + point);
}