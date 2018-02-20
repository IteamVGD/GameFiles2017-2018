using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    public GameObject playerObj;
    public bool isClose; //Bool that controls wether the player is close enough to "open/enter" the door
    public int maxAccessRange; //The distance at which the player can access the door
    public GameObject gameControllerObj;

	// Update is called once per frame
	void Update () {

		if((Input.GetKeyDown(KeyCode.E) || playerObj.transform.GetChild(0).GetComponent<PlayerController>().upPressed) && isClose) //Opens door if E or UpDpad is pressed
        {
            StartCoroutine(gameControllerObj.transform.GetComponent<GameController>().ChangeViewFadeOut(0.04f, 0.043f, 0.5f));
        }

        if(Vector3.Distance(gameObject.transform.position, playerObj.transform.GetChild(0).position) < maxAccessRange)
        {
            isClose = true;
        }
        else
        {
            isClose = false;
        }
    }
}
