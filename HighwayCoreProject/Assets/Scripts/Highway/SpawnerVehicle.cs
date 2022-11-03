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

    public void SpawnEnemy(Enemy enemy)
    {

        StartCoroutine(SpawnCooldown());
    }
    public float DistanceFrom(Vector3 pos) => Mathf.Abs(pos.z - position);

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