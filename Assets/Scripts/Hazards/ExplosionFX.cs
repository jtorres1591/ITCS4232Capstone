using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class ExplosionFX : MonoBehaviour
{
    // ANIMATION.
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] explosionSprites;
    // Sprite length.
    //[SerializeField] private float numAnimationFrames;
    // Counts sprite animation is on.
    private float spriteStages;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimationFramesPlaying());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator AnimationFramesPlaying()
    {
        spriteRenderer.sprite = explosionSprites[Mathf.FloorToInt(spriteStages)];
        yield return new WaitForSeconds(0.05f);
        if (spriteStages < explosionSprites.Length-1)
        {
            spriteStages += 1;
            //Debug.Log(spriteStages);
        }
        else
        {
            Destroy(gameObject);
        }
        //Debug.Log(walkCycle);
        StartCoroutine(AnimationFramesPlaying());
    }
}
