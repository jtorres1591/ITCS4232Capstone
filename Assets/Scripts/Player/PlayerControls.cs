using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using TMPro.Examples;

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
    // Invincibility Frames.
    [SerializeField] private float damageCooldown = 0.4f;
    private bool vulnerable = true;
    // GameManager
    private GameManager gameManager;
    // UI
    public TextMeshProUGUI healthText;
    // Collider/RB
    [SerializeField] private Rigidbody2D playerRb;
    // Circle Collider prevents the player from snagging on walls. Also allows the player to squeeze through 1x1 gaps.
    [SerializeField] private CircleCollider2D playerCollider;
    // References used for Physics Movement.
    private Vector3 movement;
    private float horizontalInput = 0.0f;
    private float verticalInput = 0.0f;
    // Sound Objects.
    [SerializeField] private GameObject soundPlayerDamage;
    [SerializeField] private GameObject soundPlayerDeath;
    // ANIMATION.
    [SerializeField] private GameObject spriteGameObject;
    [SerializeField] private SpriteRenderer spriteRenderer;
    // Flags; Positive Horizontal is Right
    private bool positiveHor = true;
    private bool facingRight = true;
    // Float used for idle directions.
    private float currentDirection = 0;
    // Float used for walk cycle. Ranges from zero to five.
    private float walkCycle = 0;
    // Sprites.
    [SerializeField] private Sprite idleSpriteUp;
    [SerializeField] private Sprite idleSpriteDown;
    [SerializeField] private Sprite idleSpriteSide;
    [SerializeField] private Sprite[] walkSpriteUp;
    [SerializeField] private Sprite[] walkSpriteDown;
    [SerializeField] private Sprite[] walkSpriteSide;
    // VFX
    [SerializeField] private GameObject deathVFX;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize Health.
        health = maxHealth;
        healthText.text = "Health: " + health;
        // GameManager.
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Start WalkCycle to loop so long as it is active.
        StartCoroutine(WalkCycleActive());
    }
    // Walk Cycle animation is always active.
    private IEnumerator WalkCycleActive() {
        yield return new WaitForSeconds(0.2f);
        if (walkCycle < 5)
        {
            walkCycle += 1;
        }
        else {
            walkCycle = 0;
        }
        //Debug.Log(walkCycle);
        StartCoroutine(WalkCycleActive());
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
        // ANIMATION. CAPS FOR EMPHASIS.
        // IF HORIZONTAL AND VERTICAL ARE BOTH ZERO, IDLE.
        if (horizontalInput == 0 && verticalInput == 0)
        {
            GetIdleSprite(currentDirection);
        }
        // DECIDE BETWEEN HORIZONTAL AND VERTICAL DEPENDING ON THE GREATER ABSOLUTE VALUE.
        else if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput))
        {
            // IF VERTICAL IS POSITIVE, FACE UP.
            if (verticalInput > 0)
            {
                //Debug.Log("Up");
                currentDirection = 0;
                GetWalkSprite(currentDirection);
            }
            // IF VERTICAL IS NEGATIVE, FACE DOWN.
            else {
                //Debug.Log("Down");
                currentDirection = 1;
                GetWalkSprite(currentDirection);
            }
        }
        else
        {
            currentDirection = 2;
            GetWalkSprite(currentDirection);
            // IF HORIZONTAL IS NEGATIVE, FACE LEFT.
            if (horizontalInput < 0)
            {
                //Debug.Log("Left");
                positiveHor = false;
            }
            // IF HORIZONTAL IS POSITIVE, FACE RIGHT.
            else {
                //Debug.Log("Right");
                positiveHor = true;
            }
            if (positiveHor != facingRight) FlipSprite();
        }
    }
    // ANIMATION. FLIP SPRITE WHEN HORIZONTAL.
    private void FlipSprite() {
        Vector3 scale = spriteGameObject.transform.localScale;
        
        // Flip sprite by changing the x value of the local scale.
        scale.x = -scale.x;

        // Apply the new scale.
        spriteGameObject.transform.localScale = scale;
        if (facingRight)
        {
            facingRight = false;
        }
        else {
            facingRight = true;
        }
    }
    private void GetIdleSprite(float choice) {
        switch (choice) {
            case 0:
                spriteRenderer.sprite = idleSpriteUp;
                break;
            case 1:
                spriteRenderer.sprite = idleSpriteDown;
                break;
            case 2:
                spriteRenderer.sprite = idleSpriteSide;
                break;
        }
    }
    private void GetWalkSprite(float choice)
    {
        switch (choice)
        {
            case 0:
                spriteRenderer.sprite = walkSpriteUp[Mathf.FloorToInt(walkCycle)];
                break;
            case 1:
                spriteRenderer.sprite = walkSpriteDown[Mathf.FloorToInt(walkCycle)];
                break;
            case 2:
                spriteRenderer.sprite = walkSpriteSide[Mathf.FloorToInt(walkCycle)];
                break;
        }
    }
    // ANIMATION. DECIDE FRAME OF ANIMATION.

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
        // Cancel if not vulnerable.
        if (!vulnerable) return;
        health--;
        // Start Damage Cooldown.
        StartCoroutine(DamageCooldown());
        // Update Health Text.
        UpdateHealthText();
        // Die if health is zero.
        if (health <= 0)
        {
            Instantiate(soundPlayerDeath, transform.position, Quaternion.Euler(0, 0, 0));
            PlayerDeath();
        }
        else {
            Instantiate(soundPlayerDamage, transform.position, Quaternion.Euler(0, 0, 0));
        }
    }
    // Damage Cooldown/Invincibility Frames.
    public IEnumerator DamageCooldown()
    {
        vulnerable = false;
        yield return new WaitForSeconds(damageCooldown);
        vulnerable = true;
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
        Instantiate(deathVFX, transform.position, UnityEngine.Quaternion.Euler(0, 0, 0));
        // Game Over
        gameManager.GameOver();
        // Deactivating is safer, so it is better than destroying the player object.
        gameObject.SetActive(false);
    }
    // Taking damage from Enemy Projectiles. Enemy damage is in the enemy superclass so exceptions can be managed, but projectiles should always damage.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyAttack") || collision.gameObject.CompareTag("Explosion")) {
            DamagePlayer();
            Destroy(collision.gameObject);
        }
    }

}
