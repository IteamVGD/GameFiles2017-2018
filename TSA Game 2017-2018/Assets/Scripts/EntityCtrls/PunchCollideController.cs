using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollideController : MonoBehaviour {

    public GameObject playerObj; //The parent with PlayerController script
    public BoxCollider2D punchCollider;
    public bool colliderEnabled;
	
	// Update is called once per frame
	void Update () {
		if(playerObj.transform.GetComponent<PlayerController>().isPunching == true && playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "PunchLeft_4") //Frame of punch animation where collider should turn on (arm is extended, punch is in full effect)
        {
            punchCollider.enabled = true;
            colliderEnabled = true;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9) //Layer 9 = Entity, includes any "passive" entities or things that arent enemies that could be punched
        {
            if(collision.tag == "Enemy") //If is an enemy
            {
                // collision.transform.GetComponent<EnemyController>().health -= playerObj.transform.GetComponent<PlayerController>().standardPunchDamage;
            }
        }         
    }
}
