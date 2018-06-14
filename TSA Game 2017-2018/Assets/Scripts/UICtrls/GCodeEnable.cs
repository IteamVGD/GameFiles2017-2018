using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCodeEnable : MonoBehaviour { //This is only used to enable GCode

    public GameObject playerObj;
    public GameObject indicator;

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            playerObj.transform.GetChild(0).GetComponent<PlayerController>().GCodeEnabled = true;
            indicator.SetActive(true);
        }
    }
}
