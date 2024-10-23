using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class PlayerControls : MonoBehaviour
{
    // GameObjects
    [SerializeField] private GameObject playerProjectile;
    // Player Stats
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float maxHealth = 3.0f;
    [SerializeField] private float overHealTime = 4.0f;
    private float health;
    public bool overHeal = false;
    // GameManager
    private GameManager gameManager;
    // UI
    public TextMeshProUGUI healthText;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize Health.
        health = maxHealth;
        healthText.text = "Health: " + health;
        // GameManager.
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(healthText.color);
        // Walk
        PlayerMovement();
        // Shoot on Left Mouse Click.
        if (Input.GetMouseButtonDown(0)) ShootProjectile();
    }
    // Walking
    private void PlayerMovement() {
        // Get input from axis.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //Debug.Log(verticalInput);

        // Calculate movement direction.
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * walkSpeed * Time.deltaTime;
        // Apply movement to Player's position.
        transform.position += movement;
    }
    // Shoot projectiles when mouse is clicked.
    void ShootProjectile()
    {
        //Debug.Log("Click");
        // Instantiating not only needs object, but both position and rotation. Remember that.
        Instantiate(playerProjectile, transform.position, transform.rotation);
    }
    // Take Damage.
    public void DamagePlayer() {
        health--;
        // Update Health Text.
        healthText.text = "Health: " + health;
        // Die if health is zero.
        if (health <= 0) PlayerDeath();
    }
    // Heal Player.
    public void HealPlayer(float healHealth) {
        health+= healHealth;
        
        // Update Health Text.
        UpdateHealthText();
    }
    // Over time, gradually reduce overheal.
    public IEnumerator OverHeal(float overHealTime) { 
        yield return new WaitForSeconds(overHealTime);
        health--;
        // Update Health Text.
        UpdateHealthText();

    }
    public void UpdateHealthText() {
        // Set Overheal if it is true. If true, UI turns green, if false, it turns white.
        if (health > maxHealth)
        {
            overHeal = true;
            StartCoroutine(OverHeal(overHealTime));
            healthText.color = Color.green;
        }
        else { 
            overHeal=false;
            StopCoroutine(OverHeal(overHealTime));
            healthText.color = Color.white;
        }
        healthText.text = "Health: " + health;
    }
    // Player Death. Can be called manually if instant-death is implemented.
    public void PlayerDeath() {
        // Game Over
        gameManager.GameOver();
        // Deactivating is safer, so it is better than destroying the player object.
        gameObject.SetActive(false);
    }

}
