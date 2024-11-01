using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class EnemyBasicProjectile : MonoBehaviour
{
    // Stats & References.
    public float speed = 5f;
    protected Transform playerTransform;
    protected UnityEngine.Vector2 direction;
    public float angleOffset = 0.0f;
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
            //transform.position += (Vector3)direction * speed * Time.deltaTime;
            // New Version.
            transform.position += (UnityEngine.Vector3)direction * speed * Time.deltaTime;
        }
    }
    // Get Direction the projectile will move in.
    protected virtual void GetDirection() {
        // Old Version.
        direction = (playerTransform.position - transform.position).normalized;
        // Applying direction change.
        if (angleOffset != 0) direction = AngleOffset(direction, angleOffset);
        UnityEngine.Debug.Log(direction);
        
    }
    // Change the direction based on an offset.
    protected virtual UnityEngine.Vector2 AngleOffset(UnityEngine.Vector2 originalVector, float angleChange) {
        // Convert the angle to radians.
        float radianAngle = angleChange * Mathf.Deg2Rad;
        // 
        float newX = originalVector.x * Mathf.Cos(angleChange) - originalVector.y * Mathf.Sin(angleChange);
        float newY = originalVector.x * Mathf.Sin(angleChange) + originalVector.y * Mathf.Cos(angleChange);
        return new UnityEngine.Vector2 (newX, newY);
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
