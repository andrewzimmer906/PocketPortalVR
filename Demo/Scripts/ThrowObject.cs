/*
 * Copyright (c) 2017 VR Stuff
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour {

	public GameObject objectToThrow;
	public Vector3 force;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			
			Vector3 offset = transform.TransformDirection (new Vector3 (0, 0, 2));
			GameObject thrown = Instantiate (objectToThrow, transform.position + offset, Quaternion.identity);
			thrown.GetComponent<Rigidbody> ().AddForce (transform.TransformDirection (force));
			thrown.layer = this.gameObject.layer;
		}
	}
}