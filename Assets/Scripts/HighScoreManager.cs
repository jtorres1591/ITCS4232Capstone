using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static float highScore = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // This will ensure only one instance of the GameManager exists across scenes
    private void Awake()
    {
        // Check if another instance already exists
        if (FindObjectsOfType<HighScoreManager>().Length > 1)
        {
            Destroy(gameObject); // Destroy the duplicate
        }
        else
        {
            DontDestroyOnLoad(gameObject); // Keep this GameObject across scenes
        }
    }
    // Method to update the high score if the current score is higher
    public bool UpdateHighScore(float currentScore)
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            // Use player prefs to keep high score persistent across loading.
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
            return true;
        }
        else { 
            return false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    // Returns high score.
    public float GetHighScore() {
        highScore = PlayerPrefs.GetFloat("HighScore", 0);
        return highScore;
    }
}
