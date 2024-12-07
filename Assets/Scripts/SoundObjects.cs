using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObjects : MonoBehaviour
{
    public AudioClip soundVariable;
    // Start is called before the first frame update
    void Start()
    {
        // Play Sound and destroy once finished.
        AudioSource.PlayClipAtPoint(soundVariable, transform.position);
        Destroy(gameObject, soundVariable.length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
