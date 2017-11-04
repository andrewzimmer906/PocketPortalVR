using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraLayerManager : MonoBehaviour {

	void OnPreCull() {
		foreach (Dimension dimension in LayerManager.definedDimensions) {
			dimension.PreRender ();
		}
	}

	void OnPostRender() {
		foreach (Dimension dimension in LayerManager.definedDimensions) {
			dimension.PostRender ();
		}
	}
}
