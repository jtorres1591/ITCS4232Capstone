using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakWallSpawner : MonoBehaviour
{
    
    [SerializeField] private float respawnTimer = 30.0f;
    [SerializeField] private GameObject weakWallPrefab;
    [SerializeField] private float spawnRange = 200.0f;
    // References and Flags.
    private GameObject weakWall;
    private bool wallExists = false;
    private bool respawnTimerActive = false;
    private bool respawnTimerComplete = false;
    // Player References.
    protected GameObject player;
    protected Transform playerTransform;
    // VFX
    [SerializeField] private GameObject breakVFX;
    // Start is called before the first frame update
    void Start()
    {
        // Get player transform.
        player = GameObject.Find("Player");
        if (player != null) playerTransform = player.transform;
        // Spawn First Wall.
        SpawnWall();
    }

    // Update is called once per frame
    void Update()
    {
        if (weakWall == null) wallExists = false;
        if (!wallExists && !respawnTimerActive) StartCoroutine(RespawnTimer());
        if (respawnTimerComplete && player != null)
        {
            if(OutOfRange()) SpawnWall();
        }
    }
    // First Spawn.
    private void SpawnWall() { 
        weakWall = Instantiate(weakWallPrefab, transform.position, transform.rotation);
        wallExists = true;
        // Reset RespawnTimerComplete.
        respawnTimerComplete = false;
    }
    // RespawnTimer
    private IEnumerator RespawnTimer() {
        // VFX.
        Instantiate(breakVFX, transform.position, Quaternion.Euler(0, 0, 0));
        respawnTimerActive = true;
        yield return new WaitForSeconds(respawnTimer);
        respawnTimerComplete = true;
        respawnTimerActive=false;
    }
    // Range Check. Want to be out of range (A little past being Off-Screen) before the wall respawns.
    private bool OutOfRange() {
        // Get distance from player to spawner.
        float distanceToPlayer = (transform.position - playerTransform.position).sqrMagnitude;
        return (distanceToPlayer >= spawnRange);
    }
    // Every Spawn After.

}
