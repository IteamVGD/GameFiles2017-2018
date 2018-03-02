using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    public bool cityToCityDoor; //If true, transitions from city -> city
    public bool cityToLevel; //If true, transitions from city -> level
    public bool levelToCity; //If true, transitions from level -> city

    public GameObject playerObj;
    public int maxAccessRange; //The distance at which the player can access the door
    public GameObject gameControllerObj;
    public bool isBeingAccessed;

    public int nextID; //Which city/level to go to

    public bool hasRemovedBlockade;
    public GameObject firstTownBlockade;

    public bool playMusicOnEnable;
    public GameObject musicToRevertTo; //What to play after this object is done playing music (if it should, controlled by bool above)

    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update () {
        if(!isBeingAccessed)
        {
            if (!cityToCityDoor) //If this door goes from city -> level or level -> city
            {
                if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire2")) && Vector3.Distance(gameObject.transform.position, playerObj.transform.GetChild(0).position) < maxAccessRange) //Opens door if E or UpDpad is pressed
                {
                    playerObj.transform.GetComponent<PlayerController>().timesKOd = 0;
                    StartCoroutine(gameControllerObj.transform.GetComponent<GameController>().ChangeViewFadeOut(0.04f, 0.04f, 0.45f));
                    if (cityToLevel)
                    {
                        gameControllerObj.transform.GetComponent<GameController>().levelID = nextID;
                        gameControllerObj.transform.GetComponent<GameController>().travellingToLevel = true;
                    }
                    else
                    {
                        if (levelToCity)
                        {
                            gameControllerObj.transform.GetComponent<GameController>().cityID = nextID;
                            gameControllerObj.transform.GetComponent<GameController>().travellingToCity = true;
                            if(!hasRemovedBlockade && nextID == 0)
                            {
                                firstTownBlockade.SetActive(false);
                                hasRemovedBlockade = true;
                            }
                        }                            
                    }
                    isBeingAccessed = true;
                }
            }
            else //If is a city -> city door
            {
                if (Vector3.Distance(gameObject.transform.position, playerObj.transform.GetChild(0).position) < maxAccessRange) //If player is close enough to this object (no button press required)
                {
                    StartCoroutine(gameControllerObj.transform.GetComponent<GameController>().ChangeViewFadeOut(0.03f, 0.03f, 0.55f));
                    gameControllerObj.transform.GetComponent<GameController>().cityID = nextID;
                    gameControllerObj.transform.GetComponent<GameController>().travellingToCity = true;
                }   
            }
        }

        if (musicToRevertTo != null && !musicToRevertTo.activeSelf)
            musicToRevertTo.SetActive(true);
    }

    private void OnEnable()
    {
        isBeingAccessed = false;
    }
}
