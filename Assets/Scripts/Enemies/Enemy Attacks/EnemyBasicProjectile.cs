using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicProjectile : MonoBehaviour
{
    // Stats & References.
    public float speed = 5f;
    protected Transform playerTransform;
    protected Vector2 direction;
    // Shouldn't make sound on destruction if it is due to being off screen.
    protected bool offScreen = false;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Find the player object by tag.
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // Destroy if null.
        if(playerTransform == null) Destroy(gameObject);
        // Calculate the direction to the player.
        GetDirection();
    }

    // Update is called once per frame.
    protected virtual void Update()
    {
        if (playerTransform != null)
        {
            

            // Move the projectile towards the player.
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }
    // Get Direction the projectile will move in.
    protected virtual void GetDirection() {
        direction = (playerTransform.position - transform.position).normalized;
    }
    // Destroy if off screen.
    void OnBecameInvisible()
    {
        offScreen = true;
        Destroy(gameObject);
    }
    // TODO: ON DESTROY, CREATE VISUAL EFFECT AND SOUND IF OFFSCREEN IS FALSE.
    protected virtual void OnDestroy()
    {
        if (!offScreen)
        {
            // VISUAL EFFECT AND SOUND.
        }
    }
    // Destroy on contact with walls.
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) Destroy(gameObject);
    }
}
