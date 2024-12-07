using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakWall : MonoBehaviour
{
    private bool despawnEnemy = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(SetDespawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {

    }
    // Despawn enemy on collision in the tenth of a second that the wall spawns.
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Enemy") && despawnEnemy) Destroy(collision.gameObject);
    }
    // Once a tenth of a second passes, enemies no longer despawn on collision.
    private IEnumerator SetDespawnEnemy(){
        yield return new WaitForSeconds(0.1f);
        despawnEnemy = false;
    }
    
}
