/*
 * Copyright (c) 2017 VR Stuff
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CameraExtensions {

	public static void ClearCameraComponents(this Camera camera) {
		// Get rid of extra components
		if (camera.GetComponent<AudioListener> ()) {
			MonoBehaviour.Destroy (camera.GetComponent<AudioListener> ());
		}
		if (camera.GetComponent<FlareLayer> ()) {
			MonoBehaviour.Destroy (camera.GetComponent<FlareLayer> ());
		}
	
		// This keeps AR Tracking from getting buggy
		/*
		foreach (Component component in camera.GetComponents<Component>()) {
			if (component.name.Contains ("Unity AR")) {
				MonoBehaviour.Destroy (component);
			}
		}
		*/

		camera.gameObject.tag = "Untagged";
		camera.enabled = false;
	}

	public static void LayerCullingShow(this Camera cam, int layer) {
		cam.cullingMask |= 1 << layer;
	}

	public static void LayerCullingShowMask(this Camera cam, int layerMask) {
		cam.cullingMask |= layerMask;
	}

	public static void LayerCullingHide(this Camera cam, int layerMask) {
		cam.cullingMask &=  ~(1 << layerMask);
	}

	public static void LayerCullingToggle(this Camera cam, int layerMask) {
		cam.cullingMask ^= layerMask;
	}

	public static bool LayerCullingIncludes(this Camera cam, int layerMask) {
		return (cam.cullingMask & layerMask) > 0;
	}

	public static Camera CameraForObject(this GameObject obj) {
		if (obj.GetComponent<Camera> ()) {
			return obj.GetComponent<Camera> ();
		}

		if (obj.GetComponentInChildren<Camera> ()) {
			return obj.GetComponentInChildren<Camera> ();
		}

		return null;
	}
}