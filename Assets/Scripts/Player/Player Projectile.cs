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
    // Sounds
    [SerializeField] private GameObject soundFire;
    [SerializeField] private GameObject soundHit1;
    [SerializeField] private GameObject soundHit2;
    [SerializeField] private GameObject soundMiss;
    // VFX
    [SerializeField] private GameObject projectileVFX;
    // Start is called before the first frame update
    void Start()
    {
        // Get mouse position in world coordinates.
        destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        destination.z = 0f;  // Ensure the z-component is 0 (for 2D)

        // Calculate direction to the mouse position.
        direction = (destination - transform.position).normalized;

        // Apply velocity in the direction towards the mouse.
        GetComponent<Rigidbody2D>().velocity = direction * speed;
        // Fire Sound.
        Instantiate(soundFire, transform.position, Quaternion.Euler(0, 0, 0));
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
        if (collision.gameObject.CompareTag("Wall"))
        {
            Instantiate(soundMiss, transform.position, Quaternion.Euler(0, 0, 0));
            Instantiate(projectileVFX, transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy")) {
            Instantiate(projectileVFX, transform.position, Quaternion.Euler(0, 0, 0));
            int soundChoice = Random.Range(0, 2);
            if (soundChoice == 0) Instantiate(soundHit1, transform.position, Quaternion.Euler(0, 0, 0));
            if (soundChoice == 1) Instantiate(soundHit2, transform.position, Quaternion.Euler(0, 0, 0));
        }
    }
}
