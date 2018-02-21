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

    //UI Objects
    public GameObject sideScrollUIObj;
    public GameObject topDownUIObj;
    public GameObject miscUIObj;

    //Health UI
    public GameObject healthSlider; //The top left slider that shows the player's health
    public GameObject currentHealthTxt; //The text above the slider that displays the player's current health
    public GameObject maxHealthTxt; //Text to the right of the slider that shows the player's maximum possible health
    public GameObject minHealthTxt; //Text to the left of the slider that shows the player's minumum possible health

    //Block UI
    public GameObject blockSlider; //The top left slider that shows the player's block cooldown
    public GameObject currentBlockTxt; //The text above the slider that displays the player's current block cooldown
    public GameObject maxBlockTxt; //Text to the right of the slider that shows the player's maximum possible block cooldown
    public GameObject minBlockTxt; //Text to the left of the slider that shows the player's minumum possible block cooldown

    //KO UI
    public GameObject koSlider; //The slider that appears in the middle of the screen when the player is being KO'd
    public GameObject koText; //The text above the slider that displays the "Spam A or Space..." text
    public GameObject koTimerText; //Text below the slider that counts the timer down

    //Chunk and world loading variables

    public List<Vector3> startPositions; //A list of the coordinates that the player should be placed at when loading each level. Includes both actual leves and towns. Starts at 0 with the first (tutorial) level, 1 first town, etc.
    public List<GameObject> backgroundObjs; //A list of the parent objects of each background tile, used for "chunk managing"
    public int chunkUnloadRange; //How far away a "chunk" needs to be from the player to be "unloaded" (disabled)
    public int chunkLoadRange; //How far away a "chunk" needs to be from the player to be "loaded" (enabled)
    public int entityUnloadRange; //How far away an entity needs to be from the player to be "unloaded" (disabled)
    public int entityLoadRange; //How far away an entity needs to be from the player to be "loaded" (enabled)
    public int lightLoadRange; //How far away a light needs to be from the player to be "loaded" (enabled)
    public int lightUnloadRange;//How far away a light needs to be from the player to be "unloaded" (disabled)

    public List<GameObject> enemyList; //A list of all enemies in the level
    public List<GameObject> lightList; //A list of all lights in the level

    public bool controllerConnected; //if true, run controller only code

    public GameObject powEffectPrefab; //The effect that appears when an enemy punches the player or the other way around

    // Use this for initialization
    void Start () {
        playerObjStatic = playerObj;
        Application.targetFrameRate = 60;
        if(Input.GetJoystickNames().Length > 0)
            controllerConnected = true;
    }

    // Update is called once per frame
    void Update () {
        if(currentView == 2 && playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity.x > 0) //Only runs "chunk manager" when in sidescroll mode & when player is moving
        {
            //Loads/unloads enemies
            foreach (GameObject enemy in enemyList)
            {
                if(enemy == null)
                {
                    enemyList.Remove(enemy);
                    return;
                }
            }

            foreach(GameObject light in lightList)
            {
                if(light.activeSelf == true && Vector3.Distance(playerObj.transform.GetChild(0).position, light.transform.position) > lightUnloadRange)
                {
                    light.SetActive(false);
                }
                else
                {
                    if (light.activeSelf == false && Vector3.Distance(playerObj.transform.GetChild(0).position, light.transform.position) < lightLoadRange)
                    {
                        light.SetActive(true);
                    }
                }
            }
            //Loas/unloads chunks
            foreach (GameObject chunk in backgroundObjs)
            {
                if (chunk.activeSelf == true && Vector3.Distance(playerObj.transform.GetChild(0).position, chunk.transform.position) > chunkUnloadRange) //Unloads chunks farther than the unloadRange from the player
                {
                    chunk.SetActive(false);
                }
                else
                {
                    if (chunk.activeSelf == false && Vector3.Distance(playerObj.transform.GetChild(0).position, chunk.transform.position) < chunkLoadRange) //Loads chunks closer than the loadRange from the player
                    {
                        chunk.SetActive(true);
                    }
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
                for(int k=0;k<background.transform.GetChild(0).transform.childCount;k++)
                {
                    lightList.Add(background.transform.GetChild(0).transform.GetChild(k).gameObject);
                }
            }
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemyList.Add(enemy);
                enemy.SetActive(false);
            }

            foreach (GameObject light in lightList)
            {
                if (light.activeSelf == true && Vector3.Distance(playerObj.transform.GetChild(0).position, light.transform.position) > lightUnloadRange)
                {
                    light.SetActive(false);
                }
                else
                {
                    if (light.activeSelf == false && Vector3.Distance(playerObj.transform.GetChild(0).position, light.transform.position) < lightLoadRange)
                    {
                        light.SetActive(true);
                    }
                }
            }
            //Loas/unloads chunks
            foreach (GameObject chunk in backgroundObjs)
            {
                if (chunk.activeSelf == true && Vector3.Distance(playerObj.transform.GetChild(0).position, chunk.transform.position) > chunkUnloadRange) //Unloads chunks farther than the unloadRange from the player
                {
                    chunk.SetActive(false);
                }
                else
                {
                    if (chunk.activeSelf == false && Vector3.Distance(playerObj.transform.GetChild(0).position, chunk.transform.position) < chunkLoadRange) //Loads chunks closer than the loadRange from the player
                    {
                        chunk.SetActive(true);
                    }
                }
            }
        }
    }

    public void changeToTopdown()
    {
        topDownUIObj.SetActive(true); //Enables the parent of the topdown ui, disables the parent of the sidescroll ui
        sideScrollUIObj.SetActive(false);
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 0.0f; //Stops player from reacting to gravity
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops player
        sideScrollMapObj.SetActive(false); //Disables sidescroll map
        topDownMapObj.SetActive(true); //Enables topdown map
        playerObj.transform.GetChild(0).transform.position = new Vector3(0, 0, 0); //Resets player to middle of screen
        playerObj.transform.GetChild(0).GetComponent<Animator>().SetLayerWeight(1, 1);
        mainCameraObj.transform.position = new Vector3(0, 0, -10);
        mainCameraObj.transform.GetComponent<CameraController>().followY = true;
    }

    public void changeToSidescroll()
    {
        sideScrollUIObj.SetActive(true); //Enables the parent of the sidescroll ui, disables the parent of the topdown ui
        topDownUIObj.SetActive(false);
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 2.5f; //Makes player react to gravity
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops player
        sideScrollMapObj.SetActive(true); //Enables sidescroll map
        topDownMapObj.SetActive(false); //Disables topdown map
        playerObj.transform.GetChild(0).transform.position = new Vector3(-7.6f, -3.55f, 0); //Resets player to start
        playerObj.transform.GetChild(0).GetComponent<Animator>().SetLayerWeight(1, 0);
        mainCameraObj.transform.position = new Vector3(-1.6716f, 0, -10);
        mainCameraObj.transform.GetComponent<CameraController>().followY = false;

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

    public void updateHealthSlider(int minVal, int maxVal, int val) //minVal = minimum "health" on the slider, maxVal = maximum "health" on the slider, val = current "health" on the slider; also updates max, min, and current health txts left, right, and above the slider
    {
        Slider sliderComponent = healthSlider.transform.GetComponent<Slider>(); //Temp variable that keeps track of the slider component on the slider obj; used to clean up code
        sliderComponent.minValue = minVal; //Sets min value on slider
        minHealthTxt.transform.GetComponent<Text>().text = minVal.ToString(); //Updates minimum health txt left of slider
        sliderComponent.maxValue = maxVal; //Sets max value on slider
        maxHealthTxt.transform.GetComponent<Text>().text = maxVal.ToString(); //Updates maximum health txt right of slider
        sliderComponent.value = val; //Sets current value on slider
        currentHealthTxt.transform.GetComponent<Text>().text = "Health: " + val; //Updates current health txt above slider

        if(val <= minVal) //If below or at minimum health
        {
            healthSlider.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false); //Sets fill object on health slider to inactive to prevent small amount of red "health" at the very left
        }
    }

    public void updateBlockSlider(int minVal, int maxVal, int val) //minVal = minimum "block" on the slider, maxVal = maximum "block" on the slider, val = current "block" on the slider; also updates max, min, and current block txts left, right, and above the slider
    {
        Slider sliderComponent = blockSlider.transform.GetComponent<Slider>(); //Temp variable that keeps track of the slider component on the slider obj; used to clean up code
        sliderComponent.minValue = minVal; //Sets min value on slider
        minBlockTxt.transform.GetComponent<Text>().text = minVal.ToString(); //Updates minimum health txt left of slider
        sliderComponent.maxValue = maxVal; //Sets max value on slider
        maxBlockTxt.transform.GetComponent<Text>().text = maxVal.ToString(); //Updates maximum health txt right of slider
        sliderComponent.value = val; //Sets current value on slider
        currentBlockTxt.transform.GetComponent<Text>().text = "Block: " + val; //Updates current health txt above slider

        if (val <= minVal) //If below or at minimum health
            blockSlider.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false); //Sets fill object on block slider to inactive to prevent small amount of blue "block" at the very left
        if (val > minVal) //If above minimum health
            blockSlider.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true); //Sets fill object on block slider to active
    }

    public IEnumerator SpawnPow(Vector3 collPosition) //Spawns a POW damage effect at the position of the collision
    {
        Quaternion powRotation = new Quaternion(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-180, 180), Random.Range(-180, 180)); //Randomises the pow's rotation
        GameObject powObj = Instantiate(powEffectPrefab, collPosition, powRotation); //Instatiates the pow effect obh
        yield return new WaitForSeconds(0.4f); //Waits for (almost) half a second
        Destroy(powObj); //Destroys the pow effect
    }
}
