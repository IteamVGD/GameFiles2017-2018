using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpScript : MonoBehaviour {
    public bool test;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Object" || collision.gameObject.tag == "Crate") //If collided with an "object" (ex. crates)
        {
            test = true;
            transform.parent.parent.GetChild(0).GetComponent<EnemyController>().Jump(); //Makes the enemy jump
        }
    }
}
