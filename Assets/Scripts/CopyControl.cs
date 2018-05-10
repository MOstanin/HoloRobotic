using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyControl : MonoBehaviour {


    public GameObject origin;
    Vector3 old_pos;
	// Use this for initialization
	void Awake () {
        old_pos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        
		if (transform.position != old_pos)
        {
            Vector3 v = old_pos - transform.position;
            origin.transform.position = origin.transform.position - v / 2;
            old_pos = transform.position;
        }
        

    }
}
