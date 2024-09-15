using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SuperclassEnemyProperties : MonoBehaviour
{
    // This script is intended to be a superclass for enemies. Any properties that all enemies will share should be here.
    protected Rigidbody2D enemyRb;
    protected CircleCollider2D enemyCollider;
    protected PlayerControls playerScript;
    // Stats all enemies should have.
    [SerializeField] protected float health = 1.0f;
    [SerializeField] protected float speed = 2.0f;
    [SerializeField] protected float launchSpeed = 20.0f;
    [SerializeField] protected float launchTime = 4.0f;
    // Invincibility frame to prevent the same bullet from damaging twice.
    private bool vulnerable = true;
    [SerializeField] private float damageCooldown = 0.1f;
    // Physics reference. Finds direction projectile hits from.
    Vector3 projectileDirection;
    // Start is called before the first frame update
    void Start()
    {
        // Get the enemy's rigidbody, collider and the player's script to be able to deal damage.
        enemyRb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<CircleCollider2D>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Damage player on collision.
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //UnityEngine.Debug.Log("Collision Detected");
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.DamagePlayer();
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
        
    }
    // Script to damage an enemy and destroy it if it runs out of health.
    public virtual void DamageEnemy()
    {
        
        health--;
        StartCoroutine(DamageCooldown());
        //UnityEngine.Debug.Log("Damaged Enemy");
        if (health <= 0)
        {
            Launch(projectileDirection);
            //EnemyDeath();
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
    public void Launch(Vector3 direction) {
        // Unfreeze Constraints.
        enemyRb.constraints = RigidbodyConstraints2D.None;
        // Set tag to PlayerAttack.
        gameObject.tag = "PlayerAttack";
        // Set IsTrigger to true.
        enemyCollider.isTrigger = true;
        // Apply force.
        enemyRb.AddForce(direction * launchSpeed, ForceMode2D.Impulse);
        // Trigger countdown for enemy death.
        StartCoroutine(EnemyDeath());
    }
    // Destroy the enemy, and trigger anything you want to happen upon death.
    public IEnumerator EnemyDeath()
    {
        yield return new WaitForSeconds(launchTime);
        Destroy(gameObject);
    }
}
