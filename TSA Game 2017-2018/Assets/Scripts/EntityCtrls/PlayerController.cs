using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameController gameControllerScript;

    //Stats
    public int minHealth = 0; //The health in which the player should be KO'd
    public int maxHealth = 30; //The maximum health the player can have
    public int health; //The player's health

    public float blockMeter; //Decreases rapidly when blocking, if hits 0 block drops
    public int minBlock; //The minimum the block meter can be at (when hit, block drops)
    public int maxBlock; //The maximum the block meter can be at
    public float blockRemoveAmt; //How much to remove from the block meter every frame that the player is blocking
    public float blockAddAmt; //How much to add to the block meter every frame that the player is not blocking

    public int horizontalKnockbackStrength; //How much to knockback an enemy upon punching them on the x axis
    public int verticalKnockbackStrength; //How much to knockback an enemy upon punching them on the y axis

    public float downPunchTime; //How long the player has when pressing punch to down punch (higher -> easier to land down punch)

    public float invincibiltiyFrameTime; //How long the player should stay invincible for after taking damage (ex. 0.75 = 3/4ths of a second; preferably make them divisible by 5)
    public float reviveInvincibilityFrameTimer; //How long the player is invincible for after coming back from being KOd

    public int vendorCredits = 1; //How many "credits" the player has to buy things with
    public int credits; //How much money the player has to spend on items/things at the store (not implemented as of States version)

    //Movement Variables    
    public float horizontalMovementSpeed;
    public float verticalMovementSpeed;
    public float jumpSpeed;
    public bool canJump;
    public bool movementKeyBeingPressed;
    public GameObject fastFallStarObj; //The star indicator that apears when the player is fast falling
    public Color fastFallColor; //Used to make the player slightly darker when fast falling
    public Color originalColor; //The player's original color. Is reset to this fater fast falling

    public float punchTime = 0.8f; //Time a punch stays displayed; DONT MESS WITH! Complex punch system to allow charge punching to work
    public float crouchPunchTime = 0.5f; //Time a crouch punch stays displayed

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
    public int sideFacing = 3; //Topdown: 1 = up/back, 2 = left, 3 = down/front, 4 = right; Sidescroller: 4-> right, 2-> left, 3-> idle
    public int previousSideFacing = 4; //Same as side facing but without 3 for idle; used in many animation transitions to know which side was being faced before going idle

    public Vector2 colliderOriginalSize;
    public Vector2 colliderOriginalOffset;

    public float fastFallStarTime; //How long the fast fall star stays for before dissapearing

    //Attack Variables
    public int standardPunchDamage = 10; //punchDamage is reset to this at the start of every punch
    public int punchDamage; //Changes according to how long you hold it. Default is 10
    public int punchDamageCap = 20; //Used for variable charge damage system

    public int spamPunchTimerInt; //Used to keep animation from changing off punch if spamming
    public bool punchIsPressed;
    public float initialAnimSpeed;

    public bool isBlocking;
    public bool isInInvincibilityFrame;

    //Down Punch Variables
    public GameObject downPunchColliderObj;
    public bool isDownPunching;
    public bool canDownPunch;
    public bool antiSpamBool;

    public IEnumerator downPunchTimerCoroutine;
    public IEnumerator antiSpamDownPunchCoroutine;

    public bool canMove;
    public bool isTalkingToNPC;

    //NOTE: Most actual punch detection is done in PunchCollideController on the punch child obj

    //Animator
    Animator animatorWalk;

    //CONTROLLER DOCUMENTATION: dInput: 0 (Fire0) = A, 1 (Fire1) = B, 2 (Fire2) = X, 3 (Fire3) = Y, 6 = start, 7 = start; xInput: 

    public float xAxisFloat; //if -1, dpad left, if 1 dpad right, 0 = idle
    public bool leftPressed; //true if xAxis -1
    public bool rightPressed; //true if xAxis 1
    public float yAxisFloat; //if -1, dpad down, if 1 dpad up, 0 = idle
    public bool upPressed; //true if yAxis 1
    public bool downPressed; //true if yAxis -1

    public bool isAControllerConnected; //ControllerConnected bool from GameController. Runs controller only code

    //KO Amount
    public bool isBeingKOd;
    public float koTimer; //How long the player has to mash buttons to get back up; gets lower each time
    public List<float> koTimerList; //The list with which the koTimer scales
    public int timesKOd; //How many times the player has been KO'd
    public bool canRunDeathJingle;
    public int mashAmount; //How much the player needs to mash to get back up; gets higher each time
    public int maxMashAmount;
    public List<int> mashAmountList;

    public Vector2 topDownColliderOffset;
    public Vector2 topDownColliderSize;
    public Vector2 sideScrollColliderOffset;
    public Vector2 sideScrollColliderSize;

    //SFX
    public AudioSource audioSource;

    public AudioClip gotPunched1;
    public AudioClip gotPunched2;
    public AudioClip gotPunched3;
    public AudioClip gotPunched4;

    public AudioClip normalPunch;
    public AudioClip jump;
    public AudioClip downPunch;

    public AudioClip crateSwitch;

    public AudioClip deathJingle;

    //GCode
    public bool GCodeEnabled; //If true, allows GCode
    public string inputCode; //Controls the codes; U = up, D = down, L = left, R = right

    private string noClipString = "ULDRU"; //What the user needs to input to noclip. Inputting again disables it
    public bool noClipEnabled; 

    private void Awake()
    {
        gameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gameControllerScript.playerObj = gameObject.transform.parent.gameObject;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Use this for initialization
    void Start()
    {
        //downPunchTimerVar = downPunchTimer();
        //breakOutOfDownPunchAnim = StopDownPunchAnim();
        downPunchTimerCoroutine = DownPunchTimer();
        antiSpamDownPunchCoroutine = DownPunchAntiSpam();

        colliderOriginalSize = sideScrollColliderSize; //Saves original size and offset of box collider to reset to when un-crouching & switching views
        colliderOriginalOffset = sideScrollColliderOffset; //^^^

        health = maxHealth; //Starts player at full health
        gameControllerScript.updateHealthSlider(minHealth, maxHealth, health); //Updates health slider at top left

        //finds walk controller animator
        animatorWalk = this.GetComponent<Animator>();
        //Written by Mary!!! ^^^

        sideFacing = 3;
        previousSideFacing = 4;

        punchDamage = standardPunchDamage;

        //KO Variables Setup
        mashAmountList.Add(60);
        mashAmountList.Add(90);
        mashAmountList.Add(130);
        mashAmountList.Add(180); //Preety sure thats impossible
        koTimerList.Add(25);
        koTimerList.Add(20);
        koTimerList.Add(15);
        koTimerList.Add(10); //Preety sure thats impossible

        originalColor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if(GCodeEnabled)
        {
            //GCode String Filler
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                inputCode += "U";
                GCodeUpdater();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                inputCode += "D";
                GCodeUpdater();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                inputCode += "L";
                GCodeUpdater();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                inputCode += "R";
                GCodeUpdater();
            }
        }

        transform.parent.GetChild(1).position = transform.position;
        if ((Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Fire0")) && transform.GetComponent<Rigidbody2D>().velocity.y == 0 && !isBeingKOd)
        {
            if (blockMeter > (maxBlock / 4))
            {
                isBlocking = true;
                animatorWalk.SetBool("isBlocking", true);
                horizontalMovementSpeed = 0;
                transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }
        if ((Input.GetKeyUp(KeyCode.L) || Input.GetButtonUp("Fire0")) && isBlocking == true)
        {
            isBlocking = false;
            animatorWalk.SetBool("isBlocking", false);
        }

        if (isPunching == false && (transform.GetComponent<Animator>().speed != 1 || horizontalMovementSpeed != 4))
        {
            transform.GetComponent<Animator>().speed = 1;
            horizontalMovementSpeed = 4;
        }

        if (isBlocking == true)
        {
            if (blockMeter > minBlock + blockRemoveAmt)
            {
                blockMeter -= blockRemoveAmt; //Degrades block meter
                if (horizontalMovementSpeed != 0)
                    horizontalMovementSpeed = 0; //Slows player when blocking
                gameControllerScript.updateBlockSlider(minBlock, maxBlock, (int)blockMeter); //Updates the block meter at the top left
            }
            else
            {
                isBlocking = false;
                animatorWalk.SetBool("isBlocking", false);
            }
        }
        else
        {
            if (blockMeter < maxBlock)
            {
                blockMeter += blockAddAmt; //Regenerates block meter
                gameControllerScript.updateBlockSlider(minBlock, maxBlock, (int)blockMeter); //Updates the block meter at the top left
            }
        }

        if (isAControllerConnected == true && gameControllerScript.currentView == 2)
        {
            xAxisFloat = Input.GetAxis("Horizontal");
            yAxisFloat = Input.GetAxis("Vertical");
            if (xAxisFloat > 0.8)
            {
                leftPressed = false;
                rightPressed = true;
            }
            else
            {
                if (xAxisFloat < -0.8)
                {
                    leftPressed = true;
                    rightPressed = false;
                }
            }
            if (yAxisFloat > 0.8)
            {
                upPressed = true;
                downPressed = false;
            }
            else
            {
                if (yAxisFloat < -0.8)
                {
                    upPressed = false;
                    downPressed = true;
                }
            }
        }

        if (isAControllerConnected == true && gameControllerScript.currentView == 1)
        {
            xAxisFloat = Input.GetAxis("Horizontal");
            yAxisFloat = Input.GetAxis("Vertical");
            if (xAxisFloat > 0.8)
            {
                //leftPressed = false;
                rightPressed = true;
            }
            else
            {
                if (xAxisFloat < -0.8)
                {
                    leftPressed = true;
                    //rightPressed = false;
                }
                else
                {
                    //leftPressed = false;
                    //rightPressed = false;
                }
            }
            if (yAxisFloat > 0.8)
            {
                upPressed = true;
                //downPressed = false;
            }
            else
            {
                if (yAxisFloat < -0.8)
                {
                    //upPressed = false;
                    downPressed = true;
                }
                else
                {
                    // upPressed = false;
                    //downPressed = false;
                }
            }
        }

        animatorWalk.SetInteger("sideFacingInt", sideFacing);
        animatorWalk.SetInteger("previousSideFacing", previousSideFacing);
        if (transform.GetComponent<Rigidbody2D>().velocity.x > 0 || transform.GetComponent<Rigidbody2D>().velocity.y > 0 || transform.GetComponent<Rigidbody2D>().velocity.x < 0 || transform.GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            isIdle = false;
            transform.GetComponent<SpriteRenderer>().flipX = false;
        }
        animatorWalk.SetInteger("HorizontalSpeedTD", (int)transform.GetComponent<Rigidbody2D>().velocity.x); //Velocity based animations
        animatorWalk.SetInteger("VerticalSpeedTD", (int)transform.GetComponent<Rigidbody2D>().velocity.y); //TD = Topdown variables

        if (gameControllerScript.currentView == 1)
        {
            //Topdown Movement

            //Press down to move
            if (!isTalkingToNPC)
            {
                if (Input.GetKey(KeyCode.D) || (rightPressed == true && xAxisFloat > 0.69))
                {
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalMovementSpeed, transform.GetComponent<Rigidbody2D>().velocity.y);
                    sideFacing = 4;
                }
                if (Input.GetKey(KeyCode.A) || (leftPressed == true && xAxisFloat < -0.69))
                {
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-horizontalMovementSpeed, transform.GetComponent<Rigidbody2D>().velocity.y);
                    sideFacing = 2;
                }
                if (Input.GetKey(KeyCode.W) || (upPressed == true && yAxisFloat > 0.69))
                {
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x, verticalMovementSpeed);
                    sideFacing = 1;
                }
                if (Input.GetKey(KeyCode.S) || (downPressed == true && yAxisFloat < -0.69))
                {
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x, -verticalMovementSpeed);
                    sideFacing = 3;
                }

                //Let go to stop
                if (Input.GetKeyUp(KeyCode.D) || (rightPressed == true && xAxisFloat < 0.69))
                {
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, transform.GetComponent<Rigidbody2D>().velocity.y);
                    isIdle = true;
                    sideFacing = 3;
                    rightPressed = false;
                }
                if (Input.GetKeyUp(KeyCode.A) || (leftPressed == true && xAxisFloat > -0.69))
                {
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, transform.GetComponent<Rigidbody2D>().velocity.y);
                    isIdle = true;
                    sideFacing = 3;
                    leftPressed = false;
                }
                if (Input.GetKeyUp(KeyCode.W) || (upPressed == true && yAxisFloat < 0.69))
                {
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                    isIdle = true;
                    sideFacing = 3;
                    upPressed = false;
                }
                if (Input.GetKeyUp(KeyCode.S) || (downPressed == true && yAxisFloat > -0.69))
                {
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                    isIdle = true;
                    sideFacing = 3;
                    downPressed = false;
                }
            }

            if (!upPressed && !downPressed && !leftPressed && !rightPressed && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) //Controls animations
                animatorWalk.SetBool("isMoving", false);
            else
                animatorWalk.SetBool("isMoving", true);
        }
        if (gameControllerScript.currentView == 2 && !isBeingKOd)
        {
            //Sidescroll Movement

            //Press to move
            if ((Input.GetKey(KeyCode.D) || (rightPressed == true && xAxisFloat > 0.8)) && GetComponent<Rigidbody2D>().velocity.x < 0.005)
            {
                GetComponent<Rigidbody2D>().AddForce(transform.right * horizontalMovementSpeed * 50);
                sideFacing = 4;
                previousSideFacing = 4;
            }
            if ((Input.GetKey(KeyCode.A) || (leftPressed == true && xAxisFloat < -0.8)) && GetComponent<Rigidbody2D>().velocity.x > -0.005)
            {
                GetComponent<Rigidbody2D>().AddForce(-transform.right * horizontalMovementSpeed * 50);
                previousSideFacing = 2;
                sideFacing = 2;
            }

            if (GetComponent<Rigidbody2D>().velocity.x == -8)
                GetComponent<Rigidbody2D>().velocity = new Vector2(-4, GetComponent<Rigidbody2D>().velocity.y);
            if (GetComponent<Rigidbody2D>().velocity.x == 8)
                GetComponent<Rigidbody2D>().velocity = new Vector2(4, GetComponent<Rigidbody2D>().velocity.y);

            if ((Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Fire1")) && letGoOfSpace == false)
            {
                //canDownPunch = true;
                transform.parent.GetChild(1).GetComponent<PlayerDownPunchCollider>().enabled = true;
                animatorWalk.SetBool("canJumpBool", false);
                animatorWalk.SetBool("isBlocking", false);
                jumpheightTimerInt = 0;
                goToMinJump = true;
                audioSource.PlayOneShot(jump);
            }

            if (goToMinJump == true)
            {
                if (jumpheightTimerInt < minJumpHeight && !antiSpamBool)
                {
                    jumpheightTimerInt += 1;
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x, jumpSpeedInt);
                }
                else
                {
                    goToMinJump = false;
                    if (letGoOfSpace == true && transform.GetComponent<Rigidbody2D>().velocity.y >= 0 && isDownPunching == false)
                    {
                        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                    }
                }
            }

            if ((Input.GetKey(KeyCode.K) || (Input.GetButton("Fire1")) && goToMinJump == false && letGoOfSpace == false))
            {
                if (jumpheightTimerInt < maxJumpHeight)
                {
                    jumpheightTimerInt += 1;
                    transform.GetComponent<Rigidbody2D>().AddForce(transform.up * jumpSpeedInt);
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
                    if (jumpheightTimerInt >= minJumpHeight && transform.GetComponent<Rigidbody2D>().velocity.y >= 0 && isDownPunching == false)
                    {
                        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x, 0);
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
                    if (jumpheightTimerInt >= minJumpHeight && transform.GetComponent<Rigidbody2D>().velocity.y >= 0 && isDownPunching == false)
                    {
                        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.GetComponent<Rigidbody2D>().velocity.x, 0);
                    }
                }
            }

            //Fast Fall
            if ((Input.GetKey(KeyCode.S) || (downPressed == true && yAxisFloat < -0.8)) && !canJump)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, -jumpSpeedInt * 1.5f);
                if (fastFallStarObj.activeInHierarchy == false)
                {
                    fastFallStarObj.SetActive(true);
                    GetComponent<SpriteRenderer>().color = fastFallColor;
                    if (previousSideFacing == 2) //If player is facing left
                        fastFallStarObj.transform.localPosition = new Vector3(0.7f, fastFallStarObj.transform.localPosition.y, fastFallStarObj.transform.localPosition.z);
                    if (previousSideFacing == 4) //If player is facing right
                        fastFallStarObj.transform.localPosition = new Vector3(-0.7f, fastFallStarObj.transform.localPosition.y, fastFallStarObj.transform.localPosition.z);
                    StartCoroutine(DisableFastFallStarTimer()); //Starts a timer to disable the fast fall star after a certain amount of time
                }
            }

            //Crouch
            if ((Input.GetKey(KeyCode.S) || (downPressed == true && yAxisFloat < -0.8)) && canJump && !isCrouching && GetComponent<Rigidbody2D>().velocity.x == 0) //If press down on dpad && is grounded && was moving but now is not
            {
                isCrouching = true;
                isPunching = false;
                horizontalMovementSpeed = 1;
                animatorWalk.SetBool("isCrouching", true);
                transform.GetComponent<BoxCollider2D>().offset = new Vector2(-0.185318f, -0.4041151f); //2.562298 0.01893747
                transform.GetComponent<BoxCollider2D>().size = new Vector2(1.453499f, 1.716193f);
            }

            //Let go to stop
            if (Input.GetKeyUp(KeyCode.D) || (rightPressed == true && xAxisFloat < 0.8))
            {
                transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, transform.GetComponent<Rigidbody2D>().velocity.y);
                if (isCrouching == false)
                {
                    isIdle = true;
                    sideFacing = 3;
                    rightPressed = false;
                }
            }
            if (Input.GetKeyUp(KeyCode.A) || (leftPressed == true && xAxisFloat > -0.8))
            {
                transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, transform.GetComponent<Rigidbody2D>().velocity.y);
                if (isCrouching == false)
                {
                    isIdle = true;
                    sideFacing = 3;
                    leftPressed = false;
                }
            }
            if (isCrouching && (Input.GetKeyUp(KeyCode.S) || (downPressed == true && yAxisFloat > -0.8))) //Breaks out of crouch if S is let go or dpad down is let go or is moving
            {
                isCrouching = false;
                horizontalMovementSpeed = 4;
                animatorWalk.SetBool("isCrouching", false); //Sets animator isCrouching bool to false
                animatorWalk.SetBool("isPunching", false);
                isIdle = true;
                sideFacing = 3; //Side facing = forward/toward camera
                transform.GetComponent<BoxCollider2D>().size = colliderOriginalSize; //Resets main boxcollider's size and offset to match character forward model
                transform.GetComponent<BoxCollider2D>().offset = colliderOriginalOffset;
                downPressed = false;
            }
        }

        //Attack Code
        if ((Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire3")) && isPunching == false)
        {
            if (canJump) //If is grounded, do regular punch
            {
                if (isCrouching == false)
                {
                    //StartCoroutine(Punch()); //Initiates punch/charge punch coroutine
                    isPunching = true;
                    animatorWalk.SetBool("canStopPunching", false);
                    horizontalMovementSpeed = 2;
                    animatorWalk.SetBool("isPunching", true);
                    animatorWalk.SetBool("isBlocking", false);
                    initialAnimSpeed = 1;
                }
                else //If is crouching when punch is activated, do crouch punch instead of regular punch/charge punch
                {
                    StartCoroutine(CrouchPunch());
                }
            }
            else //If mid jump, do down punch (if allowed by canDownPunch)
            {
                if(!antiSpamBool)
                {
                    canDownPunch = false;
                    downPunchColliderObj.SetActive(true);
                }
                else
                StopCoroutine(downPunchTimerCoroutine);
                downPunchTimerCoroutine = DownPunchTimer();
                StartCoroutine(downPunchTimerCoroutine);
                StopCoroutine(antiSpamDownPunchCoroutine);
                antiSpamDownPunchCoroutine = DownPunchAntiSpam();
                StartCoroutine(antiSpamDownPunchCoroutine);
            }
        }
        if ((Input.GetButtonDown("Fire3") || (Input.GetKeyDown(KeyCode.J)) && isPunching == true)) //Spam punch code
        {
            spamPunchTimerInt = 20;
            animatorWalk.SetBool("canStopPunching", false);
        }

        if ((Input.GetKeyUp(KeyCode.J) || Input.GetButtonUp("Fire3")) && spamPunchTimerInt < 20 && isCrouching == false && isPunching == true)
        {
            //StopAllCoroutines(); //Breaks out of punch / charge punch
            Color tempColor = transform.GetComponent<SpriteRenderer>().color;
            tempColor.a = 255;
            transform.GetComponent<SpriteRenderer>().color = tempColor;
            animatorWalk.SetBool("isPunching", false);
            horizontalMovementSpeed = 4;
            isPunching = false;
            transform.GetComponent<Animator>().speed = initialAnimSpeed;
        }

        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire3"))
        {
            punchIsPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.J) || Input.GetButtonUp("Fire3"))
        {
            punchIsPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire0") || Input.GetButtonDown("Fire1") && isBeingKOd)
        {
            mashAmount++;
        }

        if (GetComponent<SpriteRenderer>().color.a <= 0 && !isInInvincibilityFrame)
        {
            Color tempColor = GetComponent<SpriteRenderer>().color;
            tempColor.a = 255;
            GetComponent<SpriteRenderer>().color = tempColor;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8 && collision.transform.GetComponent<ObjectCollisionController>() != null && collision.collider == collision.transform.GetComponent<ObjectCollisionController>().canJumpCollider) //8 == "Floor" layer
        {
            if (letGoOfSpace == true)
            {
                letGoOfSpace = false;
            }
            if (isIdle == false)
            {
                isIdle = true;
            }
            if (!canJump || !animatorWalk.GetBool("canJumpBool"))
            {
                canJump = true;
                animatorWalk.SetBool("canJumpBool", true);
                animatorWalk.SetBool("isDownPunching", false);

                //Resets fast fall
                if (fastFallStarObj.activeInHierarchy == true)
                {
                    fastFallStarObj.SetActive(false);
                    GetComponent<SpriteRenderer>().color = originalColor;
                    StopCoroutine(DisableFastFallStarTimer());
                }
                //effectiveDownPunch = false;
                //isDownPunching = false;
            }
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "DownPunchable" && !effectiveDownPunch)
            effectiveDownPunch = true;
    }*/

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8 && collision.transform.GetComponent<ObjectCollisionController>() != null && collision.collider == collision.transform.GetComponent<ObjectCollisionController>().canJumpCollider) //8 == "Floor" layer
        {
            canJump = false;
            //canDownPunch = true;
            transform.parent.GetChild(1).GetComponent<PlayerDownPunchCollider>().enabled = true;
            animatorWalk.SetBool("canJumpBool", false);
            animatorWalk.SetBool("isBlocking", false);
            animatorWalk.SetBool("canStopPunching", true);
        }
    }

    IEnumerator Punch()
    {
        punchDamage = standardPunchDamage; //Resets punch damage from previous punch
        float waitTime = punchTime / 3; //How long it will wait between incrementing damage
        int damageToAdd = punchDamageCap / 8; //Divided by 8 because base damage is already half of cap (so / 2) and we want 4 increments (so / 4) which is just / 8

        isPunching = true;
        horizontalMovementSpeed = 2;
        animatorWalk.SetBool("isPunching", true);
        animatorWalk.SetBool("isBlocking", false);
        initialAnimSpeed = 1;

        //Variable Damage System
        yield return new WaitForSeconds(0.05f); //First wait, doesnt add to punch increment so standard punch is differenciated from charge punch
        transform.GetComponent<Animator>().speed -= punchTime * 0.2f; //Slows current animation
        yield return new WaitForSeconds(waitTime); //Second wait + slows punch to show that it is a charge punch

        if (punchIsPressed == false)
        {
            StopAllCoroutines(); //Breaks out of punch / charge punch
            Color tempColor = transform.GetComponent<SpriteRenderer>().color;
            tempColor.a = 255;
            transform.GetComponent<SpriteRenderer>().color = tempColor;
            animatorWalk.SetBool("isPunching", false);
            animatorWalk.SetBool("canStopPunching", true);
            horizontalMovementSpeed = 4;
            isPunching = false;
            transform.GetComponent<Animator>().speed = initialAnimSpeed;
        }

        transform.GetComponent<Animator>().speed -= punchTime * 1f; //Slows current animation
        punchDamage += damageToAdd;
        horizontalMovementSpeed = 1;
        yield return new WaitForSeconds(waitTime); //Third wait
        transform.GetComponent<Animator>().speed -= punchTime * 0.4f; //Slows current animation
        punchDamage += damageToAdd;
        yield return new WaitForSeconds(waitTime); //Fourth wait
        punchDamage += damageToAdd;
        yield return new WaitForSeconds(waitTime); //Fifth wait. punchTime / 5 = 5 waits = 5 increments (technically 4 since the first wait doesnt lead to an increment)
        punchDamage += damageToAdd;

        animatorWalk.SetBool("isPunching", false);
        animatorWalk.SetBool("canStopPunching", true);
        horizontalMovementSpeed = 4;
        isPunching = false;
        transform.GetComponent<Animator>().speed = initialAnimSpeed;
    }

    IEnumerator CrouchPunch()
    {
        isPunching = true;
        animatorWalk.SetBool("canStopPunching", false);
        animatorWalk.SetBool("isPunching", true); //Tells animator to start punching anim
        animatorWalk.SetBool("isBlocking", false);
        punchDamage = standardPunchDamage; //Resets punch damage from previous punch

        yield return new WaitForSeconds(crouchPunchTime); //Waits crouchPunchTime

        animatorWalk.SetBool("isPunching", false); //Tells animator to stop punching anim
        isPunching = false;
    }

    public void FixedUpdate()
    {
        if (spamPunchTimerInt > 0)
            spamPunchTimerInt -= 1;
        else
        {
            animatorWalk.SetBool("canStopPunching", true);
            animatorWalk.SetBool("isPunching", false);
        }

        if (isBeingKOd)
            KoTimer();
    }

    public void TakeDamage(int dmgToTake) //Subtracts dmgToTake from health, triggers KO system if needed
    {
        if (!isBlocking && !isInInvincibilityFrame && !isBeingKOd) //If is not blocking
        {
            health -= dmgToTake;
            gameControllerScript.updateHealthSlider(minHealth, maxHealth, health); //Updates health slider at top left
            if (health <= minHealth) //If player's health is below minimum health (default is 0), start KO system
                SetupKO();
            StopAllCoroutines();
            StartCoroutine(InvincibilityFramesTimer());

            //Got Punched SFX
            int tempSoundRand = Random.Range(1, 5);
            switch (tempSoundRand)
            {
                case 1:
                    audioSource.PlayOneShot(gotPunched1);
                    break;
                case 2:
                    audioSource.PlayOneShot(gotPunched2);
                    break;
                case 3:
                    audioSource.PlayOneShot(gotPunched3);
                    break;
                case 4:
                    audioSource.PlayOneShot(gotPunched4);
                    break;
            }
        }
    }

    public void SetupKO()
    {
        animatorWalk.SetBool("isDowned", true);
        isBeingKOd = true;
        health = minHealth;
        gameControllerScript.updateHealthSlider(minHealth, maxHealth, health);
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        gameControllerScript.koSlider.transform.parent.gameObject.SetActive(true);
        timesKOd++;
        mashAmount = 0;
        if (timesKOd != koTimerList.Count)
        {
            koTimer = koTimerList[timesKOd - 1];
            maxMashAmount = mashAmountList[timesKOd - 1];
        }
        gameControllerScript.koSlider.GetComponent<Slider>().maxValue = maxMashAmount;
    }

    void KoTimer()
    {
        if (koTimer > 0 && mashAmount < maxMashAmount)
        {
            koTimer -= 0.02f;
            gameControllerScript.koTimerText.transform.GetComponent<Text>().text = koTimer.ToString("F2");
            gameControllerScript.koSlider.GetComponent<Slider>().value = mashAmount;
        }
        else
        {
            if (koTimer <= 0.03 && canRunDeathJingle)
            {
                koTimer = 0;
                timesKOd = 0;
                gameControllerScript.levelMusicObjs[gameControllerScript.levelID].GetComponent<AudioSource>().Pause();
                audioSource.PlayOneShot(deathJingle);
                IEnumerator deathJingleIEnum = DeathJingleTimer();
                StartCoroutine(deathJingleIEnum);
                canRunDeathJingle = false;
            }
            if (mashAmount >= maxMashAmount)
            {
                health = maxHealth;
                gameControllerScript.updateHealthSlider(minHealth, maxHealth, health);
                isBeingKOd = false; //Re-enables basic functions (ex. movement)
                animatorWalk.SetBool("isDowned", false); //Disables KO animation
                gameControllerScript.koSlider.transform.parent.gameObject.SetActive(false); //Disables the KO slider ui
                StartCoroutine(ReviveInvincibilityTimer()); //Makes character invincible so he doesnt immediately die again
            }
        }
    }

    IEnumerator KoTimerRunner()
    {
        if (isBeingKOd)
        {
            yield return new WaitForSeconds(0.2f);
            KoTimer();
        }
    }

    public IEnumerator DownPunchTimer()
    {
        animatorWalk.SetBool("isDownPunching", true);
        yield return new WaitForSeconds(downPunchTime);
        isDownPunching = false;
        animatorWalk.SetBool("isDownPunching", false);
    }

    public IEnumerator DownPunchAntiSpam()
    {
        antiSpamBool = true;
        yield return new WaitForSeconds(0.5f);
        antiSpamBool = false;
    }

    public IEnumerator InvincibilityFramesTimer()
    {
        isInInvincibilityFrame = true;
        float time = invincibiltiyFrameTime / 5;
        Color backupColor = transform.GetComponent<SpriteRenderer>().color;
        Color tempColor = transform.GetComponent<SpriteRenderer>().color;
        tempColor.a = 0.35f;
        transform.GetComponent<SpriteRenderer>().color = tempColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = backupColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = tempColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = backupColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = tempColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = backupColor;
        isInInvincibilityFrame = false;
    }

    public IEnumerator ReviveInvincibilityTimer() //Used after the player comes back from being KOd
    {
        isInInvincibilityFrame = true;
        float time = reviveInvincibilityFrameTimer / 10;
        Color backupColor = transform.GetComponent<SpriteRenderer>().color;
        Color tempColor = transform.GetComponent<SpriteRenderer>().color;
        tempColor.a = 0.35f;
        transform.GetComponent<SpriteRenderer>().color = tempColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = backupColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = tempColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = backupColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = tempColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = tempColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = backupColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = tempColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = backupColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = tempColor;
        yield return new WaitForSeconds(time);
        transform.GetComponent<SpriteRenderer>().color = backupColor;
        isInInvincibilityFrame = false;
    }

    public void DownPunch()
    {
        isDownPunching = true;
        audioSource.PlayOneShot(downPunch);
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpSpeedInt * 1.5f);
        if (GameObject.FindGameObjectsWithTag("Pow").Length == 0 && GetComponent<Rigidbody2D>().velocity.y > jumpSpeedInt * 1.45f)
            StartCoroutine(gameControllerScript.SpawnPow(transform.position));
    }

    IEnumerator DeathJingleTimer()
    {
        yield return new WaitForSeconds(4.5f);
        ResetAfterKO();
    }

    void ResetAfterKO()
    {
        health = maxHealth;
        gameControllerScript.updateHealthSlider(minHealth, maxHealth, health);
        isBeingKOd = false; //Re-enables basic functions (ex. movement)
        animatorWalk.SetBool("isDowned", false); //Disables KO animation
        gameControllerScript.koSlider.transform.parent.gameObject.SetActive(false); //Disables the KO slider ui
        gameControllerScript.changeToTopdown(true);
        /*gameObject.transform.position = gameControllerScript.levelSpawnpoints[gameControllerScript.levelID].transform.position; //Resets player to start of level
        gameControllerScript.mainCameraObj.transform.position = gameObject.transform.position;
        gameControllerScript.levelParallaxObjs[gameControllerScript.levelID].transform.position = new Vector3(gameObject.transform.position.x, gameControllerScript.levelParallaxObjs[gameControllerScript.levelID].transform.position.y, gameControllerScript.levelParallaxObjs[gameControllerScript.levelID].transform.position.z);*/
        gameControllerScript.mainCameraObj.GetComponent<CameraController>().desiredPostion = new Vector3(transform.position.x, transform.position.y, -10);
        gameControllerScript.levelMusicObjs[gameControllerScript.levelID].GetComponent<AudioSource>().UnPause();       
        canRunDeathJingle = true;

    }

    IEnumerator DisableFastFallStarTimer() //Disables the fast fall star after it being active for fastFallStarTime in seconds
    {
        yield return new WaitForSeconds(fastFallStarTime);
        fastFallStarObj.SetActive(false);
    }

    void GCodeUpdater()
    {
        //String lenth controller
        if (inputCode.Length > 5)
        {
            inputCode = inputCode.Substring(1, 5);
        }

        if(inputCode == noClipString)
        {
            inputCode = "";
            noClipEnabled = !noClipEnabled;
            GetComponent<BoxCollider2D>().enabled = !noClipEnabled;
        }
    }
}
