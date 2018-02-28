using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

    public int enemyType; //0 = blonde enemy, 1 = white haired enemy, 2 = boss
    public GameObject playerObj; //The player

    //Stats
    public int health; //Enemy's current health
    public int minHealth; //Enemy's minimum possible health (when this is hit, enemy is dead)
    public int maxHealth = 10; //Enemy's maxmimum health (start health)
    public int blockChance; //From 0-100, a number is randomised in that same range and if is lower than block chance then enemy manages to block in time. Ex: blockChance = 50 = 50% chance. If 35 is generated then block is successful, 68 is not
    public float deathFadeOutSpeed; //How fast it should fade upon death
    public int knockbackStregth; //How much this enemy knocks back an entity (ex. player) when punching them

    //Movement Variables
    public float horizontalMovementSpeed;
    public int verticalMovementSpeed;

    //UI Objects
    public GameObject healthSlider; //The health bar above the enemy

    //Combat
    public int damage; //How much damage to deal when punching
    public int detectionRange; //How close a player needs to be for the enemy to agro
    public float attackRange; //How close a player needs to be for the enemy to try and attack
    public bool playerInRange;

    public int agressiveTimer; //Counts down, if hits 0 enemy goes agressive. Resets if player attacks
    public bool runAgressiveTimer;
    public int agressiveTimerReset; //agressiveTimer is reset to this each time it needs to tick again
    public int agressiveRandomiser; //Random number between 1-3; 1 = charge and attack player, 2 = fall back, 3 = block and hold ground

    public int timeToChasePlayer; //How long the enemy should try to chace the player if the agroRandom picks 1; IN SECONDS, ChasePlayerTimer coroutine
    public bool closeEnoughToPunch; //Stops chasing if true
    public int timerToFallBack; //How long the enemy should walk backwards for; IN SECONDS, FallBackTimer coroutine

    public Animator anim; //Animator on this object
    public Rigidbody2D body; //Rigidbody2D on this object

    public bool isBeingPunched; //Stops any calls that change velocity to allow for knockback
    public float punchTime; //How long to wait for the punch to end
    public bool isPunching; //If the enemy is punching
    public BoxCollider2D punchCollider; //The collider that turns on when the enemy is punching
    public GameObject punchColliderObj; //The object that holds the punch collider

    public bool isBlocking; //If the enemy is blocking
    public int blockTimer; //Counts down to disable block

    //Misc
    public bool isDead; //Controls fading out
    public int randomBlockChance;
    public bool triedBlocking;

    public GameObject levelExitDoor; //Is enabled when the enemy dies
    public bool enableDoorOnDeath;


    //Use this for initialization
    void Start ()
    {
        anim = gameObject.transform.GetComponent<Animator>();
        body = gameObject.transform.GetComponent<Rigidbody2D>();
        health = maxHealth; //Start at max health
        healthSlider.transform.GetComponent<Slider>().minValue = minHealth; //Set health bar above to match health stats
        healthSlider.transform.GetComponent<Slider>().maxValue = maxHealth; //^^^
        healthSlider.transform.GetComponent<Slider>().value = health; //^^^
    }
	
	// Update is called once per frame
	void Update () {
        transform.parent.GetChild(1).position = transform.position; //Makes sure NoPushCollider follows enemy
        transform.parent.GetChild(2).position = transform.position; //Makes sure DownPunchCollider
        if (!playerInRange) //If a player is in the range for agro
        {
            if(Vector3.Distance(playerObj.transform.position, gameObject.transform.position) <= detectionRange)
                playerInRange = true;
        }
        if(playerInRange)
        {
            if (Vector3.Distance(playerObj.transform.position, gameObject.transform.position) > detectionRange)
                playerInRange = false;
        }
        if(agressiveRandomiser == 2 && Vector3.Distance(gameObject.transform.position, playerObj.transform.position) + 2 > attackRange) //Stops enemy from running away and passing agro range
        {
            runAgressiveTimer = false; //Stops timer from running until activity is done
            agressiveTimer = agressiveTimerReset; //Resets timer
            agressiveRandomiser = 1;
        }
        if(isBlocking)
        {
            if(blockTimer > 0)
                blockTimer--;
            else
            {
                isBlocking = false; //Stops blocking checks and anti-damage
                anim.SetInteger("isBlocking", 0); //Stops blocking animation
                agressiveTimer = agressiveTimerReset;
                runAgressiveTimer = true; 
            }
        }

        if (playerInRange) //If the player is in range
        {
            if (agressiveTimer <= 0 && runAgressiveTimer == true)
            {
                StopAllCoroutines();
                closeEnoughToPunch = false; //Resets bool so if the randomiser picks 1
                agressiveRandomiser = Random.Range(1, 3); //If 1, charge player, 2 fall back; 1-3 b/c max is exclusive
                if (agressiveRandomiser == 1) //If it picked 1
                {
                    runAgressiveTimer = false; //Stops timer from running until activity is done
                    agressiveTimer = agressiveTimerReset; //Resets timer
                    StartCoroutine(ChasePlayerTimer(Random.Range(1, 4))); //Starts the coroutine that will stop the enemy from chasing the player if he doesnt catch him within a certain amount of time (int passed, seconds)
                }
                if (agressiveRandomiser == 2) //If it picked 2
                {
                    runAgressiveTimer = false; //Stops timer from running until activity is done
                    agressiveTimer = agressiveTimerReset; //Resets timer
                    StartCoroutine(FallBackTimer(Random.Range(2, 5))); //Starts the coroutine that stops the enemy walking back after a certain amount of time (int passed, seconds)

                    if (playerObj.transform.position.x - gameObject.transform.position.x > 0) //If player is to the left of enemy
                    {
                        body.velocity = new Vector2(-horizontalMovementSpeed, body.velocity.y); //Move right
                        anim.SetInteger("sideMoving", 2); //Sets SideMoving in in animator to -2 (right inverse)
                    }
                    else //If player is to the right of enemy
                    {
                        body.velocity = new Vector2(horizontalMovementSpeed, body.velocity.y); //Move left
                        anim.SetInteger("sideMoving", -2); //Sets SideMoving in in animator to 2 (left inverse)
                    }
                }
            }
            if(agressiveTimer > 0 && runAgressiveTimer == true)
            {
                agressiveTimer--; //Runs agressive timer
            }

            if(playerObj.transform.GetComponent<PlayerController>().isPunching == true && Vector3.Distance(gameObject.transform.position, playerObj.transform.position) < 4 && isBeingPunched == false && triedBlocking == false) //If the player is about to punch this enemy
            {
                randomBlockChance = Random.Range(0, 101); //Actually goes to 100, max is exclusive
                if (randomBlockChance <= blockChance) //If roll is lower than the number needed to successfuly block
                    Block();
                triedBlocking = true;
            }
        }

        switch(enemyType) //Punch Controller
        {
            case 0: //If is blonde boxer
                {
                    if (isPunching && (GetComponent<SpriteRenderer>().sprite.name == "BlondePunchLeft_4" || GetComponent<SpriteRenderer>().sprite.name == "BlondePunchRight_4")) //If is punching and is on the punch sprite where the collider should turn on
                    {
                        punchCollider.enabled = true; //Enables the punch collider when the enemy's punch is on (first frame)
                    }
                    if (isPunching && (GetComponent<SpriteRenderer>().sprite.name == "BlondePunchLeft_7" || GetComponent<SpriteRenderer>().sprite.name == "BlondePunchRight_7")) //If is punching and is on the punch sprite where the collider should turn off
                    {
                        punchCollider.enabled = false; //Disables the punch collider when the enemy is ending his punch (NOTE: Technically 1 frame off as in frame 7 he is still punching, but it resets back to idle after this so its the last time to make this check)
                        anim.SetInteger("punchSide", 0); //Stops punching
                        isPunching = false; //These 4 lines reset the agressive timer
                        closeEnoughToPunch = false;
                        runAgressiveTimer = true;
                        agressiveTimer = agressiveTimerReset; //^^^
                    }
                    break;
                }
            case 2: //If is boss enemy
                {
                    if (GetComponent<SpriteRenderer>().sprite.name == "BossPunchLeft") //If is punching and is on the punch sprite where the collider should turn on
                    {
                        punchCollider.enabled = true; //Enables the punch collider when the enemy's punch is on (first frame)
                        isPunching = true;
                    }
                    if (isPunching == true && GetComponent<SpriteRenderer>().sprite.name != "BossPunchLeft") //If is punching and is on the punch sprite where the collider should turn off
                    {
                        punchCollider.enabled = false; //Disables the punch collider when the enemy is ending his punch (NOTE: Technically 1 frame off as in frame 7 he is still punching, but it resets back to idle after this so its the last time to make this check)
                        anim.SetInteger("punchSide", 0); //Stops punching
                        isPunching = false; //These 4 lines reset the agressive timer
                        closeEnoughToPunch = false;
                        runAgressiveTimer = true;
                        agressiveTimer = agressiveTimerReset; //^^^
                    }
                    break;
                }
        }


        //Checks for agro
        if (agressiveRandomiser == 1 && !runAgressiveTimer) //If the randomiser picked 1 and its not time to rerol yet
        {
            //Keeps enemy chasing player even if player switches from being to the left of enemy to being to the right or vice versa
            if(!closeEnoughToPunch && !isBeingPunched)
            {
                if (playerObj.transform.position.x - gameObject.transform.position.x >= 0) //If player is to the left of enemy
                {
                    body.velocity = new Vector2(horizontalMovementSpeed, body.velocity.y); //Move left
                    anim.SetInteger("sideMoving", 1); //Sets SideMoving in in animator to 1 (left)
                }
                else //If player is to the right of enemy
                {
                    body.velocity = new Vector2(-horizontalMovementSpeed, body.velocity.y); //Move right
                    anim.SetInteger("sideMoving", -1); //Sets SideMoving in in animator to -1 (right)
                }
            }

            //Attacks player if close enough
            if (agressiveRandomiser == 1 && Vector3.Distance(playerObj.transform.position, gameObject.transform.position) <= attackRange && closeEnoughToPunch == false) //If is close enough to punch
            {
                StopAllCoroutines(); //Stops the ChasePlayerTimer coroutine from running to show that the player has been "caught" 
                closeEnoughToPunch = true; //Stops chase code from running
                isPunching = true; //Starts punching player
                //body.velocity = new Vector2(0, 0); //Stops enemy movement
                //Punch
                if (playerObj.transform.position.x - gameObject.transform.position.x > 0) //If player is left of enemy
                    anim.SetInteger("punchSide", 1); //Punch left
                else //If not
                    anim.SetInteger("punchSide", -1); //Punch right
            }
        }

        if (agressiveRandomiser == 2) //If it picked 2
        {
            if (playerObj.transform.position.x - gameObject.transform.position.x > 0) //If player is to the left of enemy
            {
                body.velocity = new Vector2(-horizontalMovementSpeed, body.velocity.y); //Move right
                anim.SetInteger("sideMoving", 2); //Sets SideMoving in in animator to -2 (right inverse)
            }
            else //If player is to the right of enemy
            {
                body.velocity = new Vector2(horizontalMovementSpeed, body.velocity.y); //Move left
                anim.SetInteger("sideMoving", -2); //Sets SideMoving in in animator to 2 (left inverse)
            }
        }
            //Death Check
            if (isDead)
                DeathVoid(deathFadeOutSpeed); //Fade out
    }

    public void TakeDamage(int dmgToTake, bool isDownPunch) //Substracts dmgToTake from health and destroys object if at or below 0
    {
        if (!isBlocking)
        {
            health -= dmgToTake;
            if (health <= minHealth)
            {
                isDead = true; //Starts fade out
                healthSlider.transform.GetChild(1).GetChild(0).gameObject.SetActive(false); //Sets fill object on health slider to inactive to prevent small amount of red "health" at the very left
            }
            healthSlider.transform.GetComponent<Slider>().value = health; //Updates health bar above enemy with new health
        }
    }

    public void Jump() //Adds vertical force relative to the vertical movement speed
    {
        if (body.velocity.y == 0 && body.velocity.x != 0)
            body.velocity = new Vector2(body.velocity.x, verticalMovementSpeed); //Jump
    }

    IEnumerator ChasePlayerTimer(int time) //When the enemy is trying to chase the player (through picking 1 in the agroRandom) this timer will wait for TimeToChasePlayer and if ends will re-roll the agroRandom so the enemy doesnt chace for ever
    {
        yield return new WaitForSeconds(time); //Give enemy time to chase player. This coroutine is stopped if he chaches him
        runAgressiveTimer = true; //Rerol
        anim.SetInteger("sideMoving", 0);
    }

    IEnumerator FallBackTimer(float time)
    {
        yield return new WaitForSeconds(time); //Waits for enemy to fall back for this long
        runAgressiveTimer = true; //Rerol
        anim.SetInteger("sideMoving", 0); 
    }

    public void DeathVoid(float amountToReduce) //Will fade the enemy sprite renderer and health bar images out, then destroy the obj
    {
        Color tempColor = healthSlider.transform.GetChild(0).transform.GetComponent<Image>().color; //Makes a temporary color variable which has an opacity (a) that can be changed/reduced; set to the healthBars background's color
        tempColor.a -= 0.05f; //Reduces opacity by float (255 = fully visible, 0 = invisible)
        healthSlider.transform.GetChild(0).GetComponent<Image>().color = tempColor; //Sets the background's color to the new color

        tempColor = gameObject.transform.GetComponent<SpriteRenderer>().color; //Temp color = color of the enemy's sprite renderer
        tempColor.a -= 0.05f;
        gameObject.transform.GetComponent<SpriteRenderer>().color = tempColor;

        if(tempColor.a <= 0) //If object has reached transparency
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().enemyList.Remove(gameObject);
            if (enemyType == 2 || enableDoorOnDeath)
                levelExitDoor.SetActive(true); //Enables exit door
            Destroy(gameObject.transform.parent.gameObject); //Destroy enemy object
        }
    }

    public void GotPunched() //Runs when this enemy is punched
    {
        triedBlocking = false;
        //StartCoroutine(GotPunchedMovementCooldown()); //Starts a timer to reset the bool above once the punch/knockback is done
        agressiveTimer = 1;
        runAgressiveTimer = true;
    }

    IEnumerator GotPunchedMovementCooldown() //Resets isBeingPunched bool so enemy can move again when he is no longer being punched/knocked back
    {
        isBeingPunched = true; //Does not allow this script to set enemy's movement speed so he can be knocked back by the punch
        yield return new WaitForSeconds(3f); //Waits for knockback to be done
        isBeingPunched = false; //Resets movement to normal
        agressiveTimer = 1;
        runAgressiveTimer = true;
    }

    void Block()
    {
        blockTimer = 30;
        isPunching = false;
        //body.velocity = new Vector2(0, 0);
        anim.SetInteger("punchSide", 0);
        anim.SetInteger("sideMoving", 0);
        agressiveRandomiser = 0;
        runAgressiveTimer = false;
        if (playerObj.transform.position.x - gameObject.transform.position.x > 0) //If player is left of enemy
            anim.SetInteger("isBlocking", 1); //Block left
        else //If not
            anim.SetInteger("isBlocking", -1); //Block right
        isBlocking = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Object" || collision.gameObject.tag == "Crate") //If collided with an "object" (ex. crates)
        {
            Jump();
        }
    }
}
