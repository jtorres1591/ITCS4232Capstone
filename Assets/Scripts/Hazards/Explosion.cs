using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float lifeSpan = 0.3f;
    [SerializeField] private GameObject soundExplosion;
    [SerializeField] private GameObject soundWallBreak;
    [SerializeField] private GameObject visualEffect;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeSpan);
        Instantiate(visualEffect, transform.position, Quaternion.Euler(0, 0, 0));
        Instantiate(soundExplosion, transform.position, Quaternion.Euler(0, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WeakWall"))
        {
            Instantiate(soundWallBreak, transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(collision.gameObject);
        }
    }
}
