using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public delegate void LevelChange();
    public static LevelChange OnLevelChange;

    [Header("CameraSlider")]
    [SerializeField] CameraSlider cameraSlider;
    [SerializeField] Transform portalPosition;

    [Header("Player")]
    public PlayerController player;
    [SerializeField] float bonusLevelEndHealth = 25f;

    [Header("Levels")]
    public static int currentLevel = -1;
    [SerializeField] Vector3 startPos;
    [SerializeField] float playerPositionOffset = 5;
    [SerializeField] UIManager UIM;
    [SerializeField] EnemyManager enemyManager;

    public static int maxLvls = 3;
    public static bool bossLevel = false;

    private bool repositionating = false;
    private bool gameOverScreen = false;
    private float initialCameraSpeed;

    private void Awake()
    {
        currentLevel = -1;
        bossLevel = false;
    }

    private void Start()
    {
        initialCameraSpeed = cameraSlider.speed;
        StartCoroutine(LevelChangeRoutine());
    }

    void Update()
    {
        if (!CommonUtils.CheckVisible(player.transform.position, true))
        {
            player.isDead = true;
        }

        if ((EnemyManager.bossIsDead || player.isDead) && !gameOverScreen)
        {
            GameOver();
        }

        if (EnemyManager.endOfLevel && !repositionating)
        {
            StartCoroutine(LevelChangeRoutine());
        }
    }

    private void GameOver()
    {
        player.endOfLevel = true;
        gameOverScreen = true;
        UIM.GameOver(player.enemyKilled, !player.isDead);
    }

    IEnumerator LevelChangeRoutine()
    {
        repositionating = true;
        cameraSlider.speed = 0;

        if (currentLevel < maxLvls - 1)
        {
            currentLevel++;

            //start UIChangeLevel
            UIM.ChangeLevelStart();

            if (currentLevel > 0)
            {
                player.endOfLevel = true;

                player.gameObject.layer = (int)Layers.Untouchable;

                float leftEdge = cameraSlider.transform.position.x - Camera.main.aspect * Camera.main.orthographicSize;
                float rightEdge = cameraSlider.transform.position.x + Camera.main.aspect * Camera.main.orthographicSize;

                while (player.transform.position.x < rightEdge + 1)
                {
                    player.transform.Translate(2 * (player.speed / 2) * Time.deltaTime * Vector3.right);
                    yield return new WaitForSeconds(Time.deltaTime);
                }

                player.characterController.enabled = false;
                player.transform.position = new Vector3(leftEdge + playerPositionOffset, startPos.y, startPos.z);
                player.characterController.enabled = true;

                player.gameObject.layer = (int)Layers.Player;

                player.health = Mathf.Clamp(player.health + bonusLevelEndHealth, player.health, player.originalHealth);
                Debug.Log(player.health);
                player.endOfLevel = false;

            }

            //end UIChangeLevel
            StartCoroutine(UIM.ChangeLevelEnd());

            cameraSlider.speed = initialCameraSpeed;
            repositionating = false;

            OnLevelChange.Invoke();

        }
        else
        {
            bossLevel = true;
            OnLevelChange.Invoke();
            Debug.Log("FINAL BOSS");
        }
    }
}
