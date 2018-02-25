using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject playerObj;
    public bool canFollow; //Stops camera from going off map in topdown mode
    public float smoothSpeed;
    public Vector3 offset;
    public bool followY; //If the camera should track the player on the yAxis; If in sidescroll mode, no, topdown yes
    public Vector3 desiredPostion;
    public float yTrackHeight; //The y coordinate the player has to be at for the camera to start tracking him vertically
    public Vector3 sidescrollOffset;
    public Vector3 topDownOffset;
    public float constantY;

    private void FixedUpdate()
    {
        if(playerObj.transform.GetComponent<PlayerController>().gameControllerScript.currentView == 1 && canFollow)
        {
            int id = playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cityID;
            if (playerObj.transform.position.x >= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.x || playerObj.transform.position.x <= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.x) //X bounds
                canFollow = false;
            if (playerObj.transform.position.y >= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.y || playerObj.transform.position.y <= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.y) //X bounds
                canFollow = false;
        }

        if(!canFollow)
        {
            if (playerObj.transform.GetComponent<PlayerController>().gameControllerScript.currentView != 1)
            {
                canFollow = true;
            }
            else
            {
                int id = playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cityID;
                if (playerObj.transform.position.x < playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.x && playerObj.transform.position.x > playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.x && playerObj.transform.position.y < playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.y && playerObj.transform.position.y > playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.y) //X bounds
                    canFollow = true;
            }
        }

        if (playerObj.transform.GetComponent<PlayerController>().gameControllerScript.currentView == 2 && !followY && playerObj.transform.position.y > yTrackHeight)
        {
            followY = true;
        }
        else
        {
            if (playerObj.transform.GetComponent<PlayerController>().gameControllerScript.currentView == 2 && followY && playerObj.transform.position.y < yTrackHeight)
            {
                followY = false;
            }
        }

        if (followY)
        {
            if(canFollow)
                desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, playerObj.transform.position.y + offset.y, -10);
        }
        else
        {
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, constantY + offset.y, -10);
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPostion, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
