using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    // Health Amount.
    [SerializeField] private float healHealth = 1.0f;
    // Lifespan.
    [SerializeField] private float lifeSpan = 20.0f;
    // References.
    protected PlayerControls playerScript;
    protected CircleCollider2D itemCollider;
    [SerializeField] protected GameObject soundCollect;
    [SerializeField] protected GameObject collectVFX;
    // Start is called before the first frame update
    void Start()
    {
        // Get player script.
        GameObject playerReference = GameObject.Find("Player");
        if(playerReference != null) playerScript = playerReference.GetComponent<PlayerControls>();
        // Get collider.
        itemCollider = GetComponent<CircleCollider2D>();
        // Destroy after Lifespan is over.
        Destroy(gameObject, lifeSpan);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Contact with player uses and consumes item.
    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(soundCollect, transform.position, UnityEngine.Quaternion.Euler(0, 0, 0));
            Instantiate(collectVFX, transform.position, UnityEngine.Quaternion.Euler(0, 0, 0));
            playerScript.HealPlayer(healHealth);
            Destroy(gameObject);
        }
    }
}
