using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject playerObj;
    public float smoothSpeed;
    public Vector3 offset;
    public bool followY; //If the camera should track the player on the yAxis; If in sidescroll mode, no, topdown yes
    public Vector3 desiredPostion;
    public float yTrackHeight; //The y coordinate the player has to be at for the camera to start tracking him vertically

    private void FixedUpdate()
    {
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
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, playerObj.transform.position.y + offset.y, -10);
        }
        else
        {
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, 0 + offset.y, -10);
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPostion, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
