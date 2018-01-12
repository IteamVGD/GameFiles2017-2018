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
    public bool letGoOfSpace; //this bool helps stop some jumping glitches

    public static int currentGloveInt; //Currently equiped glove's "index" value, starts at 0
    public static string currentGloveString = "Default Glove";
    public static int currentBeltInt; //Currently equiped belt's "index" value, starts at 0
    public static string currentBeltString = "Default Belt";

    public bool isIdle;
    public bool isCrouching;
    public int sideFacing; //1 = up/back, 2 = left, 3 = down/front, 4 = right
    public bool isUnderSomething; //used to stop player from uncrouching when in small area
    public BoxCollider2D triggerCollider;
    public bool crouchingButKeyIsUp;

    public Vector2 colliderOriginalSize;
    public Vector2 colliderOriginalOffset;

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
        colliderOriginalSize = gameObject.transform.GetComponent<BoxCollider2D>().size;
        colliderOriginalOffset = gameObject.transform.GetComponent<BoxCollider2D>().offset;
        foreach (BoxCollider2D collider in gameObject.transform.GetComponents<BoxCollider2D>())
        {
            if (collider.isTrigger == true)
            {
                triggerCollider = collider;
                triggerCollider.enabled = false;
                break;
            }
        }

        //finds walk controller animator
        animatorWalk = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.GetComponent<Rigidbody2D>().velocity.x > 0 || gameObject.transform.GetComponent<Rigidbody2D>().velocity.y > 0 || gameObject.transform.GetComponent<Rigidbody2D>().velocity.x < 0 || gameObject.transform.GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            isIdle = false;
        }
        animatorWalk.SetBool("isIdleBool", isIdle);
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
                sideFacing = 4;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                sideFacing = 2;
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, verticalMovementSpeed);
                sideFacing = 1;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, -verticalMovementSpeed);
                sideFacing = 3;
            }

            //Let go to stop
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                isIdle = true;
                sideFacing = 3;
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                isIdle = true;
                sideFacing = 3;
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                isIdle = true;
                sideFacing = 3;
            }
            if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                isIdle = true;
                sideFacing = 3;
            }
        }
        if (gameControllerScript.currentView == 2)
        {
            //Sidescroll Movement

            //Press to move
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                sideFacing = 4;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                sideFacing = 2;
            }

            if (Input.GetKeyDown(KeyCode.Space) && letGoOfSpace == false)
            {
                jumpheightTimerInt = 0;
                goToMinJump = true;
            }

            if (goToMinJump == true)
            {
                if (jumpheightTimerInt < minJumpHeight)
                {
                    jumpheightTimerInt += 1;
                    gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, jumpSpeedInt);
                }
                else
                {
                    goToMinJump = false;
                    if (letGoOfSpace == true)
                    {
                        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                    }
                }
            }

            if (Input.GetKey(KeyCode.Space) && goToMinJump == false && letGoOfSpace == false)
            {
                if (jumpheightTimerInt < maxJumpHeight)
                {
                    jumpheightTimerInt += 1;
                    gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, jumpSpeedInt);
                }
            }

            if (Input.GetKeyUp(KeyCode.Space) && letGoOfSpace == false)
            {
                if (isIdle == false)
                {
                    if (isCrouching == false)
                    {
                        isIdle = true;
                        sideFacing = 3;
                    }
                    letGoOfSpace = true;
                    if (jumpheightTimerInt >= minJumpHeight)
                    {
                        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                    }
                }
            }

            //Press to Crouch / Duck
            if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                isCrouching = true;
                animatorWalk.SetBool("isCrouching", true);
                triggerCollider.enabled = true;
                gameObject.transform.GetComponent<BoxCollider2D>().
                gameObject.transform.GetComponent<BoxCollider2D>().size = new Vector2(1, 1.3f);
                gameObject.transform.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.35f);
            }

            //Let go to stop
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                if (isCrouching == false)
                {
                    isIdle = true;
                    sideFacing = 3;
                }
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                if (isCrouching == false)
                {
                    isIdle = true;
                    sideFacing = 3;
                }
            }
            if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                if(isUnderSomething == false)
                {
                    isCrouching = false;
                    triggerCollider.enabled = false; //disables trigger collider to avoid "OnTriggerStay" running continuously and causing lag
                    animatorWalk.SetBool("isCrouching", false); //sets animator isCrouching bool to false
                    isIdle = true;
                    sideFacing = 3; //side facing = forward/toward camera
                    gameObject.transform.GetComponent<BoxCollider2D>().size = colliderOriginalSize; //resets main boxcollider's size and offset to match character forward model
                    gameObject.transform.GetComponent<BoxCollider2D>().offset = colliderOriginalOffset;
                }
                else
                {
                    crouchingButKeyIsUp = true;
                }
            }

            if(crouchingButKeyIsUp == true && isUnderSomething == false)
            {
                isCrouching = false;
                triggerCollider.enabled = false; //disables trigger collider to avoid "OnTriggerStay" running continuously and causing lag
                animatorWalk.SetBool("isCrouching", false); //sets animator isCrouching bool to false
                isIdle = true;
                sideFacing = 3; //side facing = forward/toward camera
                gameObject.transform.GetComponent<BoxCollider2D>().size = colliderOriginalSize; //resets main boxcollider's size and offset to match character forward model
                gameObject.transform.GetComponent<BoxCollider2D>().offset = colliderOriginalOffset;
                crouchingButKeyIsUp = false;
            }
        }

        //Attack Code
        if (Input.GetMouseButtonDown(0) && isPunching == false) //Get mouse button not working
        {
            StartCoroutine(Punch());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8 && collision.collider == collision.transform.GetComponent<ObjectCollisionController>().canJumpCollider) //8 == "Floor" layer
        {
            if(letGoOfSpace == true)
            {
                letGoOfSpace = false;
            }
            if(isIdle == false)
            {
                isIdle = true;
            }
            if(canJump == false)
            {
                isIdle = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8 && collision.collider == collision.transform.GetComponent<ObjectCollisionController>().canJumpCollider) //8 == "Floor" layer
        {
            canJump = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(isCrouching)
        {
            isUnderSomething = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isCrouching && isUnderSomething == true)
        {
            isUnderSomething = false;
        }
    }

    IEnumerator Punch() //commented out sprite renderer changes since we are using the animator to do this instead
    {
        isPunching = true;
        //tempSprite = gameObject.transform.GetComponent<SpriteRenderer>().sprite;
        //gameObject.transform.GetComponent<SpriteRenderer>().sprite = playerSpriteList[4];
        yield return new WaitForSeconds(punchTime);
        //gameObject.transform.GetComponent<SpriteRenderer>().sprite = tempSprite;
        isPunching = false;
    }
}
