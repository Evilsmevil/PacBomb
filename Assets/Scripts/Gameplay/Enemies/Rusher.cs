using UnityEngine;
using System.Collections;

[AddComponentMenu("Enemy/Rusher")]
[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// This enemy looks at the player for a set amount of seconds and then fires across the screen
/// towards them
/// </summary>
public class Rusher : BaseEnemy
{
    public float speed = 20.0f;
    public float speedDecay = 0.9f;
    public float lookTime = 3.0f;
    public float chargeTime = 0.8f;
    void Start()
    {
        base.Start();
        //set ourselves on the same plane as the player
        if (player)
        {
            this.transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        }
        StartCoroutine(RunMovement());
    }

    protected override void FixedUpdate()
    {

    }

    public IEnumerator RunMovement()
    {
        //run until the enemy dies
        while (true)
        {
            //depending on the mode we are in run each mode
            StartCoroutine("LookAtPlayer");
            
            yield return new WaitForSeconds(lookTime);

            StopCoroutine("LookAtPlayer"); //we only want one behaviour to run at a time

            //yield return new WaitForSeconds(3.0f);

            StartCoroutine("MoveTowardsPlayer");

            yield return new WaitForSeconds(chargeTime);

        }
    }

    /// <summary>
    /// Enemy will look at the player (also try and slow down)
    /// </summary>
    IEnumerator LookAtPlayer()
    {
        while (true)
        {
            rigidbody.velocity *= speedDecay;
            if (player)
            {
                transform.LookAt(player.transform);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    void MoveTowardsPlayer()
    {
        //move towards player really fast
        //work out the the direction the player is

        if (player)
        {
            Vector3 direction = player.transform.position - transform.position;
            //direction.y = 0; //i know this is slow but it's easy to read
            direction.Normalize();

            Vector3 newVelocity = new Vector3(direction.x * speed, 0, direction.z * speed);
            rigidbody.velocity = newVelocity;
        }
    }

}
