using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBagController : MonoBehaviour {

    public float speed;
    public int newVel;
    public float rotZ;

    // Use this for initialization
    void Start () {
        
        newVel = 2;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(2, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
    }

    // Update is called once per frame
    void Update () {
        speed = gameObject.transform.GetComponent<Rigidbody2D>().velocity.x;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(newVel, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
        transform.Rotate(Vector3.forward * Time.deltaTime * newVel * -90);
	}

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Object" || collision.transform.tag == "Player")
        {
            if (newVel == 2)
            {
                newVel = -2;
            }
            else
            {
                newVel = 2;
            }
        }
    }
}
