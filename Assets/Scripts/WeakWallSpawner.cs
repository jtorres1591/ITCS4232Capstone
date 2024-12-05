using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakWallSpawner : MonoBehaviour
{

    [SerializeField] private float respawnTimer = 30.0f;
    [SerializeField] private GameObject weakWallPrefab;
    private GameObject weakWall;
    private bool wallExists = false;
    private bool respawnTimerActive = false;
    // Start is called before the first frame update
    void Start()
    {
        SpawnWall();
    }

    // Update is called once per frame
    void Update()
    {
        if (weakWall == null) wallExists = false;
        if (!wallExists && !respawnTimerActive) StartCoroutine(RespawnTimer());
    }
    // First Spawn.
    private void SpawnWall() { 
        weakWall = Instantiate(weakWallPrefab, transform.position, transform.rotation);
        wallExists = true;
    }
    // RespawnTimer
    private IEnumerator RespawnTimer() { 
        respawnTimerActive = true;
        yield return new WaitForSeconds(respawnTimer);
        SpawnWall();
        respawnTimerActive=false;
    }
    // Every Spawn After.

}
