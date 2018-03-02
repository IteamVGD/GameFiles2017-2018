using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDownPunchCollider : MonoBehaviour
{

    public GameObject playerObj;
    public bool shouldCheck;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(shouldCheck)
        {
            if (collision.gameObject.layer == 16)
            {
                if (collision.GetComponent<SwitchController>().triggerSongObj)
                    collision.GetComponent<SwitchController>().TriggerSong();
                playerObj.transform.GetComponent<PlayerController>().canDownPunch = true;
                playerObj.transform.GetComponent<PlayerController>().gameControllerScript.ManageBoxes();
                playerObj.transform.GetComponent<PlayerController>().DownPunch();
                shouldCheck = true;
            }
            else
            {
                if (collision.transform.tag == "DownPunchable" || collision.transform.tag == "EnemyDownPunchable")
                {
                    playerObj.transform.GetComponent<PlayerController>().canDownPunch = true;
                    playerObj.transform.GetComponent<PlayerController>().DownPunch();
                    shouldCheck = true;
                }
            }
        }
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        shouldCheck = true;
    }
}
