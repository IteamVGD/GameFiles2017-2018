using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject playerObj;

    public GameObject PerspectiveTextObj;
    public int currentView; //1 = topdown, 2 = sidecroll, effect movement
    public GameObject sideScrollMapObj;
    public GameObject topDownMapObj;

    public TopdownUIController topdownUIControllerScript;
    public SidescrollUIController sidescrollUIControllerScript;

    public void Awake()
    {
        topdownUIControllerScript = gameObject.transform.GetChild(0).GetComponent<TopdownUIController>();
        sidescrollUIControllerScript = gameObject.transform.GetChild(0).GetComponent<SidescrollUIController>();
    }
    

    // Use this for initialization
    void Start () {
        currentView = 1;
        playerObj.transform.GetChild(0).GetComponent<Animator>().SetInteger("currentViewInt", 1); //sets int in player's animator to currentView, letting the animations be topdown/sidescroll dependent
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ChangeView()
    {
        if(currentView == 1)
        {
            currentView = 2;
            changeToSidescroll();
            return;
        }
        if(currentView == 2)
        {
            currentView = 1;
            changeToTopdown();
            return;
        }
    }

    public void changeToSidescroll()
    {
        sidescrollUIControllerScript.enabled = true; //Enables sidescroll ui controller script
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 2.5f; //Makes player react to gravity
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops player
        sideScrollMapObj.SetActive(true); //Enables sidescroll map
        topDownMapObj.SetActive(false); //Disables topdown map
        playerObj.transform.GetChild(0).transform.position = new Vector3(-5, -3, 0); //Resets player to middle of screen
        playerObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = playerObj.transform.GetChild(0).GetComponent<PlayerController>().playerSpriteList[0]; //Sets player's sprite to facing forward
        playerObj.transform.GetChild(0).GetComponent<Animator>().SetInteger("currentViewInt", currentView); //sets int in player's animator to currentView, letting the animations be topdown/sidescroll dependent
    }

    public void changeToTopdown()
    {
        topdownUIControllerScript.enabled = true; //Enables topdown ui controller script
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 0.0f; //Stops player from reacting to gravity
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops player
        sideScrollMapObj.SetActive(false); //Disables sidescroll map
        topDownMapObj.SetActive(true); //Enables topdown map
        playerObj.transform.GetChild(0).transform.position = new Vector3(0, 0, 0); //Resets player to middle of screen
        playerObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = playerObj.transform.GetChild(0).GetComponent<PlayerController>().playerSpriteList[0]; //Sets player's sprite to facing forward
        playerObj.transform.GetChild(0).GetComponent<Animator>().SetInteger("currentViewInt", currentView); //sets int in player's animator to currentView, letting the animations be topdown/sidescroll dependent
    }
}
