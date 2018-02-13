using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameController gameControllerScript;

    //Stats
    public int health = 30; //Preety self explanatory. Take hit -> take damage. Go to 0 -> "KO" countdown
    public int blockMeter; //From 0 - 100, decreases rapidly when blocking, if hits 0 block drops

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

    public int currentGloveInt; //Currently equiped glove's "index" value, starts at 0
    public string currentGloveString;
    public int currentBeltInt; //Currently equiped belt's "index" value, starts at 0
    public string currentBeltString;

    public bool isIdle;
    public bool isCrouching;
    public int sideFacing; //Topdown: 1 = up/back, 2 = left, 3 = down/front, 4 = right; Sidescroller: 4-> right, 2-> left, 3-> idle
    public int previousSideFacing; //For sidescroller, keeps 4 or 2 to know to face right or left when going idle
    public bool isUnderSomething; //used to stop player from uncrouching when in small area
    public BoxCollider2D triggerCollider;
    public bool crouchingButKeyIsUp;

    public Vector2 colliderOriginalSize;
    public Vector2 colliderOriginalOffset;

    //Attack Variables
    public int punchDamage; //Changes according to how long you hold it. Default is 10
    public int punchDamageCap = 20; //Used for variable charge damage system

    public int spamPunchTimerInt; //Used to keep animation from changing off punch if spamming
    public bool punchIsPressed;
    public float initialAnimSpeed;
    
    //NOTE: Most actual punch detection is done in PunchCollideController on the punch child obj

    //Animator
    Animator animatorWalk;

    //CONTROLLER DOCUMENTATION: 0 = A, 1 = B, 6 = start, 7 = start, 

    public float xAxisFloat; //if -1, dpad left, if 1 dpad right, 0 = idle
    public bool leftPressed; //true if xAxis -1
    public bool rightPressed; //true if xAxis 1
    public float yAxisFloat; //if -1, dpad down, if 1 dpad up, 0 = idle
    public bool upPressed; //true if yAxis 1
    public bool downPressed; //true if yAxis -1

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
        if (isPunching == false && gameObject.transform.GetComponent<Animator>().speed != 1)
        {
            gameObject.transform.GetComponent<Animator>().speed = 1;
        }
        xAxisFloat = Input.GetAxis("Horizontal");
        yAxisFloat = Input.GetAxis("Vertical");
        if(xAxisFloat == 1)
        {
            leftPressed = false;
            rightPressed = true;
        }
        else
        {
            if(xAxisFloat == -1)
            {
                leftPressed = true;
                rightPressed = false;
            }
        }
        if(yAxisFloat == 1)
        {
            upPressed = true;
            downPressed = false;
        }
        else
        {
            if (yAxisFloat == -1)
            {
                upPressed = false;
                downPressed = true;
            }
        }
     
        animatorWalk.SetInteger("sideFacingInt", sideFacing);
        animatorWalk.SetInteger("previousSideFacing", previousSideFacing);
        if (gameObject.transform.GetComponent<Rigidbody2D>().velocity.x > 0 || gameObject.transform.GetComponent<Rigidbody2D>().velocity.y > 0 || gameObject.transform.GetComponent<Rigidbody2D>().velocity.x < 0 || gameObject.transform.GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            isIdle = false;
            transform.GetComponent<SpriteRenderer>().flipX = false;
        }
        animatorWalk.SetInteger("HorizontalSpeedTD", (int)gameObject.transform.GetComponent<Rigidbody2D>().velocity.x); //Velocity based animations
        animatorWalk.SetInteger("VerticalSpeedTD", (int)gameObject.transform.GetComponent<Rigidbody2D>().velocity.y); //TD = Topdown variables

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
            if(Input.GetKey(KeyCode.D) || (rightPressed == true && xAxisFloat == 1))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                sideFacing = 4;
                previousSideFacing = 4;
            }
            if (Input.GetKey(KeyCode.A) || (leftPressed == true && xAxisFloat == -1))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-horizontalMovementSpeed, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                previousSideFacing = 2;
                sideFacing = 2;
            }


            if (Input.GetKeyDown(KeyCode.K) && letGoOfSpace == false)
            {
                animatorWalk.SetBool("canJumpBool", false);
                jumpheightTimerInt = 0;
                goToMinJump = true;
            }
            if (Input.GetButtonDown("Fire1") && letGoOfSpace == false)
            {
                animatorWalk.SetBool("canJumpBool", false);
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

            if (Input.GetButton("Fire1") && goToMinJump == false && letGoOfSpace == false)
            {
                if (jumpheightTimerInt < maxJumpHeight)
                {
                    jumpheightTimerInt += 1;
                    gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, jumpSpeedInt);
                }
            }

            if (Input.GetKey(KeyCode.K) && goToMinJump == false && letGoOfSpace == false)
            {
                if (jumpheightTimerInt < maxJumpHeight)
                {
                    jumpheightTimerInt += 1;
                    gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.transform.GetComponent<Rigidbody2D>().velocity.x, jumpSpeedInt);
                }
            }

            if (Input.GetButtonUp("Fire1") && letGoOfSpace == false)
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

            if (Input.GetKeyUp(KeyCode.K) && letGoOfSpace == false)
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


            //Press to Crouch / Duck, needs "re-implementing"/"re-thinking" without movement while crouching
            /*if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                isCrouching = true;
                animatorWalk.SetBool("isCrouching", true);
                triggerCollider.enabled = true;
                gameObject.transform.GetComponent<BoxCollider2D>().size = new Vector2(1, 1.3f);
                gameObject.transform.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.35f);
            }*/

            //Let go to stop
            if (Input.GetKeyUp(KeyCode.D) || (rightPressed == true && xAxisFloat != 1))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                if (isCrouching == false)
                {
                    isIdle = true;
                    sideFacing = 3;
                    rightPressed = false;
                }
            }
            if (Input.GetKeyUp(KeyCode.A) || (leftPressed == true && xAxisFloat != -1))
            {
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
                if (isCrouching == false)
                {
                    isIdle = true;
                    sideFacing = 3;
                    leftPressed = false;
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
        if (Input.GetButtonDown("Fire2") && isPunching == false)
        {
            StartCoroutine(Punch());
        }
        if (Input.GetKeyDown(KeyCode.J) && isPunching == false)
        {
            StartCoroutine(Punch());
        }
        if (Input.GetButton("Fire2") && isPunching == true) 
        {
            spamPunchTimerInt = 20;
            animatorWalk.SetBool("canStopPunching", false);
        }
        if (Input.GetKey(KeyCode.J) && isPunching == true) 
        {
            spamPunchTimerInt = 20;
            animatorWalk.SetBool("canStopPunching", false);
        }

        if(Input.GetKeyUp(KeyCode.J) && spamPunchTimerInt < 20)
        {
            StopAllCoroutines(); //Breaks out of punch / charge punch
            animatorWalk.SetBool("isPunching", false);
            horizontalMovementSpeed = 4;
            isPunching = false;
            gameObject.transform.GetComponent<Animator>().speed = initialAnimSpeed;
        }
        if (Input.GetButtonUp("Fire2") && spamPunchTimerInt < 20)
        {
            StopAllCoroutines(); //Breaks out of punch / charge punch
            animatorWalk.SetBool("isPunching", false);
            horizontalMovementSpeed = 4;
            isPunching = false;
            gameObject.transform.GetComponent<Animator>().speed = initialAnimSpeed;
        }

        if(Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire2"))
        {
            punchIsPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.J) || Input.GetButtonUp("Fire2"))
        {
            punchIsPressed = false;
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
                canJump = true;
                animatorWalk.SetBool("canJumpBool", true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8 && collision.collider == collision.transform.GetComponent<ObjectCollisionController>().canJumpCollider) //8 == "Floor" layer
        {
            canJump = false;
            animatorWalk.SetBool("canJumpBool", false);
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

    IEnumerator Punch() 
    {   
        punchDamage = 10; //Resets punch damage from previous punch
        float waitTime = punchTime / 5; //How long it will wait between incrementing damage
        int damageToAdd = punchDamageCap / 8; //Divided by 8 because base damage is already half of cap (so / 2) and we want 4 increments (so / 4) which is just / 8

        isPunching = true;
        horizontalMovementSpeed = 2;
        animatorWalk.SetBool("isPunching", true);
        initialAnimSpeed = 1;

        //Variable Damage System
        yield return new WaitForSeconds(0.06f); //First wait, doesnt add to punch increment so standard punch is differenciated from charge punch
        yield return new WaitForSeconds(waitTime); //Second wait + slows punch to show that it is a charge punch
        gameObject.transform.GetComponent<Animator>().speed -= punchTime * 0.9f; //Slows current animation, 0.375 is a constant b/c the default punchTime value was 0.3 when the anim speed was 0.8
        punchDamage += damageToAdd;
        horizontalMovementSpeed = 1;
        yield return new WaitForSeconds(waitTime); //Third wait
        gameObject.transform.GetComponent<Animator>().speed -= punchTime * 0.5f; //Slows current animation, 0.375 is a constant b/c the default punchTime value was 0.3 when the anim speed was 0.8
        punchDamage += damageToAdd;
        yield return new WaitForSeconds(waitTime); //Fourth wait
        punchDamage += damageToAdd;
        yield return new WaitForSeconds(waitTime); //Fifth wait. punchTime / 5 = 5 waits = 5 increments (technically 4 since the first wait doesnt lead to an increment)
        punchDamage += damageToAdd;

        animatorWalk.SetBool("isPunching", false);
        horizontalMovementSpeed = 4;
        isPunching = false;
        gameObject.transform.GetComponent<Animator>().speed = initialAnimSpeed;
    }

    public void FixedUpdate()
    {
        if(spamPunchTimerInt > 0)
        {
            spamPunchTimerInt -= 1;
        }
        else
        {
            animatorWalk.SetBool("canStopPunching", true);
        }
    }
}
