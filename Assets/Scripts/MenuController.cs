using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    //add the main game scene name here to redirect to it
    private string levelScene = "drum wave";

    public Canvas exitMenu;
    public Button newGameButton;
    public Button exitButton;
	public Animator IntroAnim;
   
    void Start () {

        exitMenu = exitMenu.GetComponent<Canvas>();
        newGameButton = newGameButton.GetComponent<Button>();
        exitButton = exitButton.GetComponent<Button>();

        exitMenu.enabled = false;
	}

    /// <summary>
    /// Redirects to the level scene
    /// </summary>
    public void StartGame()
    {
		IntroAnim.SetTrigger ("outro");
		StartCoroutine ("StartGameAfterDelay");
    }

	IEnumerator StartGameAfterDelay() {
		yield return new WaitForSeconds (2.5f);
		SceneManager.LoadScene(levelScene);
	}

    /// <summary>
    /// Creates a pop up for confirmation
    /// </summary>
    public void ExitGame()
    {
        exitMenu.enabled = true;
        newGameButton.enabled = false;
        exitButton.enabled = false;

    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void YesExitPress()
    {
        Application.Quit();
    }

    /// <summary>
    /// Return to main menu
    /// </summary>
    public void NoExitPress()
    {
        exitMenu.enabled = false;
        newGameButton.enabled = true;
        exitButton.enabled = true;
    }    
}
