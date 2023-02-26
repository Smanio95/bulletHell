using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] Transform[] parents;

    [Header("Levels")]
    public Level[] levelInfo;

    [Header("Bullets")]
    [SerializeField] Transform[] bulletParents;

    [Header("Boss")]
    [SerializeField] BossEnemy bossPrefab;
    [SerializeField] Transform bossFinalPosition;
    [SerializeField] float bossSpawnSpeed = 2;
    [SerializeField] float rightEdgeOffset = 10;
    [SerializeField] Transform _rightEdge;
    [SerializeField] Transform bossBulletParent;
    [SerializeField] Transform bossEnemyParent;

    public static List<Queue<Enemy>> enemyList;
    public static List<Queue<EnemyBullet>> enemyBulletList;

    private float topEdge;
    private float rightEdge;
    private Camera mainCam;

    private float enemySize;
    private int enemyToSpawn;
    private float elapsed;

    private int enemyListMaxLength = 0;
    private int instantiateThreshold = 3;
    private int maxEnemyInSingleCol = 2;
    private int minEnemiesToSpawn;
    private int maxEnemyToSpawn;
    private float instantiatingTime;
    private int enemyPlaced = 0;
    private Enemy enemy;
    private BossEnemy boss;
    public static bool endOfLevel = true;

    public static readonly string LOCKER = "LOCKER";
    public static bool bossIsDead = false;

    public enum EnemyTypes
    {
        Standard,
        Tank,
        Fast,
        Boss
    }

    private void Awake()
    {
        GameManager.OnLevelChange += ResetVars;
        endOfLevel = true;
        bossIsDead = false;
    }

    void Start()
    {
        enemyList = new List<Queue<Enemy>>();
        enemyBulletList = new List<Queue<EnemyBullet>>();

        foreach (Level _ in levelInfo)
        {
            enemyList.Add(new Queue<Enemy>());
            enemyBulletList.Add(new Queue<EnemyBullet>());
        }

        InitializeCameraVars();
    }

    private void OnDestroy()
    {
        GameManager.OnLevelChange -= ResetVars;
    }

    void ResetVars()
    {
        endOfLevel = false;

        if (GameManager.bossLevel)
        {
            StartCoroutine(InstantiateBossLevel());
            return;
        }

        enemy = levelInfo[GameManager.currentLevel].enemy;
        instantiateThreshold = levelInfo[GameManager.currentLevel].instantiateThreshold;
        maxEnemyInSingleCol = levelInfo[GameManager.currentLevel].maxEnemyInSingleCol;
        minEnemiesToSpawn = levelInfo[GameManager.currentLevel].minEnemiesToSpawn;
        maxEnemyToSpawn = levelInfo[GameManager.currentLevel].maxEnemyToSpawn;

        enemyListMaxLength = 0;

        enemyPlaced = 0;

        enemySize = enemy
            .transform
            .GetComponent<CharacterController>()
            .radius;

        enemyToSpawn = Random.Range(minEnemiesToSpawn, maxEnemyToSpawn);

        ResetTime(true);
    }

    void Update()
    {
        if (!GameManager.bossLevel) InstantiateEnemy();
    }

    private bool CheckBossIsDead()
    {
        return boss != null && !boss.gameObject.activeSelf;
    }

    void CheckEndOfLevel()
    {
        if (enemyList[GameManager.currentLevel].Count < enemyListMaxLength)
            return;

        bool isOneActive = false;

        foreach(Enemy enemy in enemyList[GameManager.currentLevel])
        {
            if (enemy.gameObject.activeSelf)
            {
                isOneActive = true;
                break;
            }
        }

        if(!isOneActive)
            endOfLevel = true;
    }

    void InitializeCameraVars()
    {
        mainCam = Camera.main;
        topEdge = mainCam.orthographicSize;
        rightEdge = topEdge * mainCam.aspect;
    }

    void InstantiateEnemy()
    {
        if (enemyPlaced == enemyToSpawn && GameManager.currentLevel > -1)
        {
            CheckEndOfLevel();
            return;
        }

        if (elapsed >= instantiatingTime)
        {
            InitializeEnemy();
            ResetTime();
            enemyPlaced++;
        }
        elapsed += Time.deltaTime;
    }

    void ResetTime(bool firstTime = false)
    {
        elapsed = 0;
        instantiatingTime = firstTime
            ? Random.Range(1, instantiateThreshold / 2)
            : Random.Range(instantiateThreshold - 1, instantiateThreshold + 1);
    }

    private void InitializeEnemy()
    {
        List<float> yPositions = new List<float>();
        List<float> xPositions = new List<float>();
        List<Quaternion> rotations = new List<Quaternion>();

        switch (enemy.type)
        {
            case EnemyTypes.Standard:
                RetrievePosAndRotation(yPositions, xPositions, rotations, mainCam.transform.position.x + rightEdge + 1);
                break;
            case EnemyTypes.Tank:
                float[] minMax = { mainCam.transform.position.x + rightEdge / 2, mainCam.transform.position.x + rightEdge + 1 };
                RetrievePosAndRotation(yPositions, xPositions, rotations, minMax);
                break;
            case EnemyTypes.Fast:
                RetrievePosAndRotation(yPositions, xPositions, rotations);
                break;
            default:
                break;
        }

        for (int i = 0; i < yPositions.Count; i++)
        {
            PlaceEnemy(xPositions[i], yPositions[i], rotations[i]);
        }
    }

    void RetrievePosAndRotation(List<float> yPositions, List<float> xPositions, List<Quaternion> rotations, float xPos = Mathf.Infinity)
    {
        int numberOfEnemies = Random.Range(1, maxEnemyInSingleCol);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            float yPos = Random.Range(-1 * topEdge, topEdge);
            for (int j = 0; j < yPositions.Count; j++)
            {
                if (yPos < yPositions[j] + enemySize / 2 && yPos > yPositions[j] - enemySize / 2)
                {
                    yPos = Random.Range(-1 * topEdge, topEdge);
                    j = 0;
                }
            }
            yPositions.Add(yPos);
            if (xPos == Mathf.Infinity)
            {
                bool isRightSide = CommonUtils.FiftyFifty;
                if (isRightSide)
                {
                    xPositions.Add(mainCam.transform.position.x + rightEdge + 0.3f);
                    rotations.Add(enemy.transform.rotation);
                }
                else
                {
                    xPositions.Add(mainCam.transform.position.x - rightEdge - 0.3f);
                    rotations.Add(Quaternion.Euler(new Vector3(0, 0, 180)));
                }
            }
            else
            {
                xPositions.Add(xPos);
                rotations.Add(enemy.transform.rotation);
            }
        }
    }

    void RetrievePosAndRotation(List<float> yPositions, List<float> xPositions, List<Quaternion> rotations, float[] minMaxXPos)
    {
        int numberOfEnemies = Random.Range(1, maxEnemyInSingleCol);
        bool onEdge = CommonUtils.FiftyFifty;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            float yPos;
            float xPos;
            Quaternion rotation;
            if (!onEdge)
            {
                yPos = Random.Range(-1 * topEdge, topEdge);
                xPos = mainCam.transform.position.x + rightEdge + 0.5f;
                rotation = enemy.transform.rotation;
            }
            else
            {
                bool isUp = CommonUtils.FiftyFifty;
                if (isUp)
                {
                    yPos = topEdge + 1;
                    rotation = Quaternion.Euler(EnemyConstants.DOWNWARD_ROTATED);
                }
                else
                {
                    yPos = -1 * topEdge - 1;
                    rotation = Quaternion.Euler(EnemyConstants.UPWARD_ROTATED);
                }
                if (minMaxXPos.Length == 2)
                {
                    xPos = Random.Range(minMaxXPos[0], minMaxXPos[1]);
                }
                else
                {
                    xPos = mainCam.transform.position.x + rightEdge + 0.5f;
                }
            }

            onEdge = !onEdge;

            yPositions.Add(yPos);
            xPositions.Add(xPos);
            rotations.Add(rotation);
        }
    }



    void PlaceEnemy(float xPos, float yPos, Quaternion rotation)
    {
        Enemy enemyClone;
        Vector3 position = new Vector3(xPos, yPos, enemy.transform.position.z);
        if (enemyList[GameManager.currentLevel].Count == 0)
        {
            enemyListMaxLength++;
            enemyClone = Instantiate(enemy, position, rotation, parents[GameManager.currentLevel]);
            enemyClone.bulletsParent = bulletParents[GameManager.currentLevel];
        }
        else
        {
            enemyClone = enemyList[GameManager.currentLevel].Dequeue();
            enemyClone.transform.SetPositionAndRotation(position, rotation);
        }
        enemyClone.gameObject.SetActive(true);
    }

    public IEnumerator InstantiateBossLevel()
    {

        Vector3 initialPos = new Vector3(mainCam.transform.position.x + rightEdge, bossPrefab.transform.position.y, bossPrefab.transform.position.z);
        float finalPos = bossFinalPosition.position.x;

        _rightEdge.position = new Vector3(finalPos - rightEdgeOffset, _rightEdge.position.y, _rightEdge.position.z);

        boss = Instantiate(bossPrefab, initialPos, bossPrefab.transform.rotation);
        Transform bossTransform = boss.transform;

        boss.type = EnemyTypes.Boss;
        boss.gameObject.SetActive(true);
        boss.bossEnemyParent = bossEnemyParent;
        boss.bossBulletParent = bossBulletParent;
        boss.enabled = false;

        while(boss.transform.position.x > finalPos)
        {
            bossTransform.Translate(-1 * bossSpawnSpeed * Time.deltaTime * Vector3.right);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        boss.enabled = true;

        while (true)
        {
            if (CheckBossIsDead())
            {
                bossIsDead = true;
                yield break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

    }
}
