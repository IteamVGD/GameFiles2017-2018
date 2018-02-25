using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public int talkRange; //How close the player needs to be to talk to this npc
    public GameObject playerObj;
    public GameObject uiCanvas;
	
	// Update is called once per frame
	void Update () {
        if ((Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire3")) && Vector3.Distance(gameObject.transform.position, playerObj.transform.position) < talkRange)
        {

        }
    }

    public void TalkToNPC()
    {
        uiCanvas.SetActive(true);
    }
}
