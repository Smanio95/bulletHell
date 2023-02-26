using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastEnemy : Enemy
{
    private float originalHealth;
    private Vector3 originalScale;

    [SerializeField] FastBullet bulletPrefab;
    [SerializeField] int numberOfJumps = 2;
    public Animator anim;
    [SerializeField] float teleportJumpMultiplicator = 2;

    private bool teleportUsed = false;
    private bool isRightSide = false;
    private bool firstTimeMov = true;

    void Awake()
    {
        originalHealth = health;
        originalScale = transform.localScale;
        currentLevel = (int)Levels.Fast;
    }

    new void Update()
    {
        base.Update();
    }

    private new void OnEnable()
    {
        base.OnEnable();
        firstTimeMov = true;
        health = originalHealth;
        transform.localScale = originalScale;
        isRightSide = transform.position.x < Camera.main.transform.position.x;
        teleportUsed = false;
        StartCoroutine(Move());
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
            Vector3 direction = GetDirection(transform.position, isRightSide);
            if (firstTimeMov || teleportUsed || CommonUtils.FiftyFifty)
            {
                firstTimeMov = false;
                while (elapsed < movementSeconds && gameObject.activeSelf)
                {
                    characterController.Move(speed * Time.deltaTime * direction);
                    elapsed += Time.deltaTime;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }
            else
            {
                teleportUsed = true;
                int jumps = Random.Range(1, numberOfJumps + 1);
                for(int i = 0; i < jumps && gameObject.activeSelf; i++)
                {
                    anim.SetBool(AnimationBools.disappear, true);
                    yield return new WaitForSeconds(CommonUtils.GetAnimationLength(anim, AnimationClips.disappear));

                    transform.position = new Vector3(
                        transform.position.x + direction.x * teleportJumpMultiplicator,
                        transform.position.y + direction.y * teleportJumpMultiplicator,
                        transform.position.z);
                    anim.SetBool(AnimationBools.disappear, false);
                    anim.SetBool(AnimationBools.appear, true);
                    yield return new WaitForSeconds(CommonUtils.GetAnimationLength(anim, AnimationClips.appear));

                    anim.SetBool(AnimationBools.appear, false);
                    yield return new WaitForSeconds(0.2f);
                    direction = GetDirection(transform.position, isRightSide);
                }
            }

            if(!gameObject.activeSelf) yield break;

            if (CommonUtils.FiftyFifty)
            {
                FastBullet bullet = Shoot();
                yield return new WaitUntil(() => bullet.endOfAnimation);
            }
        }
    }

    FastBullet Shoot()
    {
        FastBullet bullet = GetBullet();
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    FastBullet GetBullet()
    {
        lock (EnemyManager.enemyBulletList[(int)Levels.Fast])
        {
            FastBullet bullet;

            bullet = EnemyManager.enemyBulletList[(int)Levels.Fast].Count == 0
                ? Instantiate(bulletPrefab, bulletsParent)
                : (FastBullet)EnemyManager.enemyBulletList[(int)Levels.Fast].Dequeue();

            bullet.front = front;
            bullet.rightSided = isRightSide;
            bullet.shooter = this;
            return bullet;
        }
    }
}
