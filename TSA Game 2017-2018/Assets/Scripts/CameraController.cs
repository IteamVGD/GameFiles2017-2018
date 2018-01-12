using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject playerObj;

    public float xDistance;
    public float yDistance;
    public float maxXDistance;
    public float maxYDistance;

    public float speed; 

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        xDistance = gameObject.transform.position.x - playerObj.transform.position.x;
        yDistance = gameObject.transform.position.y - playerObj.transform.position.y;

        float step = speed * Time.deltaTime;
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
        }

        
    }
}
