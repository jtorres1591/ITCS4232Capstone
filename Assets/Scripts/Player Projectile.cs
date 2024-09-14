using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    // Stats
    [SerializeField] private float speed = 8.0f;
    private Vector3 destination;
    private Vector3 direction;
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
        Destroy(gameObject);
    }
}
