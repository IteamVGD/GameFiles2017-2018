using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Object") //If collided with an "object" (ex. crates)
        {
            transform.parent.GetComponent<EnemyController>().Jump(); //Makes the enemy jump
        }
    }
}
