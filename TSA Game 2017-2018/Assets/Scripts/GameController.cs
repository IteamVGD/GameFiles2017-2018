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
        sidescrollUIControllerScript.enabled = true;
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        sideScrollMapObj.SetActive(true);
        topDownMapObj.SetActive(false);
        playerObj.transform.GetChild(0).transform.position = new Vector3(0, 0, 0);
    }

    public void changeToTopdown()
    {
        topdownUIControllerScript.enabled = true;
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        sideScrollMapObj.SetActive(false);
        topDownMapObj.SetActive(true);
        playerObj.transform.GetChild(0).transform.position = new Vector3(0, 0, 0);
    }
}
