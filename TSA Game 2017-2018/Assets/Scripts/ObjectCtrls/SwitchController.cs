using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour {

    public bool redOrBlue; //False = red, true = blue
    public Sprite redSwitchSprite;
    public Sprite blueSwitchSprite;

    public void SwitchSprite()
    {
        redOrBlue = !redOrBlue;
        if (redOrBlue)
            GetComponent<SpriteRenderer>().sprite = blueSwitchSprite;
        else
            GetComponent<SpriteRenderer>().sprite = redSwitchSprite;
    }
}
