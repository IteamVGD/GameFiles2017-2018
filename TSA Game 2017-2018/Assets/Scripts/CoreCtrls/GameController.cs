using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour { //18

    //Camera Stuff
    public GameObject mainCameraObj;
    public int cameraTopdownSize;
    public int cameraSidescrollSize;

    public GameObject playerObj;
    public static GameObject playerObjStatic;

    public int currentView; //0 = main menu, 1 = topdown, 2 = sidecroll. Effects movement
    public GameObject sideScrollMapObj;
    public GameObject topDownMapObj;
    public GameObject mainMenuObj;

    public GameObject fringeObj;
    public GameObject contenderObj;
    public GameObject fringeContenderObj;
    public GameObject titleScreenBackgroundFadeObj;
    public bool launchTitle;
    public bool titleFadeInTimer;
    public bool isFading; //If true, the player cant start another fade

    //UI Objects
    public GameObject sideScrollUIObj;
    public GameObject topDownUIObj;
    public GameObject miscUIObj;
    public GameObject creditUIObj;

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
    public List<GameObject> backgroundObjs; //A list of the parent objects of each background tile, used for "chunk managing"
    public int chunkUnloadRange; //How far away a "chunk" needs to be from the player to be "unloaded" (disabled)
    public int chunkLoadRange; //How far away a "chunk" needs to be from the player to be "loaded" (enabled)
    public int entityUnloadRange; //How far away an entity needs to be from the player to be "unloaded" (disabled)
    public int entityLoadRange; //How far away an entity needs to be from the player to be "loaded" (enabled)
    public int lightLoadRange; //How far away a light needs to be from the player to be "loaded" (enabled)
    public int lightUnloadRange;//How far away a light needs to be from the player to be "unloaded" (disabled)
    public List<Sprite> backgroundSpriteList; //Will use these lists to randomise backgrounds on runtime
    public List<Sprite> verticalBackgroundSpriteList; //^^^ same as above, uses this one for vertical backgrounds

    public List<GameObject> enemyList; //A list of all enemies in the level
    public List<GameObject> lightList; //A list of all lights in the level
    public List<GameObject> rollingBagList; //A list of all rolling bags in the level

    public List<string> cratesSortingStrings; //Contains a list of tags. Objects with these tags will be sorted under the 'Crates' parent in a background
    public List<string> bagsChainsSwitchesStrings; //Contains a list of tags. Objects with these tags will be sorted under the 'BagsChains&Switches' parent in a background
    public List<string> floorsWallsStrings; //Contains a list of tags. Objects with these tags will be sorted under the 'Floors&Walls' parent in a background
    public List<string> miscObjectsStrings; //Contains a list of tags. Objects with these tags will be sorted under the 'MiscObjects' parent in a background

    //Effect variables
    public GameObject powEffectPrefab; //The effect that appears when an enemy punches the player or the other way around
    public bool isRemovingPows;

    //City & Level Variables
    public bool dayOrNight; //Controls wether a city should be in day or night mode; False = night, true = day
    public int cityID; //Which city the player is in; 0 = first city
    public bool travellingToCity; //If true, fade out will not switch views, but will stay in topdown and load next city
    public bool travellingToLevel;
    public bool intoBuilding; //If true, go into interior
    public bool outOfBuilding; //If true, leaving interior of building
    public List<GameObject> cities; //The parent objects that hold all objects in a city
    public List<GameObject> citySpawnPoints; //Holds all spawnpoints (where the player should be placed) when travelling into a city from another city
    public List<GameObject> cityLights; //Turned on for night time, off for daytime
    public float cityDayTimeSunIntensity;
    public float cityNightTimeSunIntensity;

    public List<GameObject> cityGymList; //A list of all gyms in ascending order of cities
    public int inOrOutInt; //1 = player is indoors, 2 = player is outdoors
    public int inOrOutIntTemp; //Same as above but keeps the value until the screen fades to black for the transition, then sets the above to this
    public GameObject indoorsUpperBound; //The bounds for the camera movement when indoors; currently not used, camera is just locked when indoors
    public GameObject indoorsLowerBound; //^^^

    public int levelID;
    public List<GameObject> levels;
    public List<GameObject> levelSpawnpoints;
    public GameObject levelParalaxSkyObj; //The scrolling sky that passes by the windows in sidescroll
    public List<GameObject> levelMusicObjs;
    public float levelDayTimeSunIntensity;
    public float levelNightTimeSunIntensity;

    //Switch Stuff
    public bool redOrBlue; //False = red, true = blue
    public List<GameObject> redBoxes; //The red crate parent object, child 0 = crate, child 1 = outline
    public List<GameObject> blueBoxes; //^^^ same but for blue crate
    public List<GameObject> switches;
    public bool canBeSwitched;

    public GameObject playerTeleportSpot; //Used with the teleportDoor transition
    public GameObject teleportDoorBeingUsed; //Door used with val above; resets door to be used again

    //Controller Stuff (d/x input controls are bound via unity editor
    public int dOrXInput; //Changes control scheme depending on whether an xInput controller or dInput controller is connected; 1 = dInput, 2 = xInput
    public int numberOfControllersConnected; //Used in case a controller is disconnected or a new controller is connected to switch schemes
    public bool controllerConnected; //if true, run controller only code

    private void Awake()
    {
        Application.targetFrameRate = 60; //Caps fps at 60
        if (Input.GetJoystickNames().Length > 0) //Checks if there are controllers connected. If not, no need to run controller code
        {
            controllerConnected = true;
            playerObj.transform.GetChild(0).GetComponent<PlayerController>().isAControllerConnected = true;
            ControllerSchemeController(); //Switches to the right scheme (d or x input)
        }
    }

    // Use this for initialization
    void Start () {
        currentView = 0;
        playerObjStatic = playerObj;

        foreach (GameObject redBoxObj in GameObject.FindGameObjectsWithTag("RedBox"))
        {
            redBoxes.Add(redBoxObj);
        }
        foreach (GameObject blueBoxObj in GameObject.FindGameObjectsWithTag("BlueBox"))
        {
            blueBoxes.Add(blueBoxObj);
        }
        foreach (GameObject switchObj in GameObject.FindGameObjectsWithTag("DownPunchable"))
        {
            if (switchObj.layer == 16)
            {
                switches.Add(switchObj);
            }
        }

        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Gym"))
        {
            cityGymList.Add(obj);
        }
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("CityLight"))
        {
            cityLights.Add(obj);
        }

        foreach(GameObject obj in cities)
        {
            obj.SetActive(false);
        }

        playerObj.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (controllerConnected)
            ControllerSchemeController();

        if (currentView == 0 && Input.GetButtonDown("Start"))
        {
            startNewGame();
        }
        if(launchTitle && fringeObj.transform.GetComponent<RectTransform>().position.x < 940)
        {
            fringeObj.transform.position = new Vector3(fringeObj.transform.position.x + 70, fringeObj.transform.position.y, fringeObj.transform.position.z);
        }
        if (launchTitle && contenderObj.transform.GetComponent<RectTransform>().position.x < 940)
        {
            contenderObj.transform.position = new Vector3(contenderObj.transform.position.x + 70, contenderObj.transform.position.y, contenderObj.transform.position.z);
        }
        else
        {
            launchTitle = false;
            StartCoroutine(TitleScreenAnimation());
            fringeContenderObj.SetActive(true);
            contenderObj.SetActive(false);
            fringeObj.SetActive(false);
        }
        if(titleFadeInTimer && fringeContenderObj.transform.GetComponent<Image>().color.a < 255)
        {
            Color tempColor =  fringeContenderObj.transform.GetComponent<Image>().color;
            tempColor.r += 0.02f;
            tempColor.g += 0.02f;
            tempColor.b += 0.02f;
            fringeContenderObj.transform.GetComponent<Image>().color = tempColor;
            tempColor = titleScreenBackgroundFadeObj.transform.GetComponent<Image>().color;
            tempColor.a -= 0.015f;
            titleScreenBackgroundFadeObj.transform.GetComponent<Image>().color = tempColor;
        }
        else
        {
            titleFadeInTimer = false;
        }
        if (GameObject.FindGameObjectsWithTag("Pow").Length > 0 && !isRemovingPows)
        {
            StartCoroutine(RemoveGlitchedPows());
        }
        if(currentView == 2) //Only runs "chunk manager" when in sidescroll mode & when player is moving
        {
            //Loads/unloads enemies
            foreach (GameObject enemy in enemyList)
            {
                if (enemy == null)
                {
                    enemyList.Remove(enemy);
                    break;
                }
                if(enemy.activeSelf == true && Vector3.Distance(playerObj.transform.GetChild(0).position, enemy.transform.position) >= entityUnloadRange)
                {
                    enemy.SetActive(false);
                }
                else
                {
                    if (enemy.activeSelf == false && Vector3.Distance(playerObj.transform.GetChild(0).position, enemy.transform.position) <= entityLoadRange)
                    {
                        enemy.SetActive(true);
                    }
                }
            }
            
            //Light Chunk Loading
            foreach(GameObject light in lightList)
            {
                if(light.activeSelf == true && Vector3.Distance(playerObj.transform.GetChild(0).position, light.transform.position) >= lightUnloadRange)
                {
                    light.SetActive(false);
                }
                else
                {
                    if (light.activeSelf == false && Vector3.Distance(playerObj.transform.GetChild(0).position, light.transform.position) <= lightLoadRange)
                    {
                        light.SetActive(true);
                    }
                }
            }    
            
            //Loas/unloads chunks
            foreach (GameObject chunk in backgroundObjs)
            {
                if (chunk.activeSelf == true && Vector3.Distance(playerObj.transform.GetChild(0).position, chunk.transform.position) >= chunkUnloadRange) //Unloads chunks farther than the unloadRange from the player
                {
                    chunk.SetActive(false);
                }
                else
                {
                    if (chunk.activeSelf == false && Vector3.Distance(playerObj.transform.GetChild(0).position, chunk.transform.position) <= chunkLoadRange) //Loads chunks closer than the loadRange from the player
                    {
                        chunk.SetActive(true);
                    }
                }
            }
            
            //Loads rolling bags
            if(rollingBagList.Count > 0) //If any rolling bags have yet to be activated (only here so errors arent thrown when all bags have ben activated and the list is empty)
            {
                foreach (GameObject bag in rollingBagList)
                {
                    if(bag == null)
                    {
                        rollingBagList.Remove(bag);
                        break;
                    }
                    if (Vector3.Distance(playerObj.transform.GetChild(0).position, bag.transform.position) <= bag.transform.GetComponent<RollingBagController>().enableRange && bag.activeSelf == false) //If the player is close enough that the bag should start rolling
                    {
                        bag.SetActive(true); //Activate bag
                        bag.transform.GetComponent<RollingBagController>().StartRoll();
                    }
                }
            }
        }
	}

    public void ChangeView()
    {
        if (currentView == 3)
        {
            currentView = 1;
            cityID = 1;
            changeToTopdown(false);
        }
        if (currentView == 1 && travellingToLevel)
        {
            currentView = 2;
            changeToSidescroll();
        }
        if(currentView == 2 && travellingToCity)
        {
            currentView = 1;
            changeToTopdown(false);
        }
        if (currentView == 1 && travellingToCity && !intoBuilding && !outOfBuilding)
        {
            changeToTopdown(false);
        }
        if (currentView == 0) //Only runs once per boot when main menu is exited through new game button
        {
            currentView = 1;
            playerObj.SetActive(true);
            mainMenuObj.SetActive(false);

            //Lines below add objects to their lists now instead of adding them during start b/c during start they are disabled and as such cant be found by FindGameObjectsWithTag
            foreach (GameObject background in GameObject.FindGameObjectsWithTag("Background"))
            {
                backgroundObjs.Add(background);
                if(!background.transform.GetComponent<BackgroundController>().blockSpriteRandomiser) //If the background's sprite should be randomised
                {
                    if (background.transform.position.y > 0) //If is a vertical background
                    {
                        //int randomSpriteInt = Random.Range(0, verticalBackgroundSpriteList.Count + 1); //Randomises the sprite for this background if is a vertical background. Needs way to detect if is a "middle" or "top" vertical background
                        //background.transform.GetComponent<SpriteRenderer>().sprite = verticalBackgroundSpriteList[randomSpriteInt];
                    }
                    else
                    {
                        int randomSpriteInt = Random.Range(0, backgroundSpriteList.Count); //Randomises the sprite for this background
                        background.transform.GetComponent<SpriteRenderer>().sprite = backgroundSpriteList[randomSpriteInt];
                    }
                }
                for(int k=0;k<background.transform.GetChild(0).transform.childCount;k++)
                {
                    lightList.Add(background.transform.GetChild(0).transform.GetChild(k).gameObject);
                }
            }

            //Item Hierarchy Sorter
            foreach(string tag in cratesSortingStrings)
            {
                HierarchySorter(tag, 1);
            }
            foreach (string tag in bagsChainsSwitchesStrings)
            {
                HierarchySorter(tag, 2);
            }
            foreach (string tag in floorsWallsStrings)
            {
                HierarchySorter(tag, 3);
            }
            foreach (string tag in miscObjectsStrings)
            {
                HierarchySorter(tag, 4);
            }

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemyList.Add(enemy.transform.parent.gameObject);
                enemy.transform.parent.gameObject.SetActive(false);
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
            changeToTopdown(false);
        }
    }

    public void changeToTopdown(bool kickedOutOfLevel)
    {
        if(travellingToCity && cityID == 999) //Go to credits
        {
            playerObj.SetActive(false);
            currentView = 3;
            creditUIObj.SetActive(true);
            sideScrollMapObj.SetActive(false);
            topDownMapObj.SetActive(false);
            mainCameraObj.GetComponent<CameraController>().enabled = false;
            mainCameraObj.transform.position = new Vector3(0, 0, -10);
            levelParalaxSkyObj.SetActive(false); //Disables scrolling sky
            sideScrollUIObj.SetActive(false);
            topDownUIObj.SetActive(false);
        }
        else
        {
            creditUIObj.SetActive(false);
            playerObj.SetActive(true);
            mainCameraObj.GetComponent<CameraController>().enabled = true;
            currentView = 1;
            topDownUIObj.SetActive(true); //Enables the parent of the topdown ui, disables the parent of the sidescroll ui
            levelParalaxSkyObj.SetActive(false); //Disables scrolling sky
            sideScrollUIObj.SetActive(false);
            playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 0.0f; //Stops player from reacting to gravity
            playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops player
            playerObj.transform.GetChild(0).GetComponent<TDObjSortLayerCtrl>().enabled = true; //Enables the sorting layer controller for topdown
            playerObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Objects"; //Sets the player's sorting layer to 'Objects' instead of 'Player'; 'Objects' is needed in topdown for the sorting layer ctrl to work
            sideScrollMapObj.SetActive(false); //Disables sidescroll map
            topDownMapObj.SetActive(true); //Enables topdown map
            playerObj.transform.GetChild(0).GetComponent<Animator>().SetLayerWeight(1, 1);
            mainCameraObj.transform.GetComponent<CameraController>().followY = true;
            playerObj.transform.GetChild(0).transform.GetComponent<BoxCollider2D>().offset = playerObj.transform.GetChild(0).transform.GetComponent<PlayerController>().topDownColliderOffset;
            playerObj.transform.GetChild(0).transform.GetComponent<BoxCollider2D>().size = playerObj.transform.GetChild(0).transform.GetComponent<PlayerController>().topDownColliderSize;
            mainCameraObj.transform.GetComponent<CameraController>().offset = mainCameraObj.transform.GetComponent<CameraController>().topDownOffset;
            foreach (GameObject city in cities)
            {
                if (city.activeSelf)
                {
                    city.SetActive(false);
                    break;
                }
            }
            cities[cityID].SetActive(true);
            if(!kickedOutOfLevel)
            {
                playerObj.transform.GetChild(0).transform.position = citySpawnPoints[cityID].transform.position;
                mainCameraObj.transform.position = citySpawnPoints[cityID].transform.position;
            }
            else
            {
                playerObj.transform.GetChild(0).transform.position = cityGymList[cityID].transform.position;
                mainCameraObj.transform.position = citySpawnPoints[cityID].transform.position;
            }
            mainCameraObj.transform.GetComponent<CameraController>().canFollowX = true;
            mainCameraObj.transform.GetComponent<CameraController>().canFollowY = true;
            mainCameraObj.transform.GetComponent<Camera>().orthographicSize = cameraTopdownSize;

            //City Stuff
            cities[cityID].SetActive(true); //Enables the city the player is at
            if (!kickedOutOfLevel)
            {
                if (dayOrNight) //If should be day (dayOrNight true = day, false = night)
                {
                    gameObject.transform.GetChild(1).transform.GetComponent<Light>().intensity = cityDayTimeSunIntensity; //Day time sun intensity (default 0.7)
                    foreach (GameObject light in cityLights)
                        light.SetActive(false); //Disables all city lights
                }
                else
                {
                    gameObject.transform.GetChild(1).transform.GetComponent<Light>().intensity = cityNightTimeSunIntensity; //Night time sun intensity (default 0)
                    foreach (GameObject light in cityLights)
                        light.SetActive(true); //Enables all city lights
                }
                if (!travellingToCity)
                {
                    dayOrNight = !dayOrNight; //Flips day/night state from previous level
                }
                else
                {
                    travellingToCity = false;
                }
            }
        }
    }

    public void changeToSidescroll()
    {
        mainCameraObj.GetComponent<CameraController>().enabled = true;
        sideScrollUIObj.SetActive(true); //Enables the parent of the sidescroll ui, disables the parent of the topdown ui
        foreach(GameObject level in levels) //Disables previous level
        {
            if (level.activeSelf)
            {
                level.SetActive(false);
                break;
            }
        }
        levels[levelID].SetActive(true); //Enables next level
        levels[levelID].transform.GetChild(4).gameObject.SetActive(true);
        topDownUIObj.SetActive(false);
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale = 2.5f; //Makes player react to gravity
        playerObj.transform.GetChild(0).GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stops player
        playerObj.transform.GetChild(0).GetComponent<TDObjSortLayerCtrl>().enabled = false; //Disables the sorting layer controller for topdown
        playerObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Player"; //Sets the player's sorting layer to 'Player' instead of 'Objects'; 'Objects' is needed in topdown for the sorting layer ctrl to work
        playerObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 0; //Resets sorting order (in topdown its some crazy number from -1000 to 1000)
        sideScrollMapObj.SetActive(true); //Enables sidescroll map
        topDownMapObj.SetActive(false); //Disables topdown map
        playerObj.transform.GetChild(0).transform.position = levelSpawnpoints[levelID].transform.position;
        mainCameraObj.transform.position = levelSpawnpoints[levelID].transform.position;
        levelParalaxSkyObj.SetActive(true); //Enables the scrolling sky
        levelParalaxSkyObj.transform.position = new Vector3(mainCameraObj.transform.position.x, mainCameraObj.GetComponent<CameraController>().parallaxConstantY, -5); //Sets the sky to the right(ish) position, camera controller takes care of the rest
        playerObj.transform.GetChild(0).GetComponent<Animator>().SetLayerWeight(1, 0);
        mainCameraObj.transform.GetComponent<CameraController>().followY = false;
        playerObj.transform.GetChild(0).transform.GetComponent<BoxCollider2D>().offset = playerObj.transform.GetChild(0).transform.GetComponent<PlayerController>().sideScrollColliderOffset;
        playerObj.transform.GetChild(0).transform.GetComponent<BoxCollider2D>().size = playerObj.transform.GetChild(0).transform.GetComponent<PlayerController>().sideScrollColliderSize;
        mainCameraObj.transform.GetComponent<CameraController>().offset = mainCameraObj.transform.GetComponent<CameraController>().sidescrollOffset;
        mainCameraObj.transform.GetComponent<Camera>().orthographicSize = cameraSidescrollSize;
        //playerObj.transform.GetChild(0).GetComponent<PlayerController>().health = playerObj.transform.GetChild(0).GetComponent<PlayerController>().maxHealth;
        //updateHealthSlider(playerObj.transform.GetChild(0).GetComponent<PlayerController>().minHealth, playerObj.transform.GetChild(0).GetComponent<PlayerController>().maxHealth, playerObj.transform.GetChild(0).GetComponent<PlayerController>().health); //Resets player's health after level
        playerObj.transform.GetChild(0).GetComponent<PlayerController>().blockMeter = playerObj.transform.GetChild(0).GetComponent<PlayerController>().maxBlock;
        updateBlockSlider(playerObj.transform.GetChild(0).GetComponent<PlayerController>().minBlock, playerObj.transform.GetChild(0).GetComponent<PlayerController>().maxBlock, (int) playerObj.transform.GetChild(0).GetComponent<PlayerController>().blockMeter);

        /*if (dayOrNight) //If should be day (dayOrNight true = day, false = night)
        {
            gameObject.transform.GetChild(1).transform.GetComponent<Light>().intensity = levelDayTimeSunIntensity; //Day time sun intensity (default 0.6)
            foreach (GameObject light in lightList)
                light.SetActive(false);
        }
        else
        {
            gameObject.transform.GetChild(1).transform.GetComponent<Light>().intensity = levelNightTimeSunIntensity; //Night time sun intensity (default 0.2)
            foreach (GameObject light in lightList)
                light.SetActive(true);
        }*/
        gameObject.transform.GetChild(1).transform.GetComponent<Light>().intensity = levelDayTimeSunIntensity; //Lowers sun intensity
        cities[cityID].gameObject.SetActive(false); //Disables city 
        travellingToLevel = false;
    }

    public void startNewGame()
    {
        miscUIObj.SetActive(true);
        StartCoroutine(ChangeViewFadeOut(0.02f, 0.04f, 1f));
        Destroy(GetComponent<GCodeEnable>());
    }

    public void startFadeOut(float wait, float add, float waitBetween)
    {
        if(!isFading)
            StartCoroutine(ChangeViewFadeOut(wait, add, waitBetween));
    }

    //Fades to black, then switches view, then sends to changeViewFadeWait
    public IEnumerator ChangeViewFadeOut(float fadeWaitTime, float fadeAddAmount, float timeToWaitBetweenFades) //Fade wait time = how long it will wait between each up in fade, fade add amount is how much it will add to the fade each time it runs, timetowaitbetweenfades how long between fading out -> fading in
    {
        isFading = true; //Stops any more fades from starting until this one ends
        yield return new WaitForSeconds(fadeWaitTime);
        if (miscUIObj.transform.GetChild(0).transform.GetComponent<Image>().color.a < 1)
        {
            Color tempColor = miscUIObj.transform.GetChild(0).transform.GetComponent<Image>().color;
            tempColor.a += fadeAddAmount; //Increases opacity of fade to black image
            miscUIObj.transform.GetChild(0).transform.GetComponent<Image>().color = tempColor;
            StartCoroutine(ChangeViewFadeOut(fadeWaitTime, fadeAddAmount, timeToWaitBetweenFades));
        }
        else
        {
            StartCoroutine(ChangeViewFadeWait(fadeWaitTime, fadeAddAmount, timeToWaitBetweenFades));
            if (playerTeleportSpot != null)
            {
                if (playerTeleportSpot.GetComponent<DoorController>()) //If he is teleporting to another teleport door
                    playerObj.transform.GetChild(0).position = new Vector3(playerTeleportSpot.transform.position.x, playerTeleportSpot.transform.position.y + playerTeleportSpot.GetComponent<DoorController>().tpHereYOffset, playerTeleportSpot.transform.position.z); //Teleports player to the teleport spot & adds the spot's y offset to the player's y
                else
                    playerObj.transform.GetChild(0).position = playerTeleportSpot.transform.position; //Teleports player to the teleport spot
                playerObj.transform.GetChild(0).GetComponent<TDObjSortLayerCtrl>().UpdateSortingOrder(); //Updates the player's sorting order to be accurate with his new y pos
                inOrOutInt = inOrOutIntTemp;

                //Disables camera tracking of player & focuses it on the interior
                Vector3 newCamPos = new Vector3(playerObj.transform.GetChild(0).position.x, playerObj.transform.GetChild(0).position.y + 2, -10); //+ 2 to y so that when tp'ing to the inside of a building the camera focuses on the entire interior
                mainCameraObj.transform.position = newCamPos;
                mainCameraObj.GetComponent<CameraController>().desiredPostion = newCamPos;
                mainCameraObj.GetComponent<CameraController>().canFollowX = false;
                mainCameraObj.GetComponent<CameraController>().canFollowY = false;

                playerTeleportSpot = null;
            }
            else
            {
                ChangeView();
            }
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
            if(teleportDoorBeingUsed != null)
            {
                teleportDoorBeingUsed.transform.GetComponent<DoorController>().isBeingAccessed = false;
                teleportDoorBeingUsed = null;
            }
            isFading = false; //Allows the camera to fade in again when needed
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

        if (val <= minVal) //If below or at minimum health
            blockSlider.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false); //Sets fill object on block slider to inactive to prevent small amount of blue "block" at the very left
        if (val > minVal) //If above minimum health
            blockSlider.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true); //Sets fill object on block slider to active
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

        if (val <= minVal) //If below or at minimum block
            blockSlider.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false); //Sets fill object on block slider to inactive to prevent small amount of blue "block" at the very left
        if (val > minVal) //If above minimum block
            blockSlider.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true); //Sets fill object on block slider to active
    }

    public IEnumerator SpawnPow(Vector3 collPosition) //Spawns a POW damage effect at the position of the collision
    {
        Quaternion powRotation = new Quaternion(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-180, 180), Random.Range(-180, 180)); //Randomises the pow's rotation
        GameObject powObj = Instantiate(powEffectPrefab, collPosition, powRotation); //Instatiates the pow effect obh
        yield return new WaitForSeconds(0.15f); //Waits for (almost) half a second
        Destroy(powObj); //Destroys the pow effect
    }

    IEnumerator RemoveGlitchedPows()
    {
        isRemovingPows = true;
        yield return new WaitForSeconds(3);
        foreach (GameObject pow in GameObject.FindGameObjectsWithTag("Pow"))
        {
            Destroy(pow);
        }
        isRemovingPows = false;
    }

    IEnumerator TitleScreenAnimation()
    {
        yield return new WaitForSeconds(0.7f);
        titleFadeInTimer = true;
    }

    //Switch Stuff
    public void ManageBoxes()
    {
        if (canBeSwitched)
        {
            redOrBlue = !redOrBlue;
            if (redOrBlue)
            {
                foreach (GameObject box in redBoxes)
                {
                    box.transform.GetChild(0).gameObject.SetActive(false);
                }
                foreach (GameObject box in blueBoxes)
                {
                    box.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject box in redBoxes)
                {
                    box.transform.GetChild(0).gameObject.SetActive(true);
                }
                foreach (GameObject box in blueBoxes)
                {
                    box.transform.GetChild(0).gameObject.SetActive(false);
                }
            }

            foreach (GameObject switchObj in switches)
            {
                switchObj.transform.GetComponent<SwitchController>().SwitchSprite();
            }

            StartCoroutine(CanBeSwitchedTimer());
        }
    }

    IEnumerator CanBeSwitchedTimer()
    {
        canBeSwitched = false;
        yield return new WaitForSeconds(0.25f);
        canBeSwitched = true;
    }

    void ControllerSchemeController()
    {
        if(Input.GetJoystickNames().Length != numberOfControllersConnected) //If a controller has been connected or disconnected, or if the game is booting and it needs to check which controller scheme to run (x or d input)
        {
            numberOfControllersConnected = Input.GetJoystickNames().Length;
            dOrXInput = 1; //Default is dInput b/c idk how to test for dInput controllers, but I can test for xInput controllers by seeing if theres xbox in the name
            foreach (string controller in Input.GetJoystickNames())
            {
                Debug.Log("Controller Detected: " + controller);
                if (controller.ToLower().Contains("xbox")) //If an xbox controller is connected, switch to xInput scheme instead of dInput scheme
                    dOrXInput = 2;
            }

            //Scheme Implementation
            if (dOrXInput == 1) //Implement dInput
            {
                playerObj.transform.GetChild(0).GetComponent<PlayerController>().mashButton1 = "dMash1";
                playerObj.transform.GetChild(0).GetComponent<PlayerController>().mashButton2 = "dMash2";
                playerObj.transform.GetChild(0).GetComponent<PlayerController>().blockInput = "dBlock";
                playerObj.transform.GetChild(0).GetComponent<PlayerController>().attackInput = "dAttack";
                playerObj.transform.GetChild(0).GetComponent<PlayerController>().jumpInput = "dJump";
                playerObj.transform.GetChild(0).GetComponent<PlayerController>().interactInput = "dInteract";
                playerObj.transform.GetChild(0).GetComponent<PlayerController>().cancelInteractInput = "dCancelInteract";
            }
            else
            {
                if (dOrXInput == 2) //Implement xInput
                {
                    playerObj.transform.GetChild(0).GetComponent<PlayerController>().mashButton1 = "xMash1";
                    playerObj.transform.GetChild(0).GetComponent<PlayerController>().mashButton2 = "xMash2";
                    playerObj.transform.GetChild(0).GetComponent<PlayerController>().blockInput = "xBlock";
                    playerObj.transform.GetChild(0).GetComponent<PlayerController>().attackInput = "xAttack";
                    playerObj.transform.GetChild(0).GetComponent<PlayerController>().jumpInput = "xJump";
                    playerObj.transform.GetChild(0).GetComponent<PlayerController>().interactInput = "xInteract";
                    playerObj.transform.GetChild(0).GetComponent<PlayerController>().cancelInteractInput = "xCancelInteract";
                }
            }
        }
    }

    void HierarchySorter(string str, int childNumber) //Puts objects under the right background parents; Str = which object type to sort, ex: "Crates"; childNumber = which parent to go under when under the right background, ex: BagsChains&Switches
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(str))
        {
            foreach (GameObject background in backgroundObjs)
            {
                if (background.transform.position.x - 8.8 < obj.transform.position.x && background.transform.position.x + 8.8 > obj.transform.position.x)
                {
                    obj.transform.SetParent(background.transform.GetChild(childNumber));
                    break;
                }
            }
            if (obj.transform.GetComponent<SpriteRenderer>() && obj.transform.GetComponent<SpriteRenderer>().sprite.name == "BoxingBagSide") //If this is a rolling bag
            {
                rollingBagList.Add(obj); //Adds to rolling back list
                obj.SetActive(false); //Disables so it wont roll until we want it to
            }
        }
    }
}
