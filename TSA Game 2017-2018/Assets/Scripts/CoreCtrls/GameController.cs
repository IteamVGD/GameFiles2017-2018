using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour { //18

    public GameObject mainCameraObj;
    public GameObject playerObj;
    public static GameObject playerObjStatic;

    public int currentView; //0 = main menu, 1 = topdown, 2 = sidecroll. Effects movement
    public GameObject sideScrollMapObj;
    public GameObject topDownMapObj;
    public GameObject mainMenuObj;

    public GameObject sideScrollUIObj;
    public GameObject topDownUIObj;
    public GameObject miscUIObj;

    public List<Vector3> startPositions; //A list of the coordinates that the player should be placed at when loading each level. Includes both actual leves and towns. Starts at 0 with the first (tutorial) level, 1 first town, etc.
    public List<GameObject> backgroundObjs; //A list of the parent objects of each background tile, used for "chunk managing"
    public int chunkUnloadRange; //How far away a "chunk" needs to be from the player to be "unloaded" (disabled)
    public int chunkLoadRange; //How far away a "chunk" needs to be from the player to be "loaded" (enabled)
    public int entityUnloadRange; //How far away an entity needs to be from the player to be "unloaded" (disabled)
    public int entityLoadRange; //How far away an entity needs to be from the player to be "loaded" (enabled)

    public List<GameObject> enemyList;

    // Use this for initialization
    void Start () {
        currentView = 0; //0 = main menu, 1= topdown, 2 = sidescroll
        playerObjStatic = playerObj;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update () {
        if(currentView == 2) //Only runs "chunk manager" when in sidescroll mode
        {
            //Loads/unloads enemies
            foreach (GameObject enemy in enemyList)
            {
                if (Vector3.Distance(playerObj.transform.GetChild(0).position, enemy.transform.position) > entityUnloadRange) //Unloads enemies farther than the unloadRange from the player
                {
                    enemy.SetActive(false);
                }
            }
            foreach (GameObject enemy in enemyList)
            {
                if (Vector3.Distance(playerObj.transform.GetChild(0).position, enemy.transform.position) < entityLoadRange) //Loads enemies closer than the loadRange from the player
                {
                    enemy.SetActive(true);
                }
            }
            //Loas/unloads chunks
            foreach (GameObject chunk in backgroundObjs)
            {
                if (chunk.activeSelf == true && Vector3.Distance(playerObj.transform.GetChild(0).position, chunk.transform.position) > chunkUnloadRange) //Unloads chunks farther than the unloadRange from the player
                {
                    chunk.SetActive(false);
                    return;
                }
                if (chunk.activeSelf == false && Vector3.Distance(playerObj.transform.GetChild(0).position, chunk.transform.position) < chunkLoadRange) //Loads chunks closer than the loadRange from the player
                {
                    chunk.SetActive(true);
                    return;
                }
            }
        }
	}

    public void ChangeView()
    {
        if(currentView == 1)
        {
            currentView = 2;
            changeToSidescroll();
            return;
        }
        if(currentView == 2)
        {
            currentView = 1;
            changeToTopdown();
            return;
        }
        if (currentView == 0) //Only runs once per boot when main menu is exited through new game button
        {
            currentView = 2;
            changeToSidescroll();
            playerObj.SetActive(true);
            mainMenuObj.SetActive(false);

            //Lines below add objects to their lists now instead of adding them during start b/c during start they are disabled and as such cant be found by FindGameObjectsWithTag
            foreach (GameObject background in GameObject.FindGameObjectsWithTag("Background"))
            {
                backgroundObjs.Add(background);
            }
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemyList.Add(enemy);
            }
            return;
        }
    }

    public void changeToTopdown()
    {
        topDownUIObj.SetActive(true); //Enables the parent of the topdown ui, disables the part of the sidescroll ui
        sideScrollUIObj.SetActive(false);
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 0.0f; //Stops player from reacting to gravity
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops player
        sideScrollMapObj.SetActive(false); //Disables sidescroll map
        topDownMapObj.SetActive(true); //Enables topdown map
        playerObj.transform.GetChild(0).transform.position = new Vector3(0, 0, 0); //Resets player to middle of screen
        playerObj.transform.GetChild(0).GetComponent<Animator>().SetLayerWeight(1, 1);
        mainCameraObj.transform.position = new Vector3(0, 0, -10);
    }

    public void changeToSidescroll()
    {
        sideScrollUIObj.SetActive(true); //Enables the parent of the sidescroll ui, disables the part of the topdown ui
        topDownUIObj.SetActive(false);
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 2.5f; //Makes player react to gravity
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops player
        sideScrollMapObj.SetActive(true); //Enables sidescroll map
        topDownMapObj.SetActive(false); //Disables topdown map
        playerObj.transform.GetChild(0).transform.position = new Vector3(-7.6f, -3.6f, 0); //Resets player to start
        playerObj.transform.GetChild(0).GetComponent<Animator>().SetLayerWeight(1, 0);
        mainCameraObj.transform.position = new Vector3(-1.6716f, 0, -10);
    }

    public void startNewGame()
    {
        ChangeView();
    }

    //Fades to black, then switches view, then sends to changeviewfadewait
    public IEnumerator ChangeViewFadeOut(float fadeWaitTime, float fadeAddAmount, float timeToWaitBetweenFades) //Fade wait time = how long it will wait between each up in fade, fade add amount is how much it will add to the fade each time it runs, timetowaitbetweenfades how long between fading out -> fading in
    {
        yield return new WaitForSeconds(fadeWaitTime);
        if(miscUIObj.transform.GetChild(0).transform.GetComponent<Image>().color.a < 1)
        {
            Color tempColor = miscUIObj.transform.GetChild(0).transform.GetComponent<Image>().color;
            tempColor.a += fadeAddAmount; //Increases opacity of fade to black image
            miscUIObj.transform.GetChild(0).transform.GetComponent<Image>().color = tempColor;
            StartCoroutine(ChangeViewFadeOut(fadeWaitTime, fadeAddAmount, timeToWaitBetweenFades));
        }
        else
        {
            StartCoroutine(ChangeViewFadeWait(fadeWaitTime, fadeAddAmount, timeToWaitBetweenFades));
            ChangeView();
            StopCoroutine(ChangeViewFadeOut(fadeWaitTime, fadeAddAmount, timeToWaitBetweenFades));
        }
    }

    public IEnumerator ChangeViewFadeWait(float fadeWaitTime, float fadeRemoveAmount, float timeToWaitBetweenFades) //Waits the amount of time specified in timeToWaitBetweenFades before fading back in
    {
        yield return new WaitForSeconds(timeToWaitBetweenFades);
        StartCoroutine(ChangeViewFadeIn(fadeWaitTime, fadeRemoveAmount)); //Starts fading back in
    }

    public IEnumerator ChangeViewFadeIn(float fadeWaitTime, float fadeRemoveAmount) //Fade wait time = how long it will wait between each up in fade, fade add amount is how much it will remove to the fade each time it runs. Fades back in
    {
        yield return new WaitForSeconds(fadeWaitTime);
        if (miscUIObj.transform.GetChild(0).transform.GetComponent<Image>().color.a > 0)
        {
            Color tempColor = miscUIObj.transform.GetChild(0).transform.GetComponent<Image>().color;
            tempColor.a -= fadeRemoveAmount; //Decreases opacity of fade to black image
            miscUIObj.transform.GetChild(0).transform.GetComponent<Image>().color = tempColor;
            StartCoroutine(ChangeViewFadeIn(fadeWaitTime, fadeRemoveAmount));
        }
        else
        {
            StopCoroutine(ChangeViewFadeIn(fadeWaitTime, fadeRemoveAmount));
        }
    }
}
