using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxController : MonoBehaviour {

    public string tempText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseEnter()
    {
        if(gameObject.transform.name == "GloveItemBox")
        {
            tempText = gameObject.transform.parent.gameObject.transform.parent.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<Text>().text;
            gameObject.transform.parent.gameObject.transform.parent.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<Text>().text = PlayerController.currentGloveString;
        }
    }

    private void OnMouseExit()
    {
        gameObject.transform.parent.gameObject.transform.parent.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<Text>().text = tempText;
    }
}
