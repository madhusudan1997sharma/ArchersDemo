using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Arrow : MonoBehaviour {

    Rigidbody2D rb;
    float angle;
    bool rotate = true;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        if (rotate)
        {
            Vector2 v = rb.velocity;
            angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rotate = false;
        if(collision.gameObject.tag == "ground")
        {

        }

        /*if (collision.gameObject.tag == "head" || collision.gameObject.tag == "body")
        {
            transform.parent = collision.gameObject.transform;
        }
        
        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 center = collision.collider.bounds.center;
        if(contactPoint.x > center.x)*/

        //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(GetComponent<PolygonCollider2D>());
            Destroy(GetComponent<Rigidbody2D>());

    }
}
