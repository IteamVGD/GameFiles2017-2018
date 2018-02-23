using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollisionController : MonoBehaviour {

    public Collider2D canJumpCollider; //Collider on the top of an object that is used by PlayerController to allow/disallow jumping
    public bool shouldReverseRollingBag; //If when a rolling bag touches this object, it's movement direction should be reversed

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!shouldReverseRollingBag && collision.transform.GetComponent<SpriteRenderer>().sprite.name == "BoxingBagSide")
            shouldReverseRollingBag = true;
    }
}
