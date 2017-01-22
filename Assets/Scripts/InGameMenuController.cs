using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuController : MonoBehaviour
{

    //add the main game scene name here to redirect to it
    private string menuScene = "main menu";

    public Canvas inGameMenu;

    public Button toMainMenuButton;
    public Button restartLevelButton;

    // Use this for initialization
    void Start()
    {
        toMainMenuButton = toMainMenuButton.GetComponent<Button>();
        restartLevelButton = restartLevelButton.GetComponent<Button>();
        inGameMenu = inGameMenu.GetComponent<Canvas>();

        inGameMenu.enabled = false;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void RestartLevel()
    {
        CloseMenu();
        Scene levelScene = SceneManager.GetActiveScene();//should always be draw wave
        SceneManager.LoadScene(levelScene.buildIndex);
        SceneManager.SetActiveScene(levelScene);
    }

    public void OpenMenu()
    {
        inGameMenu.enabled = true;
    }

    public void CloseMenu()
    {
        inGameMenu.enabled = false;
    }



    // Update is called once per frame
    void Update()
    {

    }
}

