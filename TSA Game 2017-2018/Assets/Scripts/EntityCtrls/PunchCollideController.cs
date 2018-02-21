using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollideController : MonoBehaviour {

    public GameObject playerObj; //The parent with PlayerController script
    public BoxCollider2D punchCollider; //The box collider that is used to punch
    public int timesRun;
    public bool runCode;

    private void Start()
    {
        runCode = true;
    }

    // Update is called once per frame
    void Update () {
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "CrouchPunchRight" && playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "CrouchPunchLeft" && playerObj.transform.GetComponent<PlayerController>().isCrouching == true && playerObj.transform.GetComponent<PlayerController>().isDownPunching == false) //If is not crouch punching
        {
            punchCollider.enabled = false; //Disable collider
        }

        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "DownPunchLeft_1" && playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "DownPunchRight_1" && playerObj.transform.GetComponent<PlayerController>().isCrouching == false && playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "PunchLeft_4" && playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "PunchRight_4" && playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "PunchLeft_5" && playerObj.transform.GetComponent<SpriteRenderer>().sprite.name != "PunchRight_5" && playerObj.transform.GetComponent<PlayerController>().isDownPunching == false)
        {
            punchCollider.enabled = false;
        }

        //Crouch Punch Controller
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "CrouchPunchLeft") //If is crouch punching left
        {
            punchCollider.offset = new Vector2(-0.9f, -0.4f); //Set collider offset to crouch + face left
            punchCollider.size = new Vector2(1.26f, 0.71f);
            punchCollider.enabled = true; //Enable collider
        }
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "CrouchPunchRight") //If is crouch punching right
        {
            punchCollider.offset = new Vector2(0.8f, -0.4f); //Set collider offset to crouch + face right
            punchCollider.size = new Vector2(1.26f, 0.71f);
            punchCollider.enabled = true; //Enable collider
        }

        //Down Punch Controller
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "DownPunchRight_1")
        {
            punchCollider.enabled = true;
            punchCollider.offset = new Vector2(-0.045f, -0.77f);
            punchCollider.size = new Vector2(1.41f, 1.062f);
        }
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "DownPunchLeft_1")
        {
            punchCollider.enabled = true;
            punchCollider.offset = new Vector2(-0.045f, -0.77f);
            punchCollider.size = new Vector2(1.41f, 1.062f);
        }

        //Normal/Charge Punch Controller
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "PunchLeft_4" || playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "PunchLeft_5") //Frame of punch animation where collider should turn on (arm is extended, punch is in full effect), punching left
        {
            punchCollider.offset = new Vector2(-1.34f, 0.59f); //Set collider offset to face left
            punchCollider.size = new Vector2(1.26f, 0.71f);
            punchCollider.enabled = true; //Enable collider
        }
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "PunchRight_4" || playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "PunchRight_5") //Frame of punch animation where collider should turn on (arm is extended, punch is in full effect), punching right
        {
            punchCollider.offset = new Vector2(1.34f, 0.59f); //Set collider offset to face right
            punchCollider.size = new Vector2(1.26f, 0.71f);
            punchCollider.enabled = true; //Enable collider
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9) //Layer 9 = Entity, includes any "passive" entities or things that arent enemies that could be punched
        {
            if (collision.gameObject.tag == "Enemy") //If is an enemy
            {
                collision.gameObject.transform.GetComponent<EnemyController>().TakeDamage(playerObj.transform.GetComponent<PlayerController>().punchDamage); //Decrease enemy's health in EnemyController by the punchDamage of the player parent's PlayerController
                collision.gameObject.transform.GetComponent<EnemyController>().isBeingPunched = true;

                //Insert knockback here
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * playerObj.transform.GetComponent<PlayerController>().verticalKnockbackStrength); //Knockback up
                if (playerObj.transform.GetComponent<PlayerController>().previousSideFacing == 4) //If player is facing right
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * playerObj.transform.GetComponent<PlayerController>().horizontalKnockbackStrength); //Knockback right
                else //If player is facing left
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-transform.right * playerObj.transform.GetComponent<PlayerController>().horizontalKnockbackStrength); //Knockback left
                collision.gameObject.transform.GetComponent<EnemyController>().GotPunched(); //Tells the enemy it just got punched

                ContactPoint2D contact = collision.contacts[0];
                StartCoroutine(playerObj.transform.GetComponent<PlayerController>().gameControllerScript.SpawnPow(contact.point));
            }
        }
    }
}
