using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TankEnemy : Enemy
{
    private float originalHealth;

    [SerializeField] TankEnemyBullet bulletPrefab;

    void Awake()
    {
        originalHealth = health;
        currentLevel = (int)Levels.Tank;
    }

    new void Update()
    {
        base.Update();
        if(gameObject.activeSelf) characterController.Move(speed * Time.deltaTime * -1 * transform.right);
    }

    private new void OnEnable()
    {
        base.OnEnable();
        health = originalHealth;
        StartCoroutine(Shoot());
    }

    private new void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            float elapsed = 0;
            while (elapsed < shootingSec)
            {
                elapsed += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            PlaceBullets();
        }
    }

    void PlaceBullets()
    {
        Array values = Enum.GetValues(typeof(TankEnemyBullet.Position));
        for (int i = 0; i < values.Length; i++)
        {
            TankEnemyBullet bullet = EnemyManager.enemyBulletList[(int)Levels.Tank].Count == 0
                ? Instantiate(bulletPrefab, bulletsParent)
                : (TankEnemyBullet)EnemyManager.enemyBulletList[(int)Levels.Tank].Dequeue();

            bullet.front = front;
            bullet.position = (TankEnemyBullet.Position)values.GetValue(i);
            bullet.transform.rotation = transform.rotation;
            bullet.gameObject.SetActive(true);
        }
    }
}
