using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    // GameObjects
    [SerializeField] private GameObject playerProjectile;
    // Player Stats
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float health = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Walk
        PlayerMovement();
        // Shoot on Left Mouse Click.
        if (Input.GetMouseButtonDown(0)) ShootProjectile();
    }
    // Walking
    private void PlayerMovement() {
        // Get input from axis.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //Debug.Log(verticalInput);

        // Calculate movement direction.
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * walkSpeed * Time.deltaTime;
        // Apply movement to Player's position.
        transform.position += movement;
    }
    // Shoot projectiles when mouse is clicked.
    void ShootProjectile()
    {
        //Debug.Log("Click");
        // Instantiating not only needs object, but both position and rotation. Remember that.
        Instantiate(playerProjectile, transform.position, transform.rotation);
    }
    // Take Damage.
    public void DamagePlayer() {
        health--;
    }
}
