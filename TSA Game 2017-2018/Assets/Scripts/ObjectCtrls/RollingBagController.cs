using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBagController : MonoBehaviour {

    public float speed; //How fast the bag is spinning
    public int newVel; //How fast it should spin
    public int damage; //How much damage to deal on collision
    public bool damageAgain;

    // Use this for initialization
    void Start () {      
        newVel = 2;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(2, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
    }

    // Update is called once per frame
    void Update () {
        speed = gameObject.transform.GetComponent<Rigidbody2D>().velocity.x;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(newVel, gameObject.transform.GetComponent<Rigidbody2D>().velocity.y);
        transform.Rotate(Vector3.forward * Time.deltaTime * newVel * -90);
	}

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Object" || collision.transform.tag == "Entity")
        {
            newVel *= -1; //Inverses spinning direction
            if(collision.transform.GetComponent<PlayerController>() != null) //Damages the player if he touches the rolling bag
            {
                collision.transform.GetComponent<PlayerController>().TakeDamage(damage);
            }
            if(collision.transform.GetComponent<EnemyController>() != null) //Damages enemies if they touch the rolling bag
            {
                collision.transform.GetComponent<EnemyController>().TakeDamage(damage);
            }
            StartCoroutine(shouldBeDamagedAgain());
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(damageAgain)
        {
            newVel *= -1; //Inverses spinning direction
            if (collision.transform.GetComponent<PlayerController>() != null) //Damages the player if he touches the rolling bag
            {
                collision.transform.GetComponent<PlayerController>().TakeDamage(damage);
            }
            if (collision.transform.GetComponent<EnemyController>() != null) //Damages enemies if they touch the rolling bag
            {
                collision.transform.GetComponent<EnemyController>().TakeDamage(damage);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        damageAgain = false;
    }

    IEnumerator shouldBeDamagedAgain()
    {
        yield return new WaitForSeconds(1);
        damageAgain = true;
    }
}
