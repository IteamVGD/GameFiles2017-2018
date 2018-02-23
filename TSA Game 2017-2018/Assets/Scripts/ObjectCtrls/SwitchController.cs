using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour {

    public bool redOrBlue; //False = red, true = blue
    public GameObject redBox; //The red crate parent object, child 0 = crate, child 1 = outline
    public GameObject blueBox; //^^^ same but for blue crate

    public Sprite redSwitchSprite;
    public Sprite blueSwitchSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        redOrBlue = !redOrBlue;
        ManageBoxes();
    }

    private void OnEnable()
    {
        ManageBoxes();
    }

    void ManageBoxes()
    {
        if (redOrBlue)
            EnableBlueBox();
        else
            EnableRedBox();
    }

    void EnableRedBox()
    {
        redBox.transform.GetChild(0).gameObject.SetActive(true); //Enables red box (keeps outline)
        blueBox.transform.GetChild(0).gameObject.SetActive(false); //Disables blue box (keeps outline)
        gameObject.transform.GetComponent<SpriteRenderer>().sprite = redSwitchSprite;
    }

    void EnableBlueBox()
    {
        redBox.transform.GetChild(0).gameObject.SetActive(false); //Disables red box (keeps outline)
        blueBox.transform.GetChild(0).gameObject.SetActive(true); //Enables blue box (keeps outline)
        gameObject.transform.GetComponent<SpriteRenderer>().sprite = blueSwitchSprite;
    }
}
