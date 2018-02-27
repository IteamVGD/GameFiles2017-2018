using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour {

    public bool redOrBlue; //False = red, true = blue
    public List<GameObject> redBoxes; //The red crate parent object, child 0 = crate, child 1 = outline
    public List<GameObject> blueBoxes; //^^^ same but for blue crate
    public List<GameObject> switches;
    public bool canBeSwitched;

    public Sprite redSwitchSprite;
    public Sprite blueSwitchSprite;

    public GameObject playerObj;
    public bool test;

    private void Start()
    {
        foreach(GameObject redBoxObj in GameObject.FindGameObjectsWithTag("RedBox"))
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
        ManageBoxes();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10 || collision.gameObject.layer == 15)
        {
            redOrBlue = !redOrBlue;
            ManageBoxes();
        }
    }

    void ManageBoxes()
    {
        if(canBeSwitched)
        {
            if (redOrBlue)
            {
                foreach (GameObject switchObj in switches)
                {
                    switchObj.transform.GetComponent<SwitchController>().EnableBlueBox();
                }
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
                foreach (GameObject switchObj in switches)
                {
                    switchObj.transform.GetComponent<SwitchController>().EnableRedBox();
                }
                foreach (GameObject box in redBoxes)
                {
                    box.transform.GetChild(0).gameObject.SetActive(true);
                }
                foreach (GameObject box in blueBoxes)
                {
                    box.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            StartCoroutine(CanBeSwitchedTimer());
        }
    }

    void EnableRedBox()
    {
        gameObject.transform.GetComponent<SpriteRenderer>().sprite = redSwitchSprite;
    }

    void EnableBlueBox()
    {
        gameObject.transform.GetComponent<SpriteRenderer>().sprite = blueSwitchSprite;
    }

    IEnumerator CanBeSwitchedTimer()
    {
        canBeSwitched = false;
        yield return new WaitForSeconds(0.2f);
        canBeSwitched = true;
    }
}
