using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    // Health Amount.
    [SerializeField] private float healHealth = 1.0f;
    // References.
    protected PlayerControls playerScript;
    protected CircleCollider2D itemCollider;
    // Start is called before the first frame update
    void Start()
    {
        // Get player script.
        playerScript = GameObject.Find("Player").GetComponent<PlayerControls>();
        // Get collider.
        itemCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Contact with player uses and consumes item.
    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            playerScript.HealPlayer(healHealth);
            Destroy(gameObject);
        }
    }
}
