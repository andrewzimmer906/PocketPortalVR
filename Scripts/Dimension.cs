/*
 * Copyright (c) 2017 VR Stuff
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dimension : MonoBehaviour {
	public Material customSkybox;

	[HideInInspector]
	public int layer;

	[Tooltip("This designates this dimension as the original dimension from which the experience will start.")]
	public bool initialWorld = false;
	[Tooltip("This forces the Dimension to only affect the layers during rendering (thus keeping things like raycasting intact). Warning: This will break the automatic physics adjustment that keeps you from hitting things in other dimensions.")]
	public bool forceKeepInitialLayers = false;
	
	[HideInInspector]
	public List<Portal> connectedPortals;

	[HideInInspector]
	public Camera cam;

	private Dictionary<int, int> layerSwitchedChildren;

    private bool mainCameraNeedsSetup = true;

	void Awake() {
		connectedPortals = new List<Portal> ();

		layer = LayerManager.Instance ().CreateLayer (gameObject.name);	
		LayerManager.definedDimensions.Add (this);

		gameObject.layer = layer;

		int defaultLayer = LayerMask.NameToLayer("Default");
		if (!this.forceKeepInitialLayers) {
			Transform[] childrenTransforms = gameObject.GetComponentsInChildren<Transform> ();
			foreach (Transform t in childrenTransforms) {
				t.gameObject.layer = layer;
				if (t.gameObject.GetComponent<Light> ()) {
					Light light = t.gameObject.GetComponent<Light> ();
					light.cullingMask = defaultLayer;
					light.cullingMask |= 1 << layer;
				}		
			}
		}
			
        foreach (Camera camera in Camera.allCameras) {
            if (this.initialWorld)
            {
                CameraExtensions.LayerCullingShow(camera, layer);
                if (camera.GetComponent<Skybox>())
                {
                    camera.GetComponent<Skybox>().material = customSkybox;
                }
            }
            else
            {
                CameraExtensions.LayerCullingHide(camera, layer);
            }
        }
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        /* This is used to enable VRTK kit builds*/
        
         if(mainCameraNeedsSetup)
        {
            if (Camera.main == null) {
                return;
            }            

            if (this.initialWorld)
            {
                CameraExtensions.LayerCullingShow(Camera.main, layer);
                Camera.main.gameObject.layer = layer;

                if (Camera.main.GetComponent<Skybox>())
                {
                    Camera.main.GetComponent<Skybox>().material = customSkybox;
                }
            } else {
                CameraExtensions.LayerCullingHide(Camera.main, layer);
            }

            this.mainCameraNeedsSetup = false;
        }
    }

	// You have just entered this dimension. All portals now point away from it.
	public void SwitchConnectingPortals() {
		foreach (Portal portal in connectedPortals) {
			if (portal.ToDimension () == this) {
				portal.SwitchPortalDimensions ();
			}
		}
	}

    public void showChildrenWithTag(string tag)
    {
        if (tag == "" || tag == null)
        {
            return;
        }

        int defaultLayer = LayerMask.NameToLayer("Default");
        Transform[] childrenTransforms = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in childrenTransforms)
        {
            if (t.CompareTag(tag))
            {
                t.gameObject.layer = defaultLayer;
            }
        }
    }


	// Handle layer switching during rendering if needed
	public void PreRender() {
		if (!forceKeepInitialLayers)
			return;

		if (layerSwitchedChildren == null) {
			layerSwitchedChildren = new Dictionary<int, int> ();
		}

		layerSwitchedChildren.Clear ();

		int defaultLayer = LayerMask.NameToLayer("Default");
		Transform[] childrenTransforms = gameObject.GetComponentsInChildren<Transform> ();
		foreach (Transform t in childrenTransforms) {
			layerSwitchedChildren.Add (t.gameObject.GetInstanceID (), t.gameObject.layer);
			t.gameObject.layer = layer;
			if (t.gameObject.GetComponent<Light> ()) {
				Light light = t.gameObject.GetComponent<Light> ();
				light.cullingMask = defaultLayer;
				light.cullingMask |= 1 << layer;
			}		
		}
	}

	public void PostRender() {
		if (!forceKeepInitialLayers)
			return;

		Transform[] childrenTransforms = gameObject.GetComponentsInChildren<Transform> ();
		foreach (Transform t in childrenTransforms) {
			int layer = layerSwitchedChildren [t.gameObject.GetInstanceID ()];
			t.gameObject.layer = layer;
		}
	}
}
