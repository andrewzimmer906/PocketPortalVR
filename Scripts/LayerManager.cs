/*
 * Copyright (c) 2017 VR Stuff
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager {
	private static LayerManager instance = null;  

	public List<string> definedLayers;
	private static int totalLayerNum = 31;

	public static LayerManager Instance() {
		if (instance == null) {
			instance = new LayerManager ();
			instance.definedLayers = new List<string> ();
		}
		return instance;
	}

	public int CreateLayer(string name) {
		definedLayers.Add (name);
	
		// Make physics independant
		for (int i = 0; i < definedLayers.Count; i++) {
			for (int j = 0; j < definedLayers.Count; j++) {
				if (i == j) continue;
				Physics.IgnoreLayerCollision (totalLayerNum - i - 1, totalLayerNum - j - 1);
			}
		}

		return totalLayerNum - definedLayers.Count;
	}


}
