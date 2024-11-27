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
    // Start is called before the first frame update
    void Start()
    {
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
        gameOverScreen.SetActive(true);
    }
    // Restart Game when Restart button is clicked.
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Return to Title Screen.
    public void MainMenu() {
        SceneManager.LoadScene("TitleScreen");
    }
    // Add score when enemies die, will be called from a reference in the enemy superclass.
    public void AddScore(float addedScore) {
        score += addedScore;
        scoreText.text = "Score: " + score;
    }
}
