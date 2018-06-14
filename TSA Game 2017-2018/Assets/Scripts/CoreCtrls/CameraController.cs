using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject playerObj;
    public GameObject gameControllerObj;
    public GameObject paralaxBackgroundObj;

    public bool canFollowX;
    public bool canFollowY;
    public float smoothSpeed;
    public float parallaxSmoothSpeed;
    public Vector3 offset;
    public bool followY; //If the camera should track the player on the yAxis; If in sidescroll mode, no, topdown yes
    public Vector3 desiredPostion;
    public float yTrackHeight; //The y coordinate the player has to be at for the camera to start tracking him vertically
    public Vector3 sidescrollOffset;
    public Vector3 topDownOffset;
    public float constantY;
    public float parallaxConstantY; //The y coordinate that the scrolling sky stays at
    public int id;

    //Interior background color variables
    public float duration = 1.5f;
    public float t = 0;
    public Color color1;
    public Color color2;
    public bool switchFlow;

    private void Update()
    {
        if(gameControllerObj.transform.GetComponent<GameController>().inOrOutInt == 1)
        {
            if (switchFlow) //Controls the changing color when in a building
            {
                GetComponent<Camera>().backgroundColor = Color.Lerp(color1, color2, t);
            }
            else
            {
                GetComponent<Camera>().backgroundColor = Color.Lerp(color2, color1, t);
            }
            if (t < 1)
            {
                t += Time.deltaTime / duration;
            }
            else
            {
                switchFlow = !switchFlow;
                t = 0;
            }
        }

        //camera tracking stuff
        if (playerObj.transform.GetComponent<PlayerController>().gameControllerScript.currentView == 1 && (canFollowX || canFollowY))
        {
            id = gameControllerObj.transform.GetComponent<GameController>().cityID;
            if (gameControllerObj.transform.GetComponent<GameController>().inOrOutInt == 1)
            {
                if (playerObj.transform.position.x >= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.x || playerObj.transform.position.x <= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.x) //X bounds
                    canFollowX = false;
                if (playerObj.transform.position.y >= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.y || playerObj.transform.position.y <= playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.y) //Y bounds
                    canFollowY = false;
            }
            else
            {
                canFollowX = false;
                canFollowY = false;
            }
        }

        if (!canFollowX || !canFollowY)
        {
            if (playerObj.transform.GetComponent<PlayerController>().gameControllerScript.currentView != 1)
            {
                canFollowX = true;
                canFollowY = true;
            }
            else
            {
                if (gameControllerObj.transform.GetComponent<GameController>().inOrOutInt != 1)
                {
                    if (playerObj.transform.position.x < playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.x && playerObj.transform.position.x > playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.x)
                        canFollowX = true;
                    if (playerObj.transform.position.y < playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(0).position.y && playerObj.transform.position.y > playerObj.transform.GetComponent<PlayerController>().gameControllerScript.cities[id].transform.GetChild(1).position.y)
                        canFollowY = true;
                }
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
            if (canFollowX)
                desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, desiredPostion.y, -10);
            if (canFollowY)
                desiredPostion = new Vector3(desiredPostion.x, playerObj.transform.position.y + offset.y, -10);
        }
        else
        {
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, constantY + offset.y, -10);
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPostion, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        if (gameControllerObj.GetComponent<GameController>().currentView == 2) //This drags along the sky behind the window at a slightly different speed (parallaxSmoothSpeed)
        {
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, parallaxConstantY, - 5); 
            smoothedPosition = Vector3.Lerp(paralaxBackgroundObj.transform.position, desiredPostion, parallaxSmoothSpeed * Time.deltaTime);
            paralaxBackgroundObj.transform.position = smoothedPosition;
        }
    }
}
