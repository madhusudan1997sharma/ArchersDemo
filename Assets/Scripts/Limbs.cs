using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Limbs : MonoBehaviour {

    public Text debug;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "arrow")
        {
            collision.transform.parent = transform;
            debug.GetComponent<Text>().text = "body shot";
        }
    }
}
