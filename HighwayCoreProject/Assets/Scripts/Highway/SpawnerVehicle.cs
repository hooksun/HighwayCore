using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerVehicle : Vehicle, IEnemySpawner
{
    public int spawnPlatform;
    public float spawnCooldown;
    bool onCooldown;
    public override bool isSpawner{get => true;}
    public bool canSpawn{get => !onCooldown;}

    public PlatformAddress address;

    void OnEnable()
    {
        onCooldown = false;
    }

    public void SpawnEnemy(Enemy enemy)
    {
        Platform plat = Platforms[spawnPlatform];
        Vector2 center = (plat.BoundsStart + plat.BoundsEnd) * 0.5f;
        address.platformIndex = spawnPlatform;
        enemy.SetPlatform(address, new TransformPoint(transform, new Vector3(center.x, plat.height, center.y) - transformOffset));
        
        StartCoroutine(SpawnCooldown());
    }
    public float DistanceFrom(Vector3 pos) => position - pos.z;

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