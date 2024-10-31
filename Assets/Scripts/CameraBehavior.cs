using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    
    // Rate at which the camera follows the player.
    [SerializeField] private float smoothSpeed = 0.125f;
    
    // Offset Vector.
    private Vector3 offset;
    // Player Reference.
    private Transform playerTransform;
    // For Testing Purposes.
    public bool followPlayer = true;
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null) playerTransform = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Using LateUpdate
    void LateUpdate() {
        CameraFollow();
        
    }
    // Camera Following.
    private void CameraFollow() {
        // This should not run if the player is null.
        if (playerTransform == null || !followPlayer) return;
        // Calculate the desired position with offsets
        //Vector3 goalPosition = playerTransform.position + new Vector3(offsetH, offsetV, -10) + offset;
        Vector3 goalPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        // Smoothly interpolate to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, goalPosition, smoothSpeed);

        // Update camera position
        transform.position = smoothedPosition;
    }
}
