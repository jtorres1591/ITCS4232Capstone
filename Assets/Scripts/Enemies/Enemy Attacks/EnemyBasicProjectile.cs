using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicProjectile : MonoBehaviour
{
    // Stats & References.
    public float speed = 5f;
    private Transform playerTransform;
    private Vector2 direction;
    // Shouldn't make sound on destruction if it is due to being off screen.
    private bool offScreen = false;
    // Start is called before the first frame update
    void Start()
    {
        // Find the player object by tag.
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // Destroy if null.
        if(playerTransform == null) Destroy(gameObject);
        // Calculate the direction to the player.
        GetDirection();
    }

    // Update is called once per frame.
    void Update()
    {
        if (playerTransform != null)
        {
            

            // Move the projectile towards the player.
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }
    // Get Direction the projectile will move in.
    private void GetDirection() {
        direction = (playerTransform.position - transform.position).normalized;
    }
    // Destroy if off screen.
    void OnBecameInvisible()
    {
        offScreen = true;
        Destroy(gameObject);
    }
    // TODO: ON DESTROY, CREATE VISUAL EFFECT AND SOUND IF OFFSCREEN IS FALSE.
    private void OnDestroy()
    {
        if (!offScreen)
        {
            // VISUAL EFFECT AND SOUND.
        }
    }
    // Destroy on contact with walls.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) Destroy(gameObject);
    }
}
