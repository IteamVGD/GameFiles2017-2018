using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject playerObj;
    public float speed; //The speed at which the camera follows the player. 3.98 is default and should NOT BE MESSED WITH b/c otherwise the player might catch the camera or the camera will go too fast and there will be "jittering"
    public float smoothSpeed;
    public Vector3 offset;
    public bool followY; //If the camera should track the player on the yAxis; If in sidescroll mode, no, topdown yes
    public Vector3 desiredPostion;

    private void FixedUpdate()
    {
        if(followY)
        {
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, playerObj.transform.position.y + offset.y, gameObject.transform.position.z + offset.z);
        }
        else
        {
            desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, gameObject.transform.position.y + offset.y, gameObject.transform.position.z + offset.z);
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPostion, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
