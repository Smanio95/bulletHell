using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("LevelChange")]
    [SerializeField] Image changeLvlPanel;
    [SerializeField] Text changeLvlText;
    [SerializeField] float fadingDecaSeconds = 30;
    [SerializeField] string baseText = "ROUND ";

    [Header("GameStatus")]
    [SerializeField] GameObject[] menus;
    [SerializeField] Text gameOverText;
    [SerializeField] Text gameOverSubText;
    [SerializeField] string winText = "you won!";
    [SerializeField] string subText = "ships destroyed: ";
    [SerializeField] string lossText = "game over";


    [Header("Audio")]
    [SerializeField] AudioSource mainSource;
    [SerializeField] AudioClip bossDeathClip;
    private static float sourceTime = 0;


    private Color originalPanelClr;
    private Color originalTextClr;

    public static GameStatus gameStatus = GameStatus.StartMenu;

    public enum GameStatus
    {
        StartMenu,
        Running,
        Pause,
        GameOver
    }

    private void Awake()
    {
        originalPanelClr = changeLvlPanel.color;
        originalTextClr = changeLvlText.color;
    }

    private void Start()
    {
        mainSource.time = sourceTime;
    }

    private void Update()
    {
        switch (gameStatus)
        {
            case GameStatus.StartMenu:
                EnableCanvas((int)MenuIndex.StartMenu);
                Pause();
                break;
            case GameStatus.Running:
                EnableCanvas(-1);
                DetectInput();
                Resume();
                break;
            case GameStatus.Pause:
                EnableCanvas((int)MenuIndex.Pause);
                DetectInput();
                Pause();
                break;
            case GameStatus.GameOver:
                EnableCanvas((int)MenuIndex.End);
                Pause();
                break;
            default:
                break;
        }
    }

    private void DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameStatus == GameStatus.Pause)
            {
                gameStatus = GameStatus.Running;
                return;
            }
            gameStatus = GameStatus.Pause;
        }
    }

    private void EnableCanvas(int index)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(i == index);
        }
    }

    public void ResumeGame()
    {
        gameStatus = GameStatus.Running;
    }

    public void MainMenu()
    {
        gameStatus = GameStatus.StartMenu;
        SceneManager.LoadScene("SampleScene");
    }

    public void RestartGame()
    {
        sourceTime = mainSource.time;
        gameStatus = GameStatus.Running;
        SceneManager.LoadScene("SampleScene");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    public void GameOver(int enemyKilled, bool winner = false)
    {
        if (!winner)
        {
            gameOverText.text = lossText;
        }
        else
        {
            mainSource.PlayOneShot(bossDeathClip);
            gameOverText.text = winText;
        }
        gameOverSubText.text = subText + enemyKilled;
        gameStatus = GameStatus.GameOver;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeLevelStart()
    {
        changeLvlPanel.color = originalPanelClr;
        changeLvlText.color = originalTextClr;
        changeLvlText.text = baseText + (GameManager.currentLevel + 1).ToString();
    }

    public IEnumerator ChangeLevelEnd()
    {
        float panelLerpAlpha;
        float textLerpAlpha;
        float rate = (1 / fadingDecaSeconds);
        float interpolation = 0;

        while (changeLvlPanel.color.a > 0.01f || changeLvlText.color.a > 0.01f)
        {
            interpolation += Time.deltaTime * rate;
            panelLerpAlpha = changeLvlPanel.color.a - changeLvlPanel.color.a * interpolation;
            textLerpAlpha = changeLvlText.color.a - changeLvlText.color.a * interpolation;

            changeLvlPanel.color = new Color(originalPanelClr.r, originalPanelClr.g, originalPanelClr.b, panelLerpAlpha);
            changeLvlText.color = new Color(originalTextClr.r, originalTextClr.g, originalTextClr.b, textLerpAlpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        changeLvlPanel.color = new Color(originalPanelClr.r, originalPanelClr.g, originalPanelClr.b, 0);
        changeLvlText.color = new Color(originalTextClr.r, originalTextClr.g, originalTextClr.b, 0);
    }

}
