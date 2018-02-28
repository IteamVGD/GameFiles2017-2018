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

    private IEnumerator coroutine;
    public IEnumerator fadeInCoroutine;
    public IEnumerator fadeOutCoroutine;
    public IEnumerator waitForAcceptanceCoroutine;
    public bool isWaitingForAcceptance;

    public int upgradeToGive; //Which upgrade this npc offers; 0 = damage, 1 = health, 2 = block

    public float xDistance;
    public float yDistance;

    public float acceptanceWaitTime;

    private void Start()
    {
        coroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
        fadeInCoroutine = FadeTextBoxIn();
        fadeOutCoroutine = FadeTextBoxOut();
    }

    // Update is called once per frame
    void Update() {

        //Code for talking to npc
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire2")) && Vector3.Distance(gameObject.transform.position, playerObj.transform.position) < talkRange && canTalk && dialogueCount < dialogue.Count - 1 && !isWaitingForAcceptance)
        {
            if(!hasGivenPrompt && playerObj.transform.GetComponent<PlayerController>().vendorCredits >= promptPrice)
            {
                if (!isBeingTalkedTo)
                {
                    isWaitingForAcceptance = false;
                    fadeInCoroutine = FadeTextBoxIn();
                    StartCoroutine(fadeInCoroutine);
                    isBeingTalkedTo = true;
                    playerObj.transform.GetComponent<PlayerController>().isTalkingToNPC = true;
                    StopCoroutine(coroutine);
                    dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().message = dialogue[dialogueCount];
                    dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
                    coroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
                    StartCoroutine(coroutine);
                    dialogueObj.transform.GetChild(2).GetComponent<Text>().text = npcName;
                }
                else
                {
                    dialogueCount++;
                    StopCoroutine(coroutine);
                    dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().message = dialogue[dialogueCount];
                    dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
                    coroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
                    StartCoroutine(coroutine);
                }
            }
            else
            {
                isWaitingForAcceptance = true;
                fadeInCoroutine = FadeTextBoxIn();
                StartCoroutine(fadeInCoroutine);
                isBeingTalkedTo = true;
                playerObj.transform.GetComponent<PlayerController>().isTalkingToNPC = true;
                StopCoroutine(coroutine);
                dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
                dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().message = promptNotAvailable;
                coroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
                StartCoroutine(coroutine);
                waitForAcceptanceCoroutine = WaitForAcceptance();
                StartCoroutine(waitForAcceptanceCoroutine);
            }
            TalkToNPC();
            playerObj.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            StartCoroutine(CanRunTalkAgain()); //Stops player from spamming talk
        }

        //Code for when npc is on last slide of dialogue and player presses button
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire2")) && isBeingTalkedTo && canTalk && dialogueCount == dialogue.Count - 1 && playerObj.transform.GetComponent<PlayerController>().vendorCredits >= promptPrice && !hasGivenPrompt)
        {
            StopCoroutine(coroutine);
            playerObj.transform.GetComponent<PlayerController>().vendorCredits -= promptPrice;
            StopCoroutine(coroutine);
            dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
            dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().message = promptAccepted;
            coroutine = dialogueObj.transform.GetChild(1).GetComponent<TextTyper>().TypeText();
            StartCoroutine(coroutine);
            hasGivenPrompt = true;
            switch(upgradeToGive)
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
            waitForAcceptanceCoroutine = WaitForAcceptance();
            StartCoroutine(waitForAcceptanceCoroutine);
            dialogueCount = 0;
        }

        //Code to stop talking to NPC
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Fire1")) && isBeingTalkedTo && canTalk) 
        {
            playerObj.transform.GetComponent<PlayerController>().isTalkingToNPC = false;
            StopCoroutine(coroutine);
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

    IEnumerator CanRunTalkAgain()
    {
        canTalk = false;
        yield return new WaitForSeconds(antiSpamTimer);
        canTalk = true;
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
        anim.Play(faceForward);
        dialogueObj.transform.GetChild(1).GetComponent<Text>().text = "";
        dialogueObj.transform.GetChild(2).GetComponent<Text>().text = "";
        Color tempColor = dialogueObj.transform.GetChild(0).GetComponent<Image>().color;
        while (tempColor.a > 0)
        {
            tempColor.a -= 0.04f;
            dialogueObj.transform.GetChild(0).GetComponent<Image>().color = tempColor;
            yield return new WaitForSeconds(0.02f);
            tempColor = dialogueObj.transform.GetChild(0).GetComponent<Image>().color;
        }
        isBeingTalkedTo = false;
        playerObj.transform.GetComponent<PlayerController>().isTalkingToNPC = false;
        isWaitingForAcceptance = false;
        StopCoroutine(coroutine);
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
}
