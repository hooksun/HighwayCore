using UnityEngine;

public class EnemyPool : ObjectPool<Enemy>
{
    protected override bool Available(Enemy obj) => !obj.spawned;
}