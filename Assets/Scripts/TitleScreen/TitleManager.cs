using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;

public class TitleManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject instructions;
    public GameObject credits;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Start the game.
    public void StartGame()
    {
        Debug.Log("Starting game");
        SceneManager.LoadScene("Level");
    }
    // Quit the game.
    public void QuitGame()
    {
        Debug.Log("Quitting game");
        UnityEngine.Application.Quit();
    }
    // Swap to Instructions Screen.
    public void Instructions()
    {
        mainMenu.SetActive(false);
        credits.SetActive(false);
        instructions.SetActive(true);
    }
    // Return to main menu.
    public void BackButton()
    {
        mainMenu.SetActive(true);
        instructions.SetActive(false);
        credits.SetActive(false);
    }
    // Swap to Credits Screen.
    public void Credits() { 
        credits.SetActive(true);
        instructions.SetActive(false);
        mainMenu.SetActive(false);
    }
}
