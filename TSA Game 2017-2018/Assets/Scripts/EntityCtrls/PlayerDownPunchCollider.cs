using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDownPunchCollider : MonoBehaviour
{
    public Vector2 isJumpPunchingOffset;
    public Vector2 isJumpPunchingSize;
    public Vector2 initialOffset;
    public Vector2 initialSize;

    public bool test;

    private void Start()
    {
        initialSize = gameObject.transform.GetComponent<BoxCollider2D>().size;
        initialOffset = gameObject.transform.GetComponent<BoxCollider2D>().offset;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.transform.tag == "DownPunchable")
        {
            gameObject.transform.parent.GetChild(0).GetComponent<PlayerController>().effectiveDownPunch = true;
            gameObject.transform.parent.GetChild(0).GetComponent<PlayerController>().objThatAllowedDownpunch = collision.gameObject;
        }
        if (collision.gameObject.transform.tag == "Enemy")
        {
            gameObject.transform.parent.GetChild(0).GetComponent<PlayerController>().effectiveDownPunch = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.transform.tag == "DownPunchable")
        {
            gameObject.transform.parent.GetChild(0).GetComponent<PlayerController>().effectiveDownPunch = false;
            if (gameObject.transform.parent.GetChild(0).GetComponent<PlayerController>().objThatAllowedDownpunch == collision.gameObject)
            {
                gameObject.transform.parent.GetChild(0).GetComponent<PlayerController>().objThatAllowedDownpunch = null;
            }
        }
        if (collision.gameObject.transform.tag == "Enemy")
        {
            gameObject.transform.parent.GetChild(0).GetComponent<PlayerController>().effectiveDownPunch = false;
        }
    }

    private void Update()
    {
        if (gameObject.transform.parent.GetChild(0).GetComponent<PlayerController>().isDownPunching)
        {
            smallerCollider();
        }
        else
        {
            if (gameObject.transform.GetComponent<BoxCollider2D>().size.x != initialSize.x)
                largerCollider();
        }
    }

    public void smallerCollider()
    {
        gameObject.transform.GetComponent<BoxCollider2D>().size = isJumpPunchingSize;
        gameObject.transform.GetComponent<BoxCollider2D>().offset = isJumpPunchingOffset;
    }

    public void largerCollider()
    {
        gameObject.transform.GetComponent<BoxCollider2D>().size = initialSize;
        gameObject.transform.GetComponent<BoxCollider2D>().offset = initialOffset;
    }
}
