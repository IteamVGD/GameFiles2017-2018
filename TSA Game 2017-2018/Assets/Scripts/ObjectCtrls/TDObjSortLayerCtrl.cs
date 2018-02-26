using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDObjSortLayerCtrl : MonoBehaviour {

    public GameObject playerObj;
    public int checkRange;
    public string lowerLayer = "TopBottomObjects";
    public string higherLayer = "TopObjects";
    public float offset;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Vector2.Distance(transform.position, playerObj.transform.position) < checkRange && playerObj.transform.GetComponent<Rigidbody2D>().velocity.y != 0)
        {
            if(playerObj.transform.position.y < transform.position.y + offset && GetComponent<SpriteRenderer>().sortingLayerName != lowerLayer)
            {
                GetComponent<SpriteRenderer>().sortingLayerName = lowerLayer;
            }
            if (playerObj.transform.position.y > transform.position.y + offset && GetComponent<SpriteRenderer>().sortingLayerName != higherLayer)
            {
                GetComponent<SpriteRenderer>().sortingLayerName = higherLayer;
            }
        }
	}
}
