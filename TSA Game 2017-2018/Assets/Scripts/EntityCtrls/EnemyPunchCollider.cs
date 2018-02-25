using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPunchCollider : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.transform.GetComponent<PlayerController>().TakeDamage(GetComponentInParent<EnemyController>().damage); //Decreases player's health by the damage this enemy deals
        ContactPoint2D contact = collision.contacts[0];
        StartCoroutine(collision.gameObject.transform.GetComponent<PlayerController>().gameControllerScript.SpawnPow(contact.point));

        /*/Knockback
        if (collision.gameObject.transform.position.x - gameObject.transform.position.x <= 0) //If player is to the right
        {
            collision.gameObject.transform.GetComponent<Rigidbody2D>().AddForce(-transform.right * gameObject.transform.parent.GetComponent<EnemyController>().knockbackStregth);
        }
        else
        {
            collision.gameObject.transform.GetComponent<Rigidbody2D>().AddForce(transform.right * gameObject.transform.parent.GetComponent<EnemyController>().knockbackStregth);
        }*/
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
