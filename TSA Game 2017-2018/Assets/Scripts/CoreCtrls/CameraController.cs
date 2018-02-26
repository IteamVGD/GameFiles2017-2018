using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject playerObj;
    public bool canFollowX;
    public bool canFollowY;
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
        if(playerObj.transform.GetComponent<PlayerController>().gameControllerScript.currentView == 1 && (canFollowX || canFollowY))
        {
            int id = playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cityID;
            if (playerObj.transform.position.x >= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.x || playerObj.transform.position.x <= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.x) //X bounds
                canFollowX = false;
            if (playerObj.transform.position.y >= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.y || playerObj.transform.position.y <= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.y) //X bounds
                canFollowY = false;
        }

        if(!canFollowX || !canFollowY)
        {
            if (playerObj.transform.GetComponent<PlayerController>().gameControllerScript.currentView != 1)
            {
                canFollowX = true;
                canFollowY = true;
            }
            else
            {
                int id = playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cityID;
                if (playerObj.transform.position.x < playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.x && playerObj.transform.position.x > playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.x)
                    canFollowX = true;
                if (playerObj.transform.position.y < playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.y && playerObj.transform.position.y > playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.y)
                    canFollowY = true;
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
            if(canFollowX)
                desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, desiredPostion.y, -10);
            if(canFollowY)
                desiredPostion = new Vector3(desiredPostion.x, playerObj.transform.position.y + offset.y, -10);
        }
        else
        {
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, constantY + offset.y, -10);
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPostion, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
