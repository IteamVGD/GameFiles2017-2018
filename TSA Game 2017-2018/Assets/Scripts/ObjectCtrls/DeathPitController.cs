using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPitController : MonoBehaviour {

    public GameObject teleportPoint; //Where the player should be placed to heal from his KO when he falls into this pit

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 13) //If player fell into death pit
        {
            collision.gameObject.transform.position = teleportPoint.transform.position;
            collision.gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            collision.gameObject.transform.GetComponent<PlayerController>().TakeDamage(collision.gameObject.transform.GetComponent<PlayerController>().maxHealth);
        }
        else
        {
            Destroy(collision.gameObject); //Kill it
        }
    }
}
