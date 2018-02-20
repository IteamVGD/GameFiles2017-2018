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
}
