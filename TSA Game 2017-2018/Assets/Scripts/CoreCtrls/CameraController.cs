using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject playerObj;
    public float smoothSpeed;
    public Vector3 offset;
    public bool followY; //If the camera should track the player on the yAxis; If in sidescroll mode, no, topdown yes
    public Vector3 desiredPostion;

    private void FixedUpdate()
    {
        if(followY)
        {
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, playerObj.transform.position.y + offset.y, -10);
        }
        else
        {
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, gameObject.transform.position.y + offset.y, -10);
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPostion, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
