using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsCtrl : MonoBehaviour {

    public float scrollSpeed;
    public GameObject creditsObj;
    public GameObject gameController;
    public Vector3 initialPos;

    private void Awake()
    {
        initialPos = creditsObj.GetComponent<RectTransform>().position;
    }

    private void Update()
    {
        if (!GetComponent<AudioSource>().isPlaying)
        {
            StartCoroutine(gameController.GetComponent<GameController>().ChangeViewFadeOut(0.02f, 0.04f, 1f));
        }
    }

    private void OnEnable()
    {
        creditsObj.GetComponent<RectTransform>().position = initialPos;
        creditsObj.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, scrollSpeed);
    }
}
