using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollisionController : MonoBehaviour {

    public Collider2D canJumpCollider; //Collider on the top of an object that is used by PlayerController to allow/disallow jumping
    public bool shouldReverseRollingBag; //If when a rolling bag touches this object, it's movement direction should be reversed
    public GameObject oppositeBounceBackObject; //When this object is collided with by a rolling bag, if this var is not null then this var's ObjectCollisionController's shouldReverseRollingBag will be set to true

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.GetComponent<RollingBagController>() != null && oppositeBounceBackObject != null && shouldReverseRollingBag)
            oppositeBounceBackObject.transform.GetComponent<ObjectCollisionController>().shouldReverseRollingBag = true;
    }
}
