using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    // Stats
    [SerializeField] private float speed = 8.0f;
    private Vector3 destination;
    private Vector3 direction;
    // Shouldn't make sound on destruction if it is due to being off screen.
    private bool offScreen = false;
    // Start is called before the first frame update
    void Start()
    {
        // Get mouse position in world coordinates
        destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        destination.z = 0f;  // Ensure the z-component is 0 (for 2D)

        // Calculate direction to the mouse position
        direction = (destination - transform.position).normalized;

        // Apply velocity in the direction towards the mouse
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Destroy if off screen.
    void OnBecameInvisible() {
        offScreen = true;
        Destroy(gameObject);
    }
    // TODO: ON DESTROY, CREATE VISUAL EFFECT AND SOUND IF OFFSCREEN IS FALSE.
    private void OnDestroy()
    {
        if (!offScreen) { 
        // VISUAL EFFECT AND SOUND.
        } 
    }
    // Destroy on contact with walls.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) Destroy(gameObject);
    }
}
