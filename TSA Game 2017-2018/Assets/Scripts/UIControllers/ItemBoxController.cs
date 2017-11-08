using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxController : MonoBehaviour {

    public string tempText;
    public int childInt; //used to define which box this is AND which text to change, defined in inspector. 0 = glove, 1 = belt, etc

	// Use this for initialization
	void Start () {
		
	}
	
	/*// Update is called once per frame. commented out to reduce lag until this update function is needed.
	void Update () {
		
	}*/

    private void OnMouseEnter()
    {
        tempText = gameObject.transform.parent.gameObject.transform.parent.transform.GetChild(0).transform.GetChild(childInt).transform.GetComponent<Text>().text;
        if (childInt == 0)
        {
            gameObject.transform.parent.gameObject.transform.parent.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<Text>().text = PlayerController.currentGloveString;
        }
        if (childInt == 1)
        {
            gameObject.transform.parent.gameObject.transform.parent.transform.GetChild(0).transform.GetChild(childInt).transform.GetComponent<Text>().text = PlayerController.currentBeltString;
        }
    }

    private void OnMouseExit()
    {
       gameObject.transform.parent.gameObject.transform.parent.transform.GetChild(0).transform.GetChild(childInt).transform.GetComponent<Text>().text = tempText;
    }
}
