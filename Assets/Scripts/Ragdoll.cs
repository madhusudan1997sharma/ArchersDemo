using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ragdoll : MonoBehaviour {

    public GameObject[] limbs;
    public Text debug;

    string mytag;

	// Use this for initialization
	void Start () {
        mytag = gameObject.tag;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "arrow")
        {
            collision.transform.parent = transform;

            for (int i = 0; i < limbs.Length; i++)
            {
                limbs[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
            //GetComponent<Rigidbody2D>().velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
            debug.GetComponent<Text>().text = "head shot";
        }
    }
}
