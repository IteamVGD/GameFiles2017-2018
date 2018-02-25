using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBagController : MonoBehaviour {

    public int startingVelocity = -2; //The speed (and direction) that the bag should be rolling in when the game starts; default = left at -2 speed
    public int newVel; //How fast it should spin
    public int damage; //How much damage to deal on collision
    public bool damageAgain;
    public int enableRange;

    // Update is called once per frame
    void Update ()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * newVel * -90);
        if (gameObject.transform.GetComponent<Rigidbody2D>().velocity.x == 0)
        {
            newVel *= -1; //Reverses spinning direction
        }
        if (gameObject.transform.GetComponent<Rigidbody2D>().velocity.x != newVel)
            gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(newVel, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y); //Sets new rolling velocity
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (((collision.transform.tag == "Object" || collision.transform.tag == "Crate") && collision.transform.GetComponent<ObjectCollisionController>().shouldReverseRollingBag))
        {
            newVel *= -1; //Reverses spinning direction
            gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(newVel, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y); //Sets new rolling velocity
            if (collision.transform.GetComponent<PlayerController>() != null) //Damages the player if he touches the rolling bag
                collision.transform.GetComponent<PlayerController>().TakeDamage(damage);
            if(collision.transform.GetComponent<EnemyController>() != null) //Damages enemies if they touch the rolling bag
                collision.transform.GetComponent<EnemyController>().TakeDamage(damage, false);
            StartCoroutine(ShouldBeDamagedAgain());
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if(damageAgain)
        {
            if (collision.transform.GetComponent<PlayerController>() != null) //Damages the player if he touches the rolling bag
                collision.transform.GetComponent<PlayerController>().TakeDamage(damage);
            if (collision.transform.GetComponent<EnemyController>() != null) //Damages enemies if they touch the rolling bag
                collision.transform.GetComponent<EnemyController>().TakeDamage(damage, false);
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(newVel, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y); //Sets new rolling velocity
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        damageAgain = false;
    }

    IEnumerator ShouldBeDamagedAgain()
    {
        yield return new WaitForSeconds(1);
        damageAgain = true;
    }

    public void StartRoll()
    {
        newVel = startingVelocity;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(startingVelocity, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
        gameObject.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; //Unfreezes object
    }
}
