using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageThrough : MonoBehaviour {

    public GameObject playerObj;
    public int walkThroughDamage; //Damage the player takes if he walks through the enemy

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == playerObj && playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "DownPunchLeft_1" && playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "DownPunchRight_1")
        {
            playerObj.transform.GetComponent<PlayerController>().TakeDamage(walkThroughDamage);
        }
    }
}
