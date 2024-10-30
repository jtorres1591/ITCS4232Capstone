using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SuperclassEnemyProperties : MonoBehaviour
{
    // This script is intended to be a superclass for enemies. Any properties that all enemies will share should be here.
    protected Rigidbody2D enemyRb;
    protected CircleCollider2D enemyCollider;
    protected PlayerControls playerScript;
    // General Stats.
    [SerializeField] protected float health = 1.0f;
    [SerializeField] protected float speed = 2.0f;
    // Aggro stats.
    [SerializeField] protected bool aggro = false;
    [SerializeField] protected float aggroRange = 6.0f;
    [SerializeField] protected float callRange = 8.0f;
    // Launching Stats.
    [SerializeField] protected float launchSpeed = 20.0f;
    [SerializeField] protected float launchTime = 4.0f;
    [SerializeField] protected float launchMass = 1.0f;
    // Wandering Stats.
    [SerializeField] protected float wanderRange = 5.0f;
    [SerializeField] protected float wanderInterval = 4.0f;
    private float wanderTimer = 0.0f;
    private Vector2 startPos;
    private Vector2 wanderPos;
    // Invincibility frame to prevent the same bullet from damaging twice.
    private bool vulnerable = true;
    [SerializeField] private float damageCooldown = 0.1f;
    // Item Drop. Randomize a number from 0 to 63, and if it is below dropRate, the item drops.
    [SerializeField] private GameObject dropItem;
    [SerializeField] private float dropRate;
    // Player reference.
    private Transform playerTransform;
    // Physics reference. Finds direction projectile hits from.
    Vector3 projectileDirection;
    [SerializeField] private PhysicsMaterial2D launchMaterial;
    // Enemy actions.
    private enum EnemyAction { 
    Wander,
    Chase,
    Strafe,
    Launched
    }
    private EnemyAction currentAction;
    // Start is called before the first frame update
    void Start()
    {
        // Get player transform.
        GameObject player = GameObject.Find("Player");
        if (player != null) playerTransform = player.transform;
        // Get the enemy's rigidbody, collider and the player's script to be able to deal damage.
        enemyRb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<CircleCollider2D>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerControls>();
        // Initialize Wandering.
        startPos = transform.position;
        SetWanderGoal();
        currentAction = EnemyAction.Wander;
    }

    // Update is called once per frame
    void Update()
    {
        
        // Wander so long as enemy either has not had aggro triggered or health has not run out.
        if (health >= 1.0f && !aggro) currentAction = EnemyAction.Wander;
        // If enemy is alive and aggro is active, aggro call.
        if (health >= 1.0f && aggro) AggroCall();
        // If enemy is alive, aggro is off, and player exists, check distance.
        if (health >= 1.0f && !aggro && playerTransform != null) DistanceCheck();
        // If enemy is alive and aggro is on, chase.
        if (health >= 1.0f && aggro) currentAction = EnemyAction.Chase;
        // Select action based on Enum. Make sure this is last in Update, and stays in Update.
        switch (currentAction) { 
            case EnemyAction.Wander:
                Wander(); break;
            case EnemyAction.Chase:
                Chase(); break;
            case EnemyAction.Strafe:
                break;
            case EnemyAction.Launched:
                break;
        }
    }
    // Override this function in child classes. Doing so will change the conditions for behaviors.
    private void EnemyBehavior() { 
    
    }
    // Damage player on collision.
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //UnityEngine.Debug.Log("Collision Detected");
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.DamagePlayer();
        }
        // Case for launched enemy colliding with enemy.
        if (collision.gameObject.CompareTag("PlayerAttack")) {
            // Remember direction projectile hit from.
            projectileDirection = -(collision.transform.position - transform.position).normalized;
            // Destroy the player's bullet that hit.
            Destroy(collision.gameObject);
            // Damage enemy.
            DamageEnemy();
        }
        
    }
    // Attack Collision
    public void OnTriggerEnter2D(Collider2D other)
    {
        //UnityEngine.Debug.Log("Trigger Detected");
        if (other.gameObject.CompareTag("PlayerAttack") && vulnerable && health >= 1.0f)
        {
            // Remember direction projectile hit from.
            projectileDirection = -(other.transform.position - transform.position).normalized;
            // Destroy the player's bullet that hit.
            Destroy(other.gameObject);
            // Damage enemy.
            DamageEnemy();

        }
        // Case for destroying weak walls. Weak Walls have a hitbox as a parent object so that the child object of the wall itself is destroyed with it.
        if (other.gameObject.CompareTag("WeakWall") && currentAction == EnemyAction.Launched) Destroy(other.gameObject);

    }
    // Script to damage an enemy and destroy it if it runs out of health.
    public virtual void DamageEnemy()
    {
        
        health--;
        // Set Aggro.
        SetAggro(true);
        StartCoroutine(DamageCooldown());
        //UnityEngine.Debug.Log("Damaged Enemy");
        if (health <= 0)
        {
            Launch(projectileDirection);
            //EnemyDeath();
        }
        else {
            // TEST: DOES TEMPORARILY STOPPING MOVEMENT STOP THE PERMANENT WRONG MOVEMENT AFTER DAMAGE. IT SEEMS SO, BUT CHECK BACK ON THIS LATER.
            enemyRb.velocity = Vector2.zero;
            // TODO: ADD SOUND EFFECT.
        }
       
    }
    // Invincibility Frames for enemy.
    public IEnumerator DamageCooldown()
    {
        vulnerable = false;
        yield return new WaitForSeconds(damageCooldown);
        vulnerable = true;
    }
    // Launch enemy and set a timer before destroying the Game Object.
    public void Launch(Vector2 direction) {
        // Set enemy action. The enemy should not be able to move independently while launched.
        currentAction = EnemyAction.Launched;
        // Change Physics Material.
        enemyCollider.sharedMaterial = launchMaterial;
        // Chance to drop item.
        ItemDrop();
        // Change Mass.
        enemyRb.mass = launchMass;
        // Unfreeze Constraints.
        enemyRb.constraints = RigidbodyConstraints2D.None;
        // Set tag to PlayerAttack.
        gameObject.tag = "PlayerAttack";
        // Set IsTrigger to true. TODO: MAY NOT BE NECCESSARY.
        //enemyCollider.isTrigger = true;
        // Apply force.
        enemyRb.AddForce(direction * launchSpeed, ForceMode2D.Impulse);
        // Trigger countdown for enemy death.
        StartCoroutine(EnemyDeath());
    }
    // Destroy the enemy, and trigger anything you want to happen upon death.
    public IEnumerator EnemyDeath()
    {
        yield return new WaitForSeconds(launchTime);
        // TODO: ADD SOUND AND VISUAL EFFECTS
        Destroy(gameObject);
    }
    // Enemy wanders before triggering aggro against player.
    private void Wander() {
        wanderTimer += Time.deltaTime;

        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, wanderPos, speed * Time.deltaTime);

        // If we reached the target position or the time interval has passed, set a new target position
        if (wanderTimer > wanderInterval)
        {
            SetWanderGoal();
            wanderTimer = 0f; // Reset the timer
        }
    }
    // Set target for Wandering.
    private void SetWanderGoal() {
        // Generate a random point within the wander range
        Vector2 randomOffset = new Vector2(Random.Range(-wanderRange, wanderRange), Random.Range(-wanderRange, wanderRange));
        wanderPos = (Vector2)startPos + randomOffset;

        // Ensure the target position stays within the range
        wanderPos = Vector2.ClampMagnitude(wanderPos - (Vector2)startPos, wanderRange) + (Vector2)startPos;
    }
    // Set enemy Aggro.
    public void SetAggro(bool aggroChange) { 
    aggro = aggroChange;
    }
    // Call surrounding enemies to get their aggro.
    private void AggroCall() {
        // Detect objects within the detection range with the specified tag
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, callRange);

        foreach (var obj in objectsInRange)
        {
            if (obj.CompareTag("Enemy"))
            {
                SuperclassEnemyProperties otherEnemies = obj.GetComponent<SuperclassEnemyProperties>();
                if (otherEnemies != null)
                {
                    otherEnemies.SetAggro(true); // Trigger aggro of surrounding enemies.
                }
            }
        }
    }
    // Check distance between player and enemy. Trigger aggro if close.
    private void DistanceCheck() {
        // Check the distance between the enemy and the player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        // If the player is within aggro range, set aggro to true.
        if (distanceToPlayer < aggroRange)
        {
            SetAggro(true);
        }
    }
    // Chase the player.
    private void Chase() {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
    }
    // Chance of dropping item on death.
    private void ItemDrop() {
        // If there is no item to be dropped, do nothing at all.
        if (dropItem != null)
        {
            float RNG = Random.Range(0, 64);
            if (RNG <= dropRate)
            {
                Instantiate(dropItem, transform.position, transform.rotation);
            }
        }
    }
    // FINAL LINE; PUT METHODS ABOVE HERE.
}
