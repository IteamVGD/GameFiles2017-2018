using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour {

    public bool redOrBlue; //False = red, true = blue
    public Sprite redSwitchSprite;
    public Sprite blueSwitchSprite;

    public bool triggerSongObj; //Enables the first child object to trigger an audio source
    public GameObject levelMusicSource; //Is disabled if this object triggers a song

    public void SwitchSprite()
    {
        redOrBlue = !redOrBlue;
        if (redOrBlue)
            GetComponent<SpriteRenderer>().sprite = blueSwitchSprite;
        else
            GetComponent<SpriteRenderer>().sprite = redSwitchSprite;
    }

    public void TriggerSong()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        levelMusicSource.SetActive(false);
    }
}
