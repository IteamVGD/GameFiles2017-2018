using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxController : MonoBehaviour {

    public int childInt; //used to define which box this is AND which text to change, defined in inspector. 0 = glove, 1 = belt, etc
    public GameObject playerObj;

    public GameObject glovesTextObj;
    public GameObject beltTextObj;

    public string glovesTempText;
    public string beltTempText;

	// Use this for initialization
	void Start () {
        playerObj = GameObject.FindGameObjectWithTag("Player");
	}
	
	/*// Update is called once per frame. commented out to reduce lag until this update function is needed.
	void Update () {
		
	}*/

    private void OnMouseEnter()
    {
        glovesTempText = glovesTextObj.transform.GetComponent<Text>().text;
        beltTempText = beltTextObj.transform.GetComponent<Text>().text;
        if (childInt == 0)
        {
            glovesTextObj.transform.GetComponent<Text>().text = playerObj.transform.GetChild(0).GetComponent<PlayerController>().currentGloveString;
        }
        if (childInt == 1)
        {
            beltTextObj.transform.GetComponent<Text>().text = playerObj.transform.GetChild(0).GetComponent<PlayerController>().currentBeltString;
        }
    }

    private void OnMouseExit()
    {
        beltTextObj.transform.GetComponent<Text>().text = beltTempText;
        glovesTextObj.transform.GetComponent<Text>().text = glovesTempText;
    }
}
