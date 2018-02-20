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
}
