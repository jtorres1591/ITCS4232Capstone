using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SuperclassEnemyProperties : MonoBehaviour
{
    // This script is intended to be a superclass for enemies. Any properties that all enemies will share should be here.
    protected Rigidbody2D enemyRb;
    protected CircleCollider2D enemyCollider;
    protected PlayerControls playerScript;
    protected GameObject player;
    protected GameManager gameManagerScript;
    // Score given on death and breaking walls.
    [SerializeField] protected float deathScore = 5.0f;
    [SerializeField] protected float wallScore = 1.0f;
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
    protected float wanderTimer = 0.0f;
    protected Vector2 startPos;
    protected Vector2 wanderPos;
    // Strafing Stats.
    [SerializeField] protected float strafeInterval = 2.5f;
    // Attacking Stats.
    // If attackInterval is zero, attacks will not occur.
    [SerializeField] protected float attackInterval = 4.0f;
    [SerializeField] protected GameObject enemyAttack;
    [SerializeField] protected GameObject enemyIndependentAttack;
    [SerializeField] protected GameObject enemyBomb;
    protected bool attackCooldown = false;
    // Triple Shot Angle Offset.
    [SerializeField] protected float tripleShotOffset = 45.0f;
    // Invincibility frame to prevent the same bullet from damaging twice.
    protected bool vulnerable = true;
    [SerializeField] protected float damageCooldown = 0.1f;
    // Item Drop. Randomize a number from 0 to 63, and if it is below dropRate, the item drops.
    [SerializeField] protected GameObject dropItem;
    [SerializeField] protected float dropRate;
    // Player reference.
    protected Transform playerTransform;
    // Physics reference. Finds direction projectile hits from.
    Vector3 projectileDirection;
    [SerializeField] protected PhysicsMaterial2D launchMaterial;
    // Enemy actions. Used to effect behavior.
    protected enum EnemyAction { 
    Idle,
    Wander,
    Chase,
    Strafe,
    Launched
    }
    protected EnemyAction currentAction;
    // Used to select specific enemy actions, for aggro and projectile choice, respectively.
    protected enum SelectEnemyAggroAction
    {
        Chase,
        Strafe
    }
    [SerializeField] protected EnemyAction selectedEnemyPeacefulAction;
    [SerializeField] protected EnemyAction selectedEnemyAggroAction;
    protected enum SelectEnemyProjectile
    {
        None,
        Basic,
        Triple,
        X,
        Cross,
        Bomb
    }
    [SerializeField] protected SelectEnemyProjectile selectedEnemyProjectile;
    // Sound Object variables.
    [SerializeField] protected GameObject soundWallBreak;
    [SerializeField] protected GameObject soundEnemyDeath;
    [SerializeField] protected GameObject soundEnemyLaunch;
    [SerializeField] protected GameObject soundEnemyRiccochet;
    [SerializeField] protected GameObject soundWallBounce;
    // VFX
    [SerializeField] protected GameObject deathVFX;
    // ANIMATION
    [SerializeField] protected GameObject spriteGameObject;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    // Sprites.
    [SerializeField] protected Sprite[] enemySprite;
    // Animation Cycle.
    protected float animationCounter = 0;
    // Start is called before the first frame update
    protected void Start()
    {
        // Get GameManager.
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Get player transform.
        player = GameObject.Find("Player");
        if (player != null) playerTransform = player.transform;
        // Get the enemy's rigidbody, collider and the player's script to be able to deal damage.
        enemyRb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<CircleCollider2D>();
        GameObject playerReference = GameObject.FindGameObjectWithTag("Player");
        if(playerReference != null) playerScript = playerReference.GetComponent<PlayerControls>();
        // Initialize Wandering.
        startPos = transform.position;
        SetWanderGoal();
        // Start with peaceful action.
        currentAction = selectedEnemyPeacefulAction;
        // Animate at all times.
        StartCoroutine(AnimationCycle());
    }
    //  Animation cycle is always active unless launched.
    private IEnumerator AnimationCycle()
    {
        spriteRenderer.sprite = enemySprite[Mathf.FloorToInt(animationCounter)];
        yield return new WaitForSeconds(0.2f);
        if (animationCounter < enemySprite.Length-1)
        {
            animationCounter += 1;
        }
        else
        {
            animationCounter = 0;
        }
        
        if(currentAction != EnemyAction.Launched) StartCoroutine(AnimationCycle());
    }
    // Update is called once per frame
    protected void Update()
    {
        // If player is gone, action becomes idle.
        if (player == null) currentAction = EnemyAction.Idle;
        // Enemy Behavior.
        EnemyBehavior();
        // Perform Peaceful Action. so long as enemy either has not had aggro triggered or health has not run out.
        if (health >= 1.0f && !aggro && player != null) currentAction = selectedEnemyPeacefulAction;
        // If enemy is alive and aggro is active, aggro call.
        if (health >= 1.0f && aggro) AggroCall();
        // If enemy is alive, aggro is off, and player exists, check distance.
        if (health >= 1.0f && !aggro && playerTransform != null) DistanceCheck();
        // If enemy is alive and aggro is on, chase.
        if (health >= 1.0f && aggro && player != null) currentAction = selectedEnemyAggroAction;
        // Select action based on Enum. Make sure this is last in Update, and stays in Update.
        switch (currentAction) {
            case EnemyAction.Idle:
                break;
            case EnemyAction.Wander:
                Wander(); break;
            case EnemyAction.Chase:
                Chase(); break;
            case EnemyAction.Strafe:
                // Use same Wander Script, but a check inside it will direct it to another script to get a position.
                Wander(); break;
            case EnemyAction.Launched:
                break;
        }
    }
    // Override this function in child classes. Doing so will change the conditions for behaviors.
    protected void EnemyBehavior() {
        // TODO: put everything in Update above the switch here, then put this in Update.
        // Attack Intervals. First checks for cooldown, interval, that the enemy should be one that attacks, and that the attack is not null.
        if (!attackCooldown && attackInterval > 0.0f && enemyAttack != null && aggro && selectedEnemyProjectile != SelectEnemyProjectile.None) StartCoroutine(EnemyAttack());
    }
    // Create Projectiles on an interval.
    protected IEnumerator EnemyAttack() {
        attackCooldown = true;
        yield return new WaitForSeconds(attackInterval);
        // EDIT HERE TO CHANGE ATTACK TYPE.
        if (player != null)
        {
            if (currentAction != EnemyAction.Launched && player.activeSelf)
            {
                // Select attack type based on enum.
                switch (selectedEnemyProjectile)
                {
                    case SelectEnemyProjectile.None:
                        break;
                    case SelectEnemyProjectile.Basic:
                        BasicAttack(); break;
                    case SelectEnemyProjectile.Triple:
                        TripleShot(); break;
                    case SelectEnemyProjectile.X:
                        XShot(); break;
                    case SelectEnemyProjectile.Cross:
                        CrossShot(); break;
                    case SelectEnemyProjectile.Bomb:
                        Bomb(); break;
                }
            }
        }
    }
    // Put different types of projectiles after here.
    // Basic Attack.
    protected void BasicAttack() {
        Instantiate(enemyAttack, transform.position, transform.rotation);
        // Set attackCooldown to false. This ensures the attack will repeat.
        attackCooldown = false;
    }
    // Triple Shot.
    protected void TripleShot() {
        for (int i = 0; i < 3; i++) { 
            GameObject projectile = Instantiate(enemyAttack, transform.position, transform.rotation);
            // For projectiles after the first, add an offset.
            if (i != 0) { 
                EnemyBasicProjectile currentProjectile = projectile.GetComponent<EnemyBasicProjectile>();
                if(i == 1) currentProjectile.angleOffset = tripleShotOffset;
                if (i == 2) currentProjectile.angleOffset = -tripleShotOffset;
            }
        }
        attackCooldown = false;
    }
    // X Shot.
    protected void XShot()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject projectile = Instantiate(enemyIndependentAttack, transform.position, transform.rotation);
            // Apply Offset.
            EnemyIndependentProjectile currentProjectile = projectile.GetComponent<EnemyIndependentProjectile>();
            if (i == 0) currentProjectile.angle = 45;
            if (i == 1) currentProjectile.angle = 135;
            if (i == 2) currentProjectile.angle = 225;
            if (i == 3) currentProjectile.angle = 315;
        }
        attackCooldown = false;
    }
    // Cross Shot.
    protected void CrossShot()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject projectile = Instantiate(enemyIndependentAttack, transform.position, transform.rotation);
            // For projectiles after the first, add an offset.
            EnemyIndependentProjectile currentProjectile = projectile.GetComponent<EnemyIndependentProjectile>();
            if (i == 1) currentProjectile.angle = 90;
            if (i == 2) currentProjectile.angle = 180;
            if (i == 3) currentProjectile.angle = 270;
        }
        attackCooldown = false;
    }
    // Bomb.
    protected void Bomb() {
        GameObject projectile = Instantiate(enemyBomb, transform.position, transform.rotation);
        attackCooldown = false;
    }
    // Put different types of projectiles before here.
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
        // Sounds for bouncing off of Walls and Enemies.
        if (collision.gameObject.CompareTag("Wall") && currentAction == EnemyAction.Launched) {
            Instantiate(soundWallBounce, transform.position, Quaternion.Euler(0, 0, 0));
        }
        if (collision.gameObject.CompareTag("Enemy") && currentAction == EnemyAction.Launched)
        {
            Instantiate(soundEnemyRiccochet, transform.position, Quaternion.Euler(0, 0, 0));
            Instantiate(deathVFX, transform.position, Quaternion.Euler(0, 0, 0));
            // Bonus score for hitting other enemies.
            gameManagerScript.AddScore(deathScore);
        }

    }
    // Reset physics on collision Exit if not launched.
    public void OnCollisionExit2D(Collision2D collision)
    {
        if(currentAction != EnemyAction.Launched) enemyRb.velocity = Vector2.zero;
    }
    // Attack Collision
    public void OnTriggerEnter2D(Collider2D other)
    {
        //UnityEngine.Debug.Log("Trigger Detected");
        if (other.gameObject.CompareTag("PlayerAttack") || (other.gameObject.CompareTag("Explosion")))
        {
            if (vulnerable && health >= 1.0f) { 
                // Remember direction projectile hit from.
                projectileDirection = -(other.transform.position - transform.position).normalized;
                
                // Damage enemy.
                DamageEnemy();
            }
            // Destroy the player's bullet that hit.
            Destroy(other.gameObject);
        }
        // Case for destroying weak walls. Weak Walls have a hitbox as a parent object so that the child object of the wall itself is destroyed with it.
        if (other.gameObject.CompareTag("WeakWall") && currentAction == EnemyAction.Launched)
        {
            Destroy(other.gameObject);
            Instantiate(soundWallBreak, transform.position, Quaternion.Euler(0, 0, 0));
            // Small Bonus Score for breaking walls.
            gameManagerScript.AddScore(wallScore);
        }

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
        // Update Score.
        gameManagerScript.AddScore(deathScore);
        // Sound.
        Instantiate(soundEnemyLaunch, transform.position, Quaternion.Euler(0, 0, 0));
        // Trigger countdown for enemy death.
        StartCoroutine(EnemyDeath());
    }
    // Destroy the enemy, and trigger anything you want to happen upon death.
    public IEnumerator EnemyDeath()
    {
        yield return new WaitForSeconds(launchTime);
        // TODO: ADD SOUND AND VISUAL EFFECTS
        Instantiate(soundEnemyDeath, transform.position, Quaternion.Euler(0, 0, 0));
        Instantiate(deathVFX, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }
    // Enemy wanders before triggering aggro against player.
    protected void Wander() {
        wanderTimer += Time.deltaTime;

        // Move towards the target position.
        transform.position = Vector2.MoveTowards(transform.position, wanderPos, speed * Time.deltaTime);

        // If the time interval has passed, set a new target position.
        if ((wanderTimer > wanderInterval && currentAction == EnemyAction.Wander) || (wanderTimer > strafeInterval && currentAction == EnemyAction.Strafe))
        {
            // Depending on state, either set a goal for wandering or strafing.
            if (currentAction == EnemyAction.Wander) SetWanderGoal();
            if (currentAction == EnemyAction.Strafe) StrafeGoal();
            wanderTimer = 0f; // Reset the timer
        }
        // Strafe Version.
        if (wanderTimer > strafeInterval && currentAction == EnemyAction.Strafe)
        {
            // Set a goal for strafing.
            if (currentAction == EnemyAction.Strafe) StrafeGoal();
            wanderTimer = 0f; // Reset the timer
        }
    }
    // Strafe around the player.
    protected void StrafeGoal()
    {
        // Generate a random point within the wander range, existing around the player for strafing.
        Vector2 randomOffset = new Vector2(Random.Range(playerTransform.position.x-wanderRange, playerTransform.position.x+wanderRange), Random.Range(playerTransform.position.y- wanderRange, playerTransform.position.y+wanderRange));
        wanderPos = (Vector2)startPos + randomOffset;

        // Ensure the target position stays within the range.
        wanderPos = Vector2.ClampMagnitude(wanderPos - (Vector2)playerTransform.position, wanderRange) + (Vector2)playerTransform.position;
    }
    // Set target for Wandering.
    protected void SetWanderGoal() {
        // Generate a random point within the wander range.
        Vector2 randomOffset = new Vector2(Random.Range(-wanderRange, wanderRange), Random.Range(-wanderRange, wanderRange));
        wanderPos = (Vector2)startPos + randomOffset;

        // Ensure the target position stays within the range.
        wanderPos = Vector2.ClampMagnitude(wanderPos - (Vector2)startPos, wanderRange) + (Vector2)startPos;
    }
    // Set enemy Aggro.
    public void SetAggro(bool aggroChange) { 
    aggro = aggroChange;
    }
    // Call surrounding enemies to get their aggro.
    protected void AggroCall() {
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
    protected void DistanceCheck() {
        // Check the distance between the enemy and the player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        // If the player is within aggro range, set aggro to true.
        if (distanceToPlayer < aggroRange)
        {
            SetAggro(true);
        }
    }
    // Chase the player.
    protected void Chase() {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
    }
    // Chance of dropping item on death.
    protected void ItemDrop() {
        // If there is no item to be dropped, do nothing at all.
        if (dropItem != null)
        {
            float RNG = Random.Range(0, 64);
            if (RNG <= dropRate)
            {
                Instantiate(dropItem, transform.position, Quaternion.Euler(0, 0, 0));
            }
        }
    }
    // FINAL LINE; PUT METHODS ABOVE HERE.
}
