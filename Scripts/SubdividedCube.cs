/**
 * Created by Andrew Zimmer.
 * 3/27/2017
 * 
 * 
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction {Up, Down, West, East, North, South};

public class SubdividedCube : MonoBehaviour {
	private MeshFilter meshFilter;
	private Mesh mesh;
	private List<Vector3> verticies = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private int squareCount;

	[Range(0, 4)]
	public int divisions;

	public void Awake() {
		RenderCube ();
	}

	/*-- Public --*/
	public Vector3[] GetCorners() {
		Vector3[] corners = new Vector3 [5];

		corners [0] = transform.TransformVector(new Vector3 (-0.5f, 0.5f, 0)); // bottom-left
		corners [1] = transform.TransformPoint(new Vector3 (0.5f, 0.5f, 0));  // bottom-right
		corners [2] = transform.TransformPoint(new Vector3 (-0.5f, -0.5f, 0));  // top-left
		corners [3] = transform.TransformPoint(new Vector3 (0.5f, -0.5f, 0));   // top-right
		corners [4] = transform.TransformPoint(new Vector3 (0, 0, 0));   // center

		return corners;
	}

	/*-- Render --*/
	public void RenderCube() {
		if (meshFilter == null) {
			meshFilter = GetComponent<MeshFilter> ();
			mesh = new Mesh ();
		}

		float sizeOfFace = 1f / (float)(divisions + 1);

		for (int x = 0; x < divisions + 1; x++) {
			for (int y = 0; y < divisions + 1; y++) {
				for (int z = 0; z < 1; z++) {
					float fx = x * sizeOfFace - 0.5f;
					float fy = y * sizeOfFace - 0.5f;
					float fz = -sizeOfFace / 2;

					if (x == 0) {
						AddSquare (fx, fy, fz, sizeOfFace, Direction.West);
					}
					if (x == divisions) {
						AddSquare (fx, fy, fz, sizeOfFace, Direction.East);
					}

					if (y == 0) {
						AddSquare (fx, fy, fz, sizeOfFace, Direction.Down);
					}
					if (y == divisions) {
						AddSquare (fx, fy, fz, sizeOfFace, Direction.Up);
					}

					if (z == 0) {
						AddSquare (fx, fy, fz, sizeOfFace, Direction.South);
					}
					if (z == 0) {
						AddSquare (fx, fy, fz, sizeOfFace, Direction.North);
					}
				}
			}
		}

		UpdateMesh ();
	}

	/* -- Geometry -- */
	void AddSquare (float x, float y, float z, float size, Direction direction) {
		if (direction == Direction.Up ||
			direction == Direction.Down) {
			AddFaceForTopBottom (x, y, z, size, direction);
		} else if (direction == Direction.North ||
			direction == Direction.South) {
			AddFaceForNorthSouth (x, y, z, size, direction);
		} else if (direction == Direction.West ||
			direction == Direction.East) {
			AddFaceForWestEast (x, y, z, size, direction);
		}

		squareCount++;
	}
		
	void AddFaceForTopBottom(float x, float y, float z, float size, Direction direction) {
		if (direction == Direction.Up) {
			y += size;
		}

		verticies.Add (new Vector3(x, y, z + size));
		verticies.Add (new Vector3(x + size, y , z + size));
		verticies.Add (new Vector3(x + size, y, z));
		verticies.Add (new Vector3(x, y, z));

		AddLatestTriangles (direction != Direction.Up);
	}

	void AddFaceForWestEast(float x, float y, float z, float size, Direction direction) {
		if (direction == Direction.East) {
			x += size;
		}

		verticies.Add ( new Vector3(x, y + size, z) );
		verticies.Add ( new Vector3(x, y + size, z + size));
		verticies.Add ( new Vector3(x, y, z + size));
		verticies.Add ( new Vector3(x, y, z));

		AddLatestTriangles (direction != Direction.East);
	}



	void AddFaceForNorthSouth(float x, float y, float z, float size, Direction direction) {
		if (direction == Direction.North) {
			z += size;
		}

		verticies.Add (new Vector3(x, y + size, z));
		verticies.Add (new Vector3(x + size, y + size, z));
		verticies.Add (new Vector3(x + size, y, z));
		verticies.Add (new Vector3(x, y, z));


		AddLatestTriangles (direction != Direction.South);
	}

	void AddLatestTriangles(bool clockwise) {
		if (clockwise) {
			triangles.Add(squareCount * 4 ); // 1
			triangles.Add(squareCount * 4 + 1 ); // 2
			triangles.Add(squareCount * 4 + 2 ); // 3

			triangles.Add(squareCount * 4  ); // 1
			triangles.Add(squareCount * 4 + 2 ); // 3
			triangles.Add(squareCount * 4 + 3 ); // 4
		} else {
			triangles.Add(squareCount * 4 + 2 ); // 3
			triangles.Add(squareCount * 4 + 1 ); // 2
			triangles.Add(squareCount * 4 ); // 1

			triangles.Add(squareCount * 4 + 3 ); // 4
			triangles.Add(squareCount * 4 + 2 ); // 3
			triangles.Add(squareCount * 4  ); // 1
		}
	}

	/* -- Draw -- */
	void UpdateMesh () {
		mesh.Clear ();
		mesh.vertices = verticies.ToArray();
		mesh.triangles = triangles.ToArray();

		mesh.RecalculateNormals ();

		// Reset back to factory
		verticies.Clear();
		triangles.Clear();
		squareCount = 0;

		meshFilter.sharedMesh = mesh;
	}
}
