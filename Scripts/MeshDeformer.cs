/*
 * Copyright (c) 2017 VR Stuff
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {

	protected Mesh deformingMesh;
	protected Vector3[] originalVertices, displacedVertices;

	public bool lockXEdges = false;
	public bool lockYEdges = false;
	public bool lockZEdges = false;

	void Start () {
		deformingMesh = GetComponent<MeshFilter>().mesh;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices[i] = originalVertices[i];
		}
	}

	public void ClearDeformingForce () {
		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices[i] = originalVertices[i];
		}

		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
	}

	public void AddDeformingForce (Vector3 point, float force) {
        point = transform.InverseTransformPoint(point);
		for (int i = 0; i < displacedVertices.Length; i++) {
			AddForceToVertex(i, point, force);
		}

		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
	}

    public virtual void AddDeformingForce(Vector3 point, float force, bool direction)
    {
        AddDeformingForce(point, force);
    }

    protected virtual void AddForceToVertex (int i, Vector3 point, float force) {
		if (lockXEdges && (originalVertices [i].x == -0.5f ||
			originalVertices [i].x == 0.5f)) {
			return;
		}

		if (lockYEdges && (originalVertices [i].y == -0.5f ||
			originalVertices [i].y == 0.5f)) {
			return;
		}

		if (lockZEdges && (originalVertices [i].z == -0.5f ||
			originalVertices [i].z == 0.5f)) {
			return;
		}
			
		Vector3 pointToVertex = originalVertices[i] - point;
		pointToVertex = new Vector3 (
			pointToVertex.x * transform.localScale.x,
			pointToVertex.y * transform.localScale.y,
			pointToVertex.z * transform.localScale.z);
					
		//pointToVertex *= uniformScale;
		float attenuatedForce = Mathf.Max (Mathf.Abs(force) - pointToVertex.magnitude, 0);
        //float attenuatedForce = Mathf.Max(force - pointToVertex.magnitude, 0);

        Vector3 transformForce = pointToVertex.normalized * attenuatedForce;
		transformForce = new Vector3 (
			transformForce.x / transform.localScale.x,
			transformForce.y / transform.localScale.y,
			transformForce.z / transform.localScale.z);
//		float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);

		displacedVertices [i] = originalVertices [i] + (transformForce);
	}
}
