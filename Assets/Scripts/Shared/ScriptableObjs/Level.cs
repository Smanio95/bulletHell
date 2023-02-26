using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Level", order = 1)]
public class Level : ScriptableObject
{
    public int instantiateThreshold = 3;
    [Range(1, 4)] public int maxEnemyInSingleCol = 2;
    public int minEnemiesToSpawn;
    public int maxEnemyToSpawn;
    public Enemy enemy;
    public EnemyBullet bullet;
}
