using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyInterface : MonoBehaviour
{
    public abstract void TakeDmg(float dmg, ref int enemyKilled);
    public abstract void EnemyDeath();

}
