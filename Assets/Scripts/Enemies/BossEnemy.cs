using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BossEnemy : Enemy
{
    [SerializeField] ParticleSystem portal;
    public Transform bossEnemyParent;
    [SerializeField] Levels[] possibleEnemies;
    [SerializeField] Enemy[] enemyPrefabs;
    [SerializeField] int maxSpawnableEnemies = 4;
    public Transform bossBulletParent;
    [SerializeField] Levels[] possibleBullets;
    [SerializeField] EnemyBullet[] bulletPrefabs;
    [SerializeField] int minBullets = 5;
    [SerializeField] int maxBullets = 8;

    private float elapsed = 0;
    private bool executingAction = false;
    private float actionWaitTime;

    private (float minRange, float maxRange) spawnRange;

    private List<Enemy> enemyList;

    private enum Actions
    {
        Shoot,
        Spawn
    }

    void Start()
    {
        spawnRange = (-portal.shape.radius, portal.shape.radius);
        enemyList = new List<Enemy>();
        actionWaitTime = 0.5f;
    }

    new void Update()
    {
        if (!executingAction)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= actionWaitTime)
            {
                ExecuteAction();
                actionWaitTime = UnityEngine.Random.Range(Mathf.Max(0, shootingSec - 0.5f), shootingSec + 0.5f);
            }
        }
    }

    private void ExecuteAction()
    {
        executingAction = true;

        int action = CheckChildrenDisabled()
            ? UnityEngine.Random.Range(0, Enum.GetValues(typeof(Actions)).Length)
            : (int)Actions.Shoot;

        switch (action)
        {
            case (int)Actions.Shoot:
                Shoot();
                break;
            case (int)Actions.Spawn:
                StartCoroutine(Spawn());
                break;
        }
    }

    private bool CheckChildrenDisabled()
    {
        foreach (Enemy enemy in enemyList)
        {
            if (enemy.gameObject.activeSelf)
                return false;
        }
        enemyList = new List<Enemy>();
        return true;
    }

    // shoot logic
    private void Shoot()
    {
        List<EnemyBullet> bulletList = GetBullets();

        foreach (EnemyBullet bullet in bulletList)
        {
            bullet.gameObject.SetActive(true);
        }

        executingAction = false;
        elapsed = 0;
    }

    List<EnemyBullet> GetBullets()
    {
        List<float> yPositions = new List<float>();
        List<EnemyBullet> enemyBullets = new List<EnemyBullet>();

        int numberOfElements = UnityEngine.Random.Range(minBullets, maxBullets + 1);
        (float minRange, float maxRange) spawnRange = (-1 * Camera.main.orthographicSize, Camera.main.orthographicSize);

        RetrieveYPos(yPositions, spawnRange, numberOfElements);

        for (int i = 0; i < numberOfElements; i++)
        {
            int randomBulletIndex = UnityEngine.Random.Range(0, possibleBullets.Length);
            EnemyBullet toSpawn;

            Vector3 position = new Vector3(front.position.x, TakeYPos(yPositions), front.position.z);

            if (EnemyManager.enemyBulletList[(int)possibleBullets[randomBulletIndex]].Count == 0)
            {
                toSpawn = Instantiate(
                    bulletPrefabs[randomBulletIndex],
                    position,
                    Quaternion.Euler(0, 0, 0),
                    bossBulletParent
                    );
            }
            else
            {
                toSpawn = EnemyManager.enemyBulletList[(int)possibleBullets[randomBulletIndex]].Dequeue();
                toSpawn.transform.SetPositionAndRotation(
                    position,
                    Quaternion.Euler(0, 0, 0)
                    );
            }

            if (possibleBullets[randomBulletIndex] == Levels.Tank)
            {
                ((TankEnemyBullet)toSpawn).position = TankEnemyBullet.Position.Straight;
            }

            toSpawn.front = null;

            enemyBullets.Add(toSpawn);
        }
        return enemyBullets;
    }


    // spawn logic
    IEnumerator Spawn()
    {
        portal.Play();
        yield return new WaitForSeconds(0.5f);

        List<Enemy> enemies = GetEnemies();
        foreach (Enemy enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
            StartAnimation(enemy);
        }
        StartCoroutine(ResetActionVars());
    }

    List<Enemy> GetEnemies()
    {
        List<float> yPositions = new List<float>();
        int numberOfElements = UnityEngine.Random.Range(1, maxSpawnableEnemies + 1);

        RetrieveYPos(yPositions, spawnRange, numberOfElements);

        for (int i = 0; i < numberOfElements; i++)
        {
            int randomEnemyIndex = UnityEngine.Random.Range(0, possibleEnemies.Length);
            Enemy toSpawn;

            //float enemySize = enemyPrefabs[randomEnemyIndex]
            //        .transform
            //        .GetComponent<CharacterController>()
            //        .radius;

            float yPos = TakeYPos(yPositions);

            if (EnemyManager.enemyList[(int)possibleEnemies[randomEnemyIndex]].Count == 0)
            {
                toSpawn = Instantiate(
                    enemyPrefabs[randomEnemyIndex],
                    new Vector3(front.position.x, yPos, front.position.z),
                    enemyPrefabs[randomEnemyIndex].transform.rotation,
                    bossEnemyParent
                    );
            }
            else
            {
                toSpawn = EnemyManager.enemyList[(int)possibleEnemies[randomEnemyIndex]].Dequeue();
                toSpawn.transform.SetPositionAndRotation(
                    new Vector3(front.position.x, yPos, front.position.z),
                    enemyPrefabs[randomEnemyIndex].transform.rotation
                    );
            }

            enemyList.Add(toSpawn);
        }

        return enemyList;
    }

    void RetrieveYPos(List<float> yPositions, (float minRange, float maxRange) spawnRange, int numberOfElements)
    {

        float size = (spawnRange.maxRange - spawnRange.minRange) / numberOfElements;

        float minYPos = spawnRange.minRange;

        for (int i = 0; i < numberOfElements; i++)
        {
            if(i == 0 || i == numberOfElements - 1)
            {
                minYPos += size / 2;
            }
            else
            {
                minYPos += size;
            }
            yPositions.Add(minYPos);
        }
    }

    float TakeYPos(List<float> yPositions, bool random = false)
    {
        int index = random ? UnityEngine.Random.Range(0, yPositions.Count) : 0;
        float pos = yPositions[index];
        yPositions.RemoveAt(index);
        return pos;
    }

    float RetrieveYPos(List<float> yPositions, (float minRange, float maxRange) spawnRange, float enemySize)
    {
        float yPos = UnityEngine.Random.Range(spawnRange.minRange, spawnRange.maxRange);
        for (int j = 0; j < yPositions.Count; j++)
        {
            if (yPos < yPositions[j] + enemySize / 2 && yPos > yPositions[j] - enemySize / 2)
            {
                yPos = UnityEngine.Random.Range(spawnRange.minRange, spawnRange.maxRange);
                j = 0;
            }
        }
        yPositions.Add(yPos);
        return yPos;
    }

    private void StartAnimation(Enemy enemy)
    {
        StandardEnemy standard;
        FastEnemy fast;
        if (enemy.type == EnemyManager.EnemyTypes.Standard)
        {
            standard = enemy.GetComponent<StandardEnemy>();
            standard.enabled = false;
            StartCoroutine(Animate(standard));
            return;
        }
        fast = enemy.GetComponent<FastEnemy>();
        fast.enabled = false;
        StartCoroutine(Animate(fast));
    }

    private IEnumerator Animate(StandardEnemy standard)
    {
        standard.anim.SetBool(AnimationBools.appear, true);
        yield return new WaitForSeconds(CommonUtils.GetAnimationLength(standard.anim, AnimationClips.appear) + 0.5f);
        standard.anim.SetBool(AnimationBools.appear, false);
        standard.enabled = true;
    }

    private IEnumerator Animate(FastEnemy fast)
    {
        fast.anim.SetBool(AnimationBools.appear, true);
        yield return new WaitForSeconds(CommonUtils.GetAnimationLength(fast.anim, AnimationClips.appear) + 0.5f);
        fast.anim.SetBool(AnimationBools.appear, false);
        fast.enabled = true;
    }

    private IEnumerator ResetActionVars()
    {
        yield return new WaitForSeconds(0.5f);
        executingAction = false;
        elapsed = 0;
        portal.Stop();
    }

}
