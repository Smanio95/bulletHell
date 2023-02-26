using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSlider : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] float tileSize = 26.4f;
    [SerializeField] float offset = 2;
    [SerializeField] GameObject[] levels;
    [SerializeField] Transform[] levelOneTiles;
    [SerializeField] Transform[] levelTwoTiles;
    [SerializeField] Transform[] levelThreeTiles;

    private Transform[][] tiles = new Transform[3][];

    [Header("CameraSlide")]
    public float speed = 1;

    private int middle;
    private int length;

    private void Awake()
    {
        GameManager.OnLevelChange += LevelChange;
    }

    private void Start()
    {
        if (tiles.Length == 0)
        {
            Debug.Log("no tiles found!");
        }

        InitializeTiles();
    }

    private void OnDestroy()
    {
        GameManager.OnLevelChange -= LevelChange;
    }

    void Update()
    {
        if (GameManager.currentLevel >= 0)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.right);

            OffsetReach();
        }
    }

    void LevelChange()
    {

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(i == GameManager.currentLevel);
        }

        UpdateSizeVars();
    }

    void InitializeTiles()
    {
        tiles[0] = levelOneTiles;
        tiles[1] = levelTwoTiles;
        tiles[2] = levelThreeTiles;
    }

    void UpdateSizeVars()
    {
        length = tiles[GameManager.currentLevel].Length;
        middle = (length - 1) / 2;
    }

    void OffsetReach()
    {
        if (transform.position.x > tiles[GameManager.currentLevel][middle].position.x + tileSize / 2 - offset)
        {
            RepositionateTiles();
        }
    }

    void RepositionateTiles()
    {
        if (tiles.Length == 0 || length == 0) return;

        Transform firstT = tiles[GameManager.currentLevel][0];
        firstT.position = new Vector3(tiles[GameManager.currentLevel][length - 1].position.x + tileSize / 2, firstT.position.y, firstT.position.z);

        for (int i = 0; i < length; i++)
        {
            if (i < length - 1)
            {
                tiles[GameManager.currentLevel][i] = tiles[GameManager.currentLevel][i + 1];
            }
            else
            {
                tiles[GameManager.currentLevel][i] = firstT;
            }
        }

    }
}
