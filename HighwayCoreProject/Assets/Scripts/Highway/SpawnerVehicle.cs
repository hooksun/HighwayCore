using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerVehicle : Vehicle, IEnemySpawner
{
    public int spawnPlatform;
    public Vector3 spawnPoint;
    public bool groundedSpawn;
    public float spawnCooldown;

    public Animator anim;
    public float spawnDelay;

    bool onCooldown;
    public override bool isSpawner{get => true;}
    public bool canSpawn{get => !onCooldown;}

    void OnEnable()
    {
        onCooldown = false;
    }

    public void SpawnEnemy(Enemy enemy)
    {
        if(anim != null)
            anim.SetTrigger("OpenHatch");
        
        StartCoroutine(SpawnDelay(enemy));
        StartCoroutine(SpawnCooldown());
    }
    public float DistanceFrom(Vector3 pos) => position - pos.z;

    IEnumerator SpawnDelay(Enemy enemy)
    {
        if(spawnDelay > 0f)
            yield return new WaitForSeconds(spawnDelay);
        
        Platform plat = Platforms[spawnPlatform];
        Vector2 center = (plat.BoundsStart + plat.BoundsEnd) * 0.5f;
        PlatformAddress address = new PlatformAddress(transform.parent.GetComponent<Lane>(), this, spawnPlatform);
        enemy.SetPlatform(address, new TransformPoint(transform, spawnPoint - transformOffset), groundedSpawn);
        enemy.Activate();
    }

    IEnumerator SpawnCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(spawnCooldown);
        onCooldown = false;
    }
}

public interface IEnemySpawner
{
    void SpawnEnemy(Enemy enemy);
    float DistanceFrom(Vector3 position);
    bool canSpawn{get;}
}