using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameController gameControllerScript;

    //Movement Variables
    public float horizontalMovementSpeed;
    public float verticalMovementSpeed;
    public float jumpSpeed;
    public bool canJump;
    public bool isJumping;
    public bool movementKeyBeingPressed;

    public float punchTime = 0.8f; //Time a punch stays displayed
    public bool isPunching;

    public Sprite tempSprite; //Used to keep track of what sprite was in use before attacking;
    public List<Sprite> playerSpriteList; //0 = front, 1 = facing right, 2 = facing left, 3 = back NOTE Back + Attack are makeshift sprites made by Gabe for now

    public int jumpheightTimerInt; //counts up as you hold space, allows for multiple jump heights
    public int maxJumpHeight = 20;
    public int minJumpHeight = 5;
    public int jumpSpeedInt = 5; //how fast he jumps
    public bool goToMinJump; //if true, initiates loop in update to go to min jump height
    public bool letGoOfSpace;

    //Animator
    Animator animatorWalk;

    private void Awake()
    {
        gameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gameControllerScript.playerObj = gameObject.transform.parent.gameObject;
    }

    // Use this for initialization
    void Start()
    {

        //Sets sprite at start to facing forward idle
        gameObject.transform.GetComponent<SpriteRenderer>().sprite = playerSpriteList[0];

        //finds walk controller animator
        animatorWalk = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        animatorWalk.SetInteger("HorizontalSpeedTD", (int)gameObject.transform.GetComponent<Rigidbody2D>().velocity.x); //Velocity based animations
        animatorWalk.SetInteger("VerticalSpeedTD", (int)gameObject.transform.GetComponent<Rigidbody2D>().velocity.y); //TD = Topdown variables
        if (Input.GetKeyDown(KeyCode.Q))
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
                //gameObject.transform.GetComponent<SpriteRenderer>().sprite = playerSpriteList[1];
                //changes animation
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                //gameObject.transform.GetComponent<SpriteRenderer>().sprite = playerSpriteList[2];
                //changes animation
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, verticalMovementSpeed);
                //gameObject.transform.GetComponent<SpriteRenderer>().sprite = playerSpriteList[3];
                //changes animation
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, -verticalMovementSpeed);
                //gameObject.transform.GetComponent<SpriteRenderer>().sprite = playerSpriteList[0];
                //changes animation
            }

            //Let go to stop
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                //changes animation
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                //changes animation
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                //changes animation
            }
            if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                //changes animation
            }
        }
        if (gameControllerScript.currentView == 2)
        {
            //Sidescroll Movement

            //Press to move
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                gameObject.transform.GetComponent<SpriteRenderer>().sprite = playerSpriteList[1];
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                gameObject.transform.GetComponent<SpriteRenderer>().sprite = playerSpriteList[2];
            }
            /*if(Input.GetKey(KeyCode.Space))
            {
                if(canJump == true)
                {
                    gameObject.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpSpeed));
                    canJump = false;
                }
            }*/

            if(Input.GetKeyDown(KeyCode.Space))
            {
                jumpheightTimerInt = 0;
                goToMinJump = true;
                letGoOfSpace = false;
            }

            if(goToMinJump == true)
            {
                if (jumpheightTimerInt < minJumpHeight)
                {
                    jumpheightTimerInt += 1;
                    gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, jumpSpeedInt);
                }
                else
                {
                    goToMinJump = false;
                    if(letGoOfSpace == true)
                    {
                        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                    }
                }
            }

            if (Input.GetKey(KeyCode.Space) && goToMinJump == false)
            {
                if (jumpheightTimerInt < maxJumpHeight)
                {
                    jumpheightTimerInt += 1;
                    gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, jumpSpeedInt);
                }
            }

            if(Input.GetKeyUp(KeyCode.Space))
            {
                letGoOfSpace = true;
                if (jumpheightTimerInt >= minJumpHeight)
                {
                    gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                }
            }

            //Let go to stop
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
            }
        }

        //Attack Code
        if (Input.GetMouseButtonDown(0) && isPunching == false) //Get mouse button not working
        {
            StartCoroutine(Punch());
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            if (canJump == false || isJumping == true)
            {
                canJump = true;
                isJumping = false;
            }
        }
    }*/

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            if (canJump == false)
            {
                canJump = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            canJump = false;
        }
    }

    IEnumerator Punch()
    {
        isPunching = true;
        tempSprite = gameObject.transform.GetComponent<SpriteRenderer>().sprite;
        gameObject.transform.GetComponent<SpriteRenderer>().sprite = playerSpriteList[4];
        yield return new WaitForSeconds(punchTime);
        gameObject.transform.GetComponent<SpriteRenderer>().sprite = tempSprite;
        isPunching = false;
    }
}
