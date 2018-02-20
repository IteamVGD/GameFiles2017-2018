using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPunchCollider : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 13) //Layer 9 = entity
        {
            collision.transform.GetComponent<PlayerController>().TakeDamage(GetComponentInParent<EnemyController>().damage); //Decreases player's health by the damage this enemy deals

            //Insert knockback code here
            if (collision.transform.position.x - gameObject.transform.position.x <= 0) //If player is to the right
            {
                collision.transform.GetComponent<Rigidbody2D>().AddForce(-transform.right * gameObject.transform.parent.GetComponent<EnemyController>().knockbackStregth);
            }
            else
            {
                collision.transform.GetComponent<Rigidbody2D>().AddForce(transform.right * gameObject.transform.parent.GetComponent<EnemyController>().knockbackStregth);
            }
        }
    }

    private void Update()
    {
        if (transform.parent.GetComponent<EnemyController>().isPunching && transform.parent.GetComponent<EnemyController>().playerObj.transform.position.x - gameObject.transform.position.x <= 0) //If player is to the right and is punching
        {
            gameObject.transform.rotation = new Quaternion(0, 0, 0, 0); //Rotate punch collider right
        }
        if (transform.parent.GetComponent<EnemyController>().isPunching && transform.parent.GetComponent<EnemyController>().playerObj.transform.position.x - gameObject.transform.position.x > 0) //If player is to the left and is punching
        {
            gameObject.transform.rotation = new Quaternion(0, 180, 0, 0); //Rotate punch collider left
        }
    }
}
