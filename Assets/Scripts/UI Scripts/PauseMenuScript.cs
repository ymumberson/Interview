using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public static bool Is_Paused;

    public GameObject pause_menu_UI;
    public GameObject ingame_menu_UI;

    private void Start()
    {
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Is_Paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pause_menu_UI.SetActive(false);
        ingame_menu_UI.SetActive(true);
        Time.timeScale = 1f;
        Is_Paused = false;
    }

    public void Pause()
    {
        pause_menu_UI.SetActive(true);
        ingame_menu_UI.SetActive(false);
        Time.timeScale = 0f;
        Is_Paused = true;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
