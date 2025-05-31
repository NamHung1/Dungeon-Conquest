using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;

    public GameObject gameOverPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelText;
    public Button homeButton;

    private float playTime;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        homeButton.onClick.AddListener(HomeScreen);
        playTime = 0f;
    }

    void Update()
    {
        playTime += Time.deltaTime;
    }

    public void ShowGameOver(bool isWin)
    {
        gameOverPanel.SetActive(true);

        titleText.text = isWin ? "You Win!" : "Game Over";
        timeText.text = "Time Played: " + FormatTime(playTime);
        levelText.text = "Level Reached: " + GetCurrentLevel();

        Time.timeScale = 0f; 
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    int GetCurrentLevel()
    {
        DungeonManager dungeonManager = FindObjectOfType<DungeonManager>();
        if (dungeonManager != null)
        {
            return dungeonManager.currentLevel;
        }
        return 1;
    }

    void HomeScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScreen");
    }
}
