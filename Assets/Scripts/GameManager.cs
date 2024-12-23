using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public TextMeshProUGUI scoreText;
    public float score;
    // Reference.
    public GameObject cameraRef;
    // Sounds
    public GameObject soundButton;
    // Notice for new High score.
    public GameObject highScoreNotice;
    // High Score Manager. Will need to be initialized in start.
    private HighScoreManager highScoreManager;
    // Start is called before the first frame update
    void Start()
    {
        highScoreManager = GameObject.Find("HighScoreManager").GetComponent<HighScoreManager>();
        // Score should always start at zero.
        score = 0.0f;
        scoreText.text = "Score: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Trigger Game Over.
    public void GameOver()
    {
        // Set High Score.
        bool isHighScore = highScoreManager.UpdateHighScore(score);
        // Game Over Screen.
        gameOverScreen.SetActive(true);
        // Notice for high score.
        if (isHighScore) highScoreNotice.SetActive(true);
    }
    // Restart Game when Restart button is clicked.
    public void RestartGame()
    {
        Instantiate(soundButton, cameraRef.transform.position, Quaternion.Euler(0, 0, 0));
        StartCoroutine(RestartDelay());
    }
    // Ensure Button Sound Plays with delay.
    public IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Return to Title Screen.
    public void MainMenu() {
        Instantiate(soundButton, cameraRef.transform.position, Quaternion.Euler(0, 0, 0));
        StartCoroutine(MainMenuDelay());
    }
    // Ensure Button Sound Plays with delay.
    public IEnumerator MainMenuDelay() {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("TitleScreen");
    }
    // Add score when enemies die, will be called from a reference in the enemy superclass.
    public void AddScore(float addedScore) {
        score += addedScore;
        scoreText.text = "Score: " + score;
    }
}
