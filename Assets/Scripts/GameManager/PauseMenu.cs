using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingMenu;

    private bool isPaused = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC clicked");
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        isPaused = false; // Set the pause state to false
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        isPaused = true;
    }

    public void OpenSetting()
    {
        pauseMenu.SetActive(false);
        settingMenu.SetActive(true);
    }

    public void CloseSetting()
    {
        pauseMenu.SetActive(true);
        settingMenu.SetActive(false);
    }

    public void BackHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScreen"); // Load the main menu scene
    }
}
