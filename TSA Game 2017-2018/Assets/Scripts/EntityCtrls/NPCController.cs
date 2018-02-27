using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour {

    public int talkRange; //How close the player needs to be to talk to this npc
    public GameObject playerObj;
    public GameObject uiCanvas;
    public GameObject dialogueObj;

    public string faceLeft = "Left";
    public string faceRight = "Right";
    public string faceForward = "Forward";
    public string faceBack = "Back";
    public bool isBeingTalkedTo;

    public List<string> dialogue;
    public int dialogueCount; //How many dialogues the player has been through
    public int dialoguePromptInt; //When the player should be prompted with something (after going through this number of messages)

    public Animator anim;
    
    // Update is called once per frame
    void Update () {
        if ((Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire3")) && Vector3.Distance(gameObject.transform.position, playerObj.transform.position) < talkRange)
        {
            if(!isBeingTalkedTo)
            {
                isBeingTalkedTo = true;
            }
            else
            {
                dialogueObj.transform.GetChild(0).GetComponent<TextTyper>().message = dialogue[]
            }
        }
    }

    public void TalkToNPC()
    {
        uiCanvas.SetActive(true);
        if (playerObj.transform.position.y - gameObject.transform.position.y >= 0) //If player is up
        {
            anim.Play(faceBack);
        }
        if (playerObj.transform.position.y - gameObject.transform.position.y < 0) //If player is down
        {
            anim.Play(faceForward);
        }
        if (playerObj.transform.position.x - gameObject.transform.position.x >= 0) //If player is right
        {
            anim.Play(faceRight);
        }
        if (playerObj.transform.position.x - gameObject.transform.position.x < 0) //If player is left
        {
            anim.Play(faceLeft);
        }
    }
}
