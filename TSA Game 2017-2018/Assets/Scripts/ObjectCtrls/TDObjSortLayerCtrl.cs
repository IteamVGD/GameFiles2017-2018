using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDObjSortLayerCtrl : MonoBehaviour {

    public SpriteRenderer sRender;

    //New System
    private int standardSubtractor; //The number that this object's y will be subracted from
    public bool canMove; //Controls whether to constantly check for y pos change; If this obj cant move, no need to check
    // Use this for initialization

    public int manualYPos; //Used to overwrite the transform y position
    private float yPos; //The position used in the calculations in UpdateSortingOrder()
    void Start()
    {
        standardSubtractor = 500;
        sRender = GetComponent<SpriteRenderer>();
        UpdateSortingOrder();
        if (GetComponent<Rigidbody2D>() != null)
            canMove = true;
    }

    // Update is called once per frame
    void Update() {
        if(canMove && GetComponent<Rigidbody2D>().velocity.y != 0)
            UpdateSortingOrder();
	}

    public void UpdateSortingOrder()
    {
        if (manualYPos != 0)
            yPos = manualYPos;
        else
            yPos = Mathf.Round(transform.position.y);

        if (transform.position.y >= 0)
            sRender.sortingOrder = (int)(standardSubtractor - yPos);
        if (transform.position.y < 0)
            sRender.sortingOrder = (int)(standardSubtractor + yPos);
    }
}
