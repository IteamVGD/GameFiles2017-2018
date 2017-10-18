using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidescrollUIController : MonoBehaviour {

    public GameController gameControllerScript;
    public TopdownUIController topdownUIControllerScript;

    private void Awake()
    {
        gameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        topdownUIControllerScript = gameObject.transform.GetComponent<TopdownUIController>();
    }

    // Use this for initialization
    void Start () {
        
    }

    private void OnEnable()
    {
        topdownUIControllerScript.enabled = false;
        gameControllerScript.PerspectiveTextObj.GetComponent<Text>().text = "Sidescroller";
    }

    // Update is called once per frame
    void Update () {
		
	}
}
