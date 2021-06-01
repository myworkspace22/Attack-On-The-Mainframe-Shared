using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject ui;

    public string menuSceneName = "StartMenu";

    public Screenfader sceneFader;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(BuildManager.instance.selectedNode != null)
            {
                BuildManager.instance.DeselectNode();
            }else if(BuildManager.instance.CanBuild){
                BuildManager.instance.DeselectShopItem();
            }
            else
            {
                Toggle();
            }
        }
    }
    public void Toggle()
    {
        if (GameManager.GameIsOver)
            return;

        ui.SetActive(!ui.activeSelf);

        if (ui.activeSelf)
        {
            Time.timeScale = 0f;
        } else if (!BuildManager.instance.GetComponent<WaveSpawner>().BuildMode){
            Time.timeScale = BuildManager.instance.GetComponent<WaveSpawner>().gameSpeed;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    public void Retry()
    {
        Toggle();
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }
    public void Menu()
    {
        Toggle();
        sceneFader.FadeTo(menuSceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
