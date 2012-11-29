using UnityEngine;
using System.Collections;

[AddComponentMenu("Enemy/Homer")]
[RequireComponent (typeof (Rigidbody))]
/// <summary>
/// This is a stupid enemy, it just moves towards the player every frame
/// </summary>
public class Homer : BaseEnemy 
{

    public float acceleration;

    protected override void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            rigidbody.AddForce(direction.normalized * acceleration * Time.fixedDeltaTime);

            transform.LookAt(player.transform);
        }
    }

}
