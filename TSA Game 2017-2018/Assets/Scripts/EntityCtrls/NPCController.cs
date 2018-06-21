using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour {

    public int talkRange; //How close the player needs to be to talk to this npc
    public int antiSpamTimer; //How long the player needs to wait between pressing E or Fire3 to talk/skip
    public bool canTalk = true;

    public GameObject playerObj;
    public GameObject uiCanvas;
    public GameObject dialogueObj;
    public Animator anim;

    public string faceLeft = "Left";
    public string faceRight = "Right";
    public string faceForward = "Forward";
    public string faceBack = "Back";
    public bool isBeingTalkedTo;

    public string promptNotAvailable;
    public List<string> dialogue;
    public string promptAccepted;
    public int dialogueCount; //How many dialogues the player has been through

    public bool hasGivenPrompt; //Ex. In state version of game if the player has taken an upgrade from this vendor in the second town
    public int promptPrice; //How much what ever this npc is offering costs (Staes game version ex. beating each gym gives you 1 credit, each upgrade the vendors offer cost 1 credit)
    public string npcName;

    private IEnumerator typeTextCouroutine;
    public IEnumerator fadeInCoroutine;
    public IEnumerator fadeOutCoroutine;
    public IEnumerator waitForAcceptanceCoroutine;
    public bool isWaitingForAcceptance;

    public int upgradeToGive; //Which upgrade this npc offers; 0 = damage, 1 = health, 2 = block

    public float xDistance;
    public float yDistance;

    public float acceptanceWaitTime;

    //Movement AI
    public bool enableAI; //If true, the NPC can walk around
    public bool stationaryAI; //If true, he acts like the npcs that move, but doesnt move
    public bool shouldGiveItem; //If true, npc will offer player something on the last slide of text?

    public int timer; //When this ticks to 0 the npc will move
    public int timerMax = 800; //The max this could be ^
    public int timerMin = 50; //The min this could be ^
    public float timeToMoveFor; //How long the npc will walk in that direction for
    public float maxTimeToMoveFor = 2.5f; //The max this could be ^
    public float minTimeToMoveFor = 0.5f; //The min this could be ^
    public float movementSpeed; //The speed at witch the npc will move
    public float maxMovementSpeed = 2f; //The max this could be ^
    public float minMovementSpeed = 1f; //The min this could be ^
    public int direction; //Which way to move; 1 = left, 2 = right, 3 = up, 4 = down

    public IEnumerator waitToStop;

    public string interactInputString;
    public string cancelInteractInputString;

    private void Start()
    {
        typeTextCouroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
        fadeInCoroutine = FadeTextBoxIn();
        fadeOutCoroutine = FadeTextBoxOut();
        waitToStop = WaitToStopMoving();
        interactInputString = playerObj.GetComponent<PlayerController>().interactInput;
        cancelInteractInputString = playerObj.GetComponent<PlayerController>().cancelInteractInput;
    }

    // Update is called once per frame
    void Update()
    {
        //Movement Code, only runs if enableAI is true, if the npc is NOT being talked to, and if stationaryAI is false
        if(enableAI && !isBeingTalkedTo && !stationaryAI)
        {
            if(timer > 0) //Counts down from random number, when it hits 0 the npc moves. 
                timer--;
            else
            {
                timer = Random.Range(timerMin, timerMax); //Resets timer until npc can move again
                StopCoroutine(WaitToStopMoving());
                Move(); //Makes npc move
            }
        }

        //Code for talking to npc
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown(interactInputString)) && Vector3.Distance(gameObject.transform.position, playerObj.transform.position) < talkRange && canTalk && dialogueCount < dialogue.Count && !isWaitingForAcceptance)
        {
            if(!hasGivenPrompt)
            {
                if (!isBeingTalkedTo)
                {
                    isWaitingForAcceptance = false;
                    fadeInCoroutine = FadeTextBoxIn();
                    StartCoroutine(fadeInCoroutine);
                    isBeingTalkedTo = true;
                    if(enableAI) //Stops the npc from moving while being talked to if he has AI enabled
                    {
                        timer = Random.Range(timerMin, timerMax);
                        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops npc
                    }
                    playerObj.transform.GetComponent<PlayerController>().isTalkingToNPC = true;
                    StopCoroutine(typeTextCouroutine);
                    dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().message = dialogue[dialogueCount];
                    dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
                    typeTextCouroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
                    StartCoroutine(typeTextCouroutine);
                    dialogueObj.transform.GetChild(2).GetComponent<Text>().text = npcName;
                }
                else
                {
                    dialogueCount++;
                    StopCoroutine(typeTextCouroutine);
                    dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().message = dialogue[dialogueCount];
                    dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
                    typeTextCouroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
                    StartCoroutine(typeTextCouroutine);
                }
            }
            TalkToNPC();
            playerObj.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }

        //Code for when npc is on last slide of dialogue and player presses button
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown(interactInputString)) && isBeingTalkedTo && canTalk && dialogueCount == dialogue.Count && !hasGivenPrompt)
        {
            if(playerObj.transform.GetComponent<PlayerController>().money >= promptPrice)
            {
                StopCoroutine(typeTextCouroutine);
                dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
                dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().message = promptAccepted;
                typeTextCouroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
                StartCoroutine(typeTextCouroutine);
                hasGivenPrompt = true;
                waitForAcceptanceCoroutine = WaitForAcceptance();
                StartCoroutine(waitForAcceptanceCoroutine);
                dialogueCount = 0;

                //Gives player something and subtracts a price (if there is one)
                if (shouldGiveItem)
                {
                    playerObj.transform.GetComponent<PlayerController>().money -= promptPrice;
                    switch (upgradeToGive)
                    {
                        case 0:
                            UpgradeDamage();
                            break;
                        case 1:
                            UpgradeHealth();
                            break;
                        case 2:
                            UpgradeBlock();
                            break;
                    }
                }
            }
            else
            {
                StopCoroutine(typeTextCouroutine);
                isWaitingForAcceptance = true;
                fadeInCoroutine = FadeTextBoxIn();
                StartCoroutine(fadeInCoroutine);
                isBeingTalkedTo = true;
                playerObj.transform.GetComponent<PlayerController>().isTalkingToNPC = true;
                StopCoroutine(typeTextCouroutine);
                dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
                dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().message = promptNotAvailable;
                typeTextCouroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
                StartCoroutine(typeTextCouroutine);
                waitForAcceptanceCoroutine = WaitForAcceptance();
                StartCoroutine(waitForAcceptanceCoroutine);
            }
        }

        //Code to stop talking to NPC
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown(cancelInteractInputString)) && isBeingTalkedTo && canTalk) 
        {
            playerObj.transform.GetComponent<PlayerController>().isTalkingToNPC = false;
            StopCoroutine(typeTextCouroutine);
            isBeingTalkedTo = false;
            StopAllCoroutines();
            dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
            dialogueObj.transform.GetChild(2).GetComponent<Text>().text = "";
            fadeOutCoroutine = FadeTextBoxOut();
            StartCoroutine(fadeOutCoroutine);
            dialogueCount = 0;
        }
    }

    public void TalkToNPC()
    {
        xDistance = playerObj.transform.position.x - gameObject.transform.position.x;
        yDistance = playerObj.transform.position.y - gameObject.transform.position.y;

        if (yDistance >= 0 && Mathf.Abs(yDistance) > Mathf.Abs(xDistance)) //If player is up
        {
            anim.Play(faceBack);
        }
        if (yDistance < 0 && Mathf.Abs(yDistance) > Mathf.Abs(xDistance)) //If player is down
        {
            anim.Play(faceForward);
        }
        if (xDistance >= 0 && Mathf.Abs(xDistance) > Mathf.Abs(yDistance)) //If player is right
        {
            anim.Play(faceRight);
        }
        if (xDistance < 0 && Mathf.Abs(xDistance) > Mathf.Abs(yDistance)) //If player is left
        {
            anim.Play(faceLeft);
        }
    }

    IEnumerator FadeTextBoxIn()
    {
        Color tempColor = dialogueObj.transform.GetChild(0).GetComponent<Image>().color;
        while(tempColor.a < 1)
        {
            tempColor.a += 0.04f;
            dialogueObj.transform.GetChild(0).GetComponent<Image>().color = tempColor;
            yield return new WaitForSeconds(0.02f);
            tempColor = dialogueObj.transform.GetChild(0).GetComponent<Image>().color;
        }
    }

    IEnumerator FadeTextBoxOut()
    {
        if(enableAI)
            hasGivenPrompt = false;
        dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
        dialogueObj.transform.GetChild(2).GetComponent<Text>().text = "";
        Color tempColor = dialogueObj.transform.GetChild(0).GetComponent<Image>().color;
        isBeingTalkedTo = false;
        playerObj.transform.GetComponent<PlayerController>().isTalkingToNPC = false;
        dialogueCount = 0;
        isWaitingForAcceptance = false;
        StopCoroutine(typeTextCouroutine);
        while (tempColor.a > 0 && isBeingTalkedTo == false)
        {
            tempColor.a -= 0.04f;
            dialogueObj.transform.GetChild(0).GetComponent<Image>().color = tempColor;
            yield return new WaitForSeconds(0.02f);
            tempColor = dialogueObj.transform.GetChild(0).GetComponent<Image>().color;
        }
    }

    IEnumerator WaitForAcceptance()
    {
        yield return new WaitForSeconds(acceptanceWaitTime);
        fadeOutCoroutine = FadeTextBoxOut();
        StartCoroutine(fadeOutCoroutine);
    }

    public void UpgradeHealth()
    {
        playerObj.transform.GetComponent<PlayerController>().maxHealth += 20;
    }

    public void UpgradeBlock()
    {
        playerObj.transform.GetComponent<PlayerController>().maxBlock += 30;
    }

    public void UpgradeDamage()
    {
        playerObj.transform.GetComponent<PlayerController>().standardPunchDamage += 5;
        playerObj.transform.GetComponent<PlayerController>().punchDamageCap += 10;
    }

    public void Move()
    {
        direction = Random.Range(1, 5); //Which direction to move in. 1-4 (high is exclusive), 1 = left, 2 = right, 3 = up, 4 = down
        movementSpeed = Random.Range(minMovementSpeed, maxMovementSpeed); //+1 because max is exclusive in Random.Range(int)
        timeToMoveFor = Random.Range(minTimeToMoveFor, maxTimeToMoveFor + 1); //^^^
        switch(direction)
        {
            case 1:
                GetComponent<Rigidbody2D>().velocity = new Vector2(-movementSpeed, 0); //Move left
                anim.Play(faceLeft);
                break;
            case 2:
                GetComponent<Rigidbody2D>().velocity = new Vector2(movementSpeed, 0); //Move right
                anim.Play(faceRight);
                break;
            case 3:
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, movementSpeed); //Move up
                anim.Play(faceBack);
                break;
            case 4:
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, -movementSpeed); //Move down
                anim.Play(faceForward);
                break;
        }
        StartCoroutine(WaitToStopMoving());//Starts countdown until npc should stop moving
    }

    IEnumerator WaitToStopMoving()
    {
        yield return new WaitForSeconds(timeToMoveFor); //Waits to stop npc from moving (otherwise it would be instant. start -> stop)
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops npc
    }
}
