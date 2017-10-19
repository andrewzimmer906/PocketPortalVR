/*
 * Copyright (c) 2017 VR Stuff
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DimensionChanger {
	public static void SwitchDimensions(GameObject obj, Dimension fromDimension, Dimension toDimension) {
		obj.layer = toDimension.layer;

		// If this is an FPS controller then make sure it goes through too.
		Transform parent = obj.transform.parent;
		if(parent != null && parent.GetComponent<CharacterController>()) {
			parent.gameObject.layer = toDimension.layer;
		}
	}

	public static void SwitchCameraRender(Camera camera, int fromDimensionLayer, int toDimensionLayer, Material dimensionSkybox) {
		CameraExtensions.LayerCullingShow (camera, toDimensionLayer);
		CameraExtensions.LayerCullingHide (camera, fromDimensionLayer);
		if (dimensionSkybox) {
			if (camera.GetComponent<Skybox> ()) {
				camera.GetComponent<Skybox> ().material = dimensionSkybox;	
			}
		}
	}
}
