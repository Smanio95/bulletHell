using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class StandardEnemy : Enemy
{
    private float originalHealth;

    [SerializeField] StandardEnemyBullet bulletPrefab;
    public Animator anim;

    void Awake()
    {
        originalHealth = health;
        currentLevel = (int)Levels.Standard;
    }

    new void Update()
    {
        base.Update();
    }

    private new void OnEnable()
    {
        base.OnEnable();
        health = originalHealth;
        StartCoroutine(Move());
        StartCoroutine(Shoot());
    }

    private new void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    IEnumerator Move()
    {
        while (true)
        {
            float elapsed = 0;
            Vector3 direction = GetDirection(transform.position);
            while (elapsed < movementSeconds)
            {
                characterController.Move(speed * Time.deltaTime * direction);
                elapsed += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
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
            StandardEnemyBullet bullet = GetBullet();
            bullet.gameObject.SetActive(true);
        }
    }

    StandardEnemyBullet GetBullet()
    {
        lock (EnemyManager.enemyBulletList[(int)Levels.Standard])
        {
            StandardEnemyBullet bullet;

            bullet = EnemyManager.enemyBulletList[(int)Levels.Standard].Count == 0
                ? Instantiate(bulletPrefab, bulletsParent)
                : (StandardEnemyBullet)EnemyManager.enemyBulletList[(int)Levels.Standard].Dequeue();

            bullet.front = front;
            return bullet;
        }
    }

}
