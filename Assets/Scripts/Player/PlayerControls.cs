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
    private bool alreadyOverHeal = false;
    // GameManager
    private GameManager gameManager;
    // UI
    public TextMeshProUGUI healthText;
    // Collider/RB
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private BoxCollider2D playerCollider;
    // References used for Physics Movement.
    private Vector3 movement;
    private float horizontalInput = 0.0f;
    private float verticalInput = 0.0f;
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
        
        // Shoot on Left Mouse Click.
        if (Input.GetMouseButtonDown(0)) ShootProjectile();
    }
    // Player Movement as an action goes in FixedUpdate.
    private void FixedUpdate()
    {
        // Walk
        PlayerMovement();
        if (movement.magnitude != 0)
        {
            PlayerMovementAction();
        }
        else {
            playerRb.velocity = Vector2.Lerp(playerRb.velocity, Vector2.zero, Time.fixedDeltaTime * 10f);
        }
    }
    // Walking
    private void PlayerMovement() {
        // Get input from axis.
        float newHorizontalInput = Input.GetAxis("Horizontal");
        float newVerticalInput = Input.GetAxis("Vertical");
        // If input changes, stop momentum.
        if(newHorizontalInput != horizontalInput || newVerticalInput != verticalInput) playerRb.velocity = Vector2.zero;
        // Apply new horizontal input to old.
        horizontalInput = newHorizontalInput;
        verticalInput = newVerticalInput;
        //Debug.Log(verticalInput);

        // Calculate movement direction.
        movement = new Vector3(horizontalInput, verticalInput, 0f) * walkSpeed * Time.deltaTime;
        // Apply movement to Player's position.
        //transform.position += movement;
        
    }
    // Walking Action.
    private void PlayerMovementAction() {
        // Physics Movement.
        playerRb.velocity = movement.normalized * walkSpeed;
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
        UpdateHealthText();
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
        if (overHeal) health--;
        // Update Health Text.
        UpdateHealthText();
        // Repeat if overheal is still active.
        if (health > maxHealth) StartCoroutine(OverHeal(overHealTime));
    }
    public void UpdateHealthText() {
        // Set Overheal if it is true. If true, UI turns green, if false, it turns white.
        if (health > maxHealth)
        {
            overHeal = true;
            if(!alreadyOverHeal) StartCoroutine(OverHeal(overHealTime));
            alreadyOverHeal = true;
            healthText.color = Color.green;
        }
        else { 
            overHeal=false;
            alreadyOverHeal = false;
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
    // Taking damage from Enemy Projectiles. Enemy damage is in the enemy superclass so exceptions can be managed, but projectiles should always damage.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyAttack")) {
            DamagePlayer();
            Destroy(collision.gameObject);
        }
    }

}
