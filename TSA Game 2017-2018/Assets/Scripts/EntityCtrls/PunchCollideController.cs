using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollideController : MonoBehaviour {

    public GameObject playerObj; //The parent with PlayerController script
    public BoxCollider2D punchCollider; //The box collider that is used to punch
    public int timesRun;
    public bool runCode;

    public int sidePunching; //0 = downPunch, 1 = right, 2 = left

    public bool canPlaySounds = true;
    public bool canSwitchSwitches = true;

    private void Start()
    {
        runCode = true;
    }

    //Update is called once per frame
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
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "CrouchPunchLeft" && punchCollider.enabled == false) //If is crouch punching left
        {
            punchCollider.offset = new Vector2(-0.9f, -0.4f); //Set collider offset to crouch + face left
            punchCollider.size = new Vector2(1.26f, 0.71f);
            punchCollider.enabled = true; //Enable collider
            sidePunching = 2;
            PlayPunchSound();
        }
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "CrouchPunchRight" && punchCollider.enabled == false) //If is crouch punching right
        {
            punchCollider.offset = new Vector2(0.8f, -0.4f); //Set collider offset to crouch + face right
            punchCollider.size = new Vector2(1.26f, 0.71f);
            punchCollider.enabled = true; //Enable collider
            sidePunching = 1;
            PlayPunchSound();
        }

        //Down Punch Controller
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "DownPunchRight_1" || playerObj.transform.GetComponent<PlayerController>().isDownPunching && punchCollider.enabled == false)
        {
            punchCollider.enabled = true;
            punchCollider.offset = new Vector2(-0.045f, -0.9f);
            punchCollider.size = new Vector2(1.41f, 1.4f);
            sidePunching = 0;
        }
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "DownPunchLeft_1" || playerObj.transform.GetComponent<PlayerController>().isDownPunching && punchCollider.enabled == false)
        {
            punchCollider.enabled = true;
            punchCollider.offset = new Vector2(-0.045f, -0.9f);
            punchCollider.size = new Vector2(1.41f, 1.4f);
            sidePunching = 0;
        }

        //Normal/Charge Punch Controller
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "PunchLeft_4" || playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "PunchLeft_5" && punchCollider.enabled == false) //Frame of punch animation where collider should turn on (arm is extended, punch is in full effect), punching left
        {
            punchCollider.offset = new Vector2(-1.25f, 0.59f); //Set collider offset to face left
            punchCollider.size = new Vector2(1.4f, 0.71f);
            punchCollider.enabled = true; //Enable collider
            sidePunching = 2;
            PlayPunchSound();
        }
        if (playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "PunchRight_4" || playerObj.transform.GetComponent<SpriteRenderer>().sprite.name == "PunchRight_5" && punchCollider.enabled == false) //Frame of punch animation where collider should turn on (arm is extended, punch is in full effect), punching right
        {
            punchCollider.offset = new Vector2(1.25f, 0.59f); //Set collider offset to face right
            punchCollider.size = new Vector2(1.4f, 0.71f);
            punchCollider.enabled = true; //Enable collider
            sidePunching = 1;
            PlayPunchSound();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9) //Layer 9 = Entity, includes any "passive" entities or things that arent enemies that could be punched
        {
            if (collision.gameObject.tag == "Enemy") //If is an enemy
            {
                if(playerObj.transform.GetComponent<PlayerController>().isDownPunching)
                    collision.gameObject.transform.GetComponent<EnemyController>().TakeDamage(playerObj.transform.GetComponent<PlayerController>().punchDamage, true); //Decrease enemy's health in EnemyController by the punchDamage of the player parent's PlayerController
                else
                    collision.gameObject.transform.GetComponent<EnemyController>().TakeDamage(playerObj.transform.GetComponent<PlayerController>().punchDamage, false); //Decrease enemy's health in EnemyController by the punchDamage of the player parent's PlayerController
                //Insert knockback here
                if (playerObj.transform.GetComponent<PlayerController>().previousSideFacing == 4) //If player is facing right
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * playerObj.transform.GetComponent<PlayerController>().horizontalKnockbackStrength); //Knockback right
                else //If player is facing left
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-transform.right * playerObj.transform.GetComponent<PlayerController>().horizontalKnockbackStrength); //Knockback left
                collision.gameObject.transform.GetComponent<EnemyController>().GotPunched(); //Tells the enemy it just got punched
                if (GameObject.FindGameObjectsWithTag("Pow").Length == 0)
                {
                    ContactPoint2D contact = collision.contacts[0];
                    StartCoroutine(playerObj.transform.GetComponent<PlayerController>().gameControllerScript.SpawnPow(contact.point));
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SwitchController>() != null)
        {
            if (collision.GetComponent<SwitchController>().triggerSongObj)
                collision.GetComponent<SwitchController>().TriggerSong();
            ActivateSwitches();
            if (GameObject.FindGameObjectsWithTag("Pow").Length == 0)
            {
                StartCoroutine(playerObj.transform.GetComponent<PlayerController>().gameControllerScript.SpawnPow(collision.gameObject.transform.position));
            }
        }
    }

    public void ActivateSwitches()
    {
        playerObj.transform.GetComponent<PlayerController>().audioSource.PlayOneShot(playerObj.transform.GetComponent<PlayerController>().crateSwitch);
        playerObj.transform.GetComponent<PlayerController>().gameControllerScript.ManageBoxes();
        StartCoroutine(ResetPlaySoundsBool());
    }

    public void PlayPunchSound()
    {
        if (canPlaySounds)
        {
            canPlaySounds = false;
            playerObj.transform.GetComponent<PlayerController>().audioSource.PlayOneShot(playerObj.transform.GetComponent<PlayerController>().normalPunch);
            StartCoroutine(ResetPlaySoundsBool());
        }
    }

    /*public void PlayDownPunchSound()
    {
        if (canPlaySounds)
        {
            canPlaySounds = false;
            playerObj.transform.GetComponent<PlayerController>().audioSource.PlayOneShot(playerObj.transform.GetComponent<PlayerController>().downPunch);
            StartCoroutine(ResetPlaySoundsBool());
        }
    }*/

    IEnumerator ResetPlaySoundsBool()
    {
        yield return new WaitForSeconds(0.7f);
        canPlaySounds = true;
        canSwitchSwitches = true;
    }
}
