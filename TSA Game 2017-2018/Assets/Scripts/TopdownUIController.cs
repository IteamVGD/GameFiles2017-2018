using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopdownUIController : MonoBehaviour {

    public GameController gameControllerScript;
    public SidescrollUIController sidescrollUIControllerScript;

    private void Awake()
    {
        gameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        sidescrollUIControllerScript = gameObject.transform.GetComponent<SidescrollUIController>();
    }

    // Use this for initialization
    void Start () {

    }

    private void OnEnable()
    {
        sidescrollUIControllerScript.enabled = false;
        gameControllerScript.PerspectiveTextObj.GetComponent<Text>().text = "Topdown";
    }

    // Update is called once per frame
    void Update () {
		
	}
}
