using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject playerObj;

    public float xDistance;
    public float yDistance;
    public float maxXDistance; //The max distance on the X xxis that the player can be from the camera
    public float maxYDistance; //The max distance on the Y axis that the player can be from the camera (currently not used as there is not much elevation)

    public float speed; //The speed at which the camera follows the player. 3.98 is default and should NOT BE MESSED WITH b/c otherwise the player might catch the camera or the camera will go too fast and there will be "jittering"
    public float smoothSpeed;
    public Vector3 offset;
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        /*xDistance = gameObject.transform.position.x - playerObj.transform.position.x;
        yDistance = gameObject.transform.position.y - playerObj.transform.position.y;

        if(xDistance > 0 && xDistance > maxXDistance)
        {
            gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
        }
        else
        {
            if (xDistance < 0 && xDistance < -maxXDistance)
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
            }
            else
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }*/

        
    }

    private void FixedUpdate()
    {
        Vector3 desiredPostion = new Vector3(playerObj.transform.position.x + offset.x, gameObject.transform.position.y + offset.y, gameObject.transform.position.z + offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPostion, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
