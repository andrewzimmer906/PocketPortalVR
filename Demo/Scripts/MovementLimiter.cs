using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used to keep the ARKit hittest from setting anything more than Y position. This is a hack so I don't have to have ARKit imported to compile :) */
public class MovementLimiter : MonoBehaviour {
	
	public Transform m_MoveTransform;

	private Quaternion oRotation;
	private Vector3 oPosition;

	// Use this for initialization
	void Start () {
		oRotation = m_MoveTransform.rotation;
		oPosition = m_MoveTransform.position;
	}
	
	// Update is called once per frame
	void Update () {
		m_MoveTransform.rotation = oRotation;
		m_MoveTransform.position = new Vector3 (oPosition.x, m_MoveTransform.position.y, oPosition.z);
	}
}
