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
    // Sound.
    public GameObject soundButton;
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
        //Debug.Log("Starting game");
        Instantiate(soundButton, transform.position, Quaternion.Euler(0, 0, 0));
        StartCoroutine(StartDelay());
    }
    // Ensure Button Sound Plays with delay.
    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("Level");
    }
    // Quit the game.
    public void QuitGame()
    {
        //Debug.Log("Quitting game");
        Instantiate(soundButton, transform.position, Quaternion.Euler(0, 0, 0));
        StartCoroutine(QuitDelay());
    }
    // Ensure Button Sound Plays with delay.
    public IEnumerator QuitDelay()
    {
        yield return new WaitForSeconds(0.1f);
        UnityEngine.Application.Quit();
    }
    // Swap to Instructions Screen.
    public void Instructions()
    {
        Instantiate(soundButton, transform.position, Quaternion.Euler(0, 0, 0));
        mainMenu.SetActive(false);
        credits.SetActive(false);
        instructions.SetActive(true);
    }
    // Return to main menu.
    public void BackButton()
    {
        Instantiate(soundButton, transform.position, Quaternion.Euler(0, 0, 0));
        mainMenu.SetActive(true);
        instructions.SetActive(false);
        credits.SetActive(false);
    }
    // Swap to Credits Screen.
    public void Credits() {
        Instantiate(soundButton, transform.position, Quaternion.Euler(0, 0, 0));
        credits.SetActive(true);
        instructions.SetActive(false);
        mainMenu.SetActive(false);
    }
}
