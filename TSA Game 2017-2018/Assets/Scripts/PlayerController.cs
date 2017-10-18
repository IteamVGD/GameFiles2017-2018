using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameController gameControllerScript;

    //Movement Variables
    public float horizontalMovementSpeed;
    public float verticalMovementSpeed;
    public float jumpSpeed;
    public bool canJump;
    public bool movementKeyBeingPressed;

    private void Awake()
    {
        gameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gameControllerScript.playerObj = gameObject.transform.parent.gameObject;
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q))
        {
            gameControllerScript.ChangeView();
        }

        if (gameControllerScript.currentView == 1)
        {
            //Topdown Movement

            //Press down to move
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, verticalMovementSpeed);
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, -verticalMovementSpeed);
            }

            //Let go to stop
            if(Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
            }
            if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
            }
        }
        if (gameControllerScript.currentView == 2)
        {
            //Sidescroll Movement

            //Press to move
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
            if(Input.GetKey(KeyCode.Space))
            {
                if(canJump == true)
                {
                    gameObject.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpSpeed));
                    canJump = false;
                }
            }

            //Let go to stop
            if(Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            canJump = true;
        }
    }
}
