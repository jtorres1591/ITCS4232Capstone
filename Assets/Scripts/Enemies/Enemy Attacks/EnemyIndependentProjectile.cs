using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIndependentProjectile : EnemyBasicProjectile
{
    public float angle = 0.0f;
    // Start is called before the first frame update
    protected override void GetDirection()
    {
        
        //float originalAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //originalAngle -= 90;
        if (angle != 0)
        {

            transform.rotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0, 0, angle));

        }
        else
        {
            transform.rotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0, 0, angle));
        }
        // Set Velocity.
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }
}
