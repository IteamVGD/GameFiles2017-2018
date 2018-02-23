using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpScript : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Object" || collision.gameObject.tag == "Crate") //If collided with an "object" (ex. crates)
        {
            transform.parent.GetComponent<EnemyController>().Jump(); //Makes the enemy jump
        }
    }
}
