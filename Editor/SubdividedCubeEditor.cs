/**
 * Created by Andrew Zimmer.
 * 3/27/2017
 * 
 * Free to use and distribute.
 * 
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (SubdividedCube))]
[RequireComponent(typeof(MeshFilter))]

public class SubdividedCubeEditor : Editor {

	public override void OnInspectorGUI () {
		SubdividedCube cubeDivide = (SubdividedCube)target;

		if (DrawDefaultInspector ()) {			
		}

		if (GUILayout.Button ("Render")) {
			cubeDivide.RenderCube ();
		}
	}
}
