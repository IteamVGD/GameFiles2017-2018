using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    public bool cityToCityDoor; //If true, transitions from city -> city
    public bool cityToLevel; //If true, transitions from city -> level
    public bool levelToCity; //If true, transitions from level -> city
    public bool teleportDoor; //Teleports the player to a new place through a fade transition. Mostly used for entering / exiting buildings
    public int inOutInt; //1 = player is indoors, 2 = player is outdoors

    public int buildingType; //If going into/outof building, this int is: 1 = hospital, 2 = market

    public GameObject playerObj;
    public int maxAccessRange; //The distance at which the player can access the door
    public GameObject gameControllerObj;
    public bool isBeingAccessed;

    public int nextID; //Which city/level to go to

    public bool hasRemovedBlockade;
    public GameObject firstTownBlockade;

    public bool playMusicOnEnable;
    public GameObject musicToRevertTo; //What to play after this object is done playing music (if it should, controlled by bool above)

    public GameObject linkedSpawnpoint; //If this door leads in/out of a building, this is where it will put the player
    public int tpHereYOffset; //If the player tps to this door this number will be added to his yPos;

    public string interactInputString;

    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        interactInputString = playerObj.GetComponent<PlayerController>().interactInput;
    }

    // Update is called once per frame
    void Update () {
        if(!isBeingAccessed)
        {
            if (!cityToCityDoor) //If this door goes from city -> level or level -> city
            {
                if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown(interactInputString)) && Vector3.Distance(gameObject.transform.position, playerObj.transform.GetChild(0).position) < maxAccessRange) //Opens door if E or UpDpad is pressed
                {
                    playerObj.transform.GetComponent<PlayerController>().timesKOd = 0;
                    gameControllerObj.transform.GetComponent<GameController>().startFadeOut(0.04f, 0.04f, 0.45f);
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
                        if(teleportDoor)
                        {
                            gameControllerObj.GetComponent<GameController>().playerTeleportSpot = linkedSpawnpoint;
                            gameControllerObj.GetComponent<GameController>().teleportDoorBeingUsed = gameObject;
                            gameControllerObj.GetComponent<GameController>().inOrOutIntTemp = inOutInt;
                            gameControllerObj.transform.GetComponent<GameController>().startFadeOut(0.04f, 0.04f, 0.45f);
                        }
                    }
                    isBeingAccessed = true;
                    StartCoroutine(isBeingAccessedTimer());
                    playerObj.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                }
            }
            else //If is a city -> city door
            {
                if(cityToCityDoor)
                {
                    if (Vector3.Distance(gameObject.transform.position, playerObj.transform.GetChild(0).position) < maxAccessRange) //If player is close enough to this object (no button press required)
                    {
                        StartCoroutine(gameControllerObj.transform.GetComponent<GameController>().ChangeViewFadeOut(0.03f, 0.03f, 0.55f));
                        gameControllerObj.transform.GetComponent<GameController>().cityID = nextID;
                        gameControllerObj.transform.GetComponent<GameController>().travellingToCity = true;
                    }
                }
 
            }
        }

        if (musicToRevertTo != null && !musicToRevertTo.activeSelf && !transform.GetComponent<AudioSource>().isPlaying)
            musicToRevertTo.SetActive(true);
    }

    private void OnEnable()
    {
        isBeingAccessed = false;
    }

    public IEnumerator isBeingAccessedTimer()
    {
        yield return new WaitForSeconds(1.5f); //Waits until player is gone so the door is interactable again
        isBeingAccessed = false;
        StopCoroutine(isBeingAccessedTimer());
    }
}
