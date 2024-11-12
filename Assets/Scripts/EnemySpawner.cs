using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRange = 200.0f;
    [SerializeField] private float despawnRange = 250.0f;
    [SerializeField] private GameObject[] enemies;
    // Getting a reference to the Sprite Renderer to turn it off on startup. It should only be visible while editing.
    private SpriteRenderer spriteRenderer;
    // CurrentEnemy is used as a reference, should not be set outside.
    private GameObject currentEnemy;
    private bool enemyExists = false;
    private bool outOfSight = false;
    // Player References.
    protected GameObject player;
    protected Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        // Get Sprite Renderer and turn it off.
        spriteRenderer = GetComponent<SpriteRenderer>();
        //if (spriteRenderer != null ) spriteRenderer.enabled = false;
        // Get player transform.
        player = GameObject.Find("Player");
        if (player != null) playerTransform = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Get distance from player to spawner.
        float distanceToPlayer = (transform.position - playerTransform.position).sqrMagnitude;
        //UnityEngine.Debug.Log(outOfSight);
        // Check if CurrentEnemy has been destroyed.
        if (currentEnemy == null && enemyExists) { 
            enemyExists = false;
        }
        // If enemy does not exist, player is in range, and spawner is off screen, spawn an enemy.
        if (!enemyExists && distanceToPlayer <= spawnRange && outOfSight) { 
            // IDEA: PUT SPAWNENEMY IN A SHORT COROUTINE THAT CREATES A SMOKE EFFECT AROUND ENEMIES AS THEY SPAWN.
            SpawnEnemy();
        }
        // Destroy Current Enemy if out of range.
        if (enemyExists && distanceToPlayer >= despawnRange) {
            if ((currentEnemy.transform.position - playerTransform.position).sqrMagnitude >= despawnRange) Destroy(currentEnemy);
        }
    }
    // Spawn Enemy.
    private void SpawnEnemy() {
        currentEnemy = Instantiate(ChooseEnemy(), transform.position, transform.rotation);
        enemyExists = true;
    }
    // Randomly select an enemy from the array.
    private GameObject ChooseEnemy() {
        int rng = Random.Range(0, enemies.Length);
        GameObject chosenEnemy = enemies[rng];


        return chosenEnemy;
    }
    // Adjust outOfSight using the entering and exiting of the camera.
    // ONCE DONE, SET THE SPRITE OF THE SPAWNER PREFAB TO NONE. THIS WILL PREVENT ONBECAMEVISIBLE FROM FAILING WHILE MAKING IT "INVISIBLE".
    private void OnBecameVisible()
    {
        outOfSight = false;
    }
    private void OnBecameInvisible()
    {
        outOfSight = true;
    }
}
