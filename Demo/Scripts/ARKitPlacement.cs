using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USES_AR_KIT
using UnityEngine.XR.iOS;
#endif

public class ARKitPlacement : MonoBehaviour {

	public Transform m_HitTransform;

#if USES_AR_KIT
	bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
	{
		List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
		if (hitResults.Count > 0) {
			foreach (var hitResult in hitResults) {
				m_HitTransform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
				return true;
			}
		}
		return false;
	}

	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0 && m_HitTransform != null)
		{
			var touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
			{
				var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
				ARPoint point = new ARPoint {
					x = screenPosition.x,
					y = screenPosition.y
				};

				// prioritize reults types
				ARHitTestResultType[] resultTypes = {
					ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
					// if you want to use infinite planes use this:
					//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
					ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
					ARHitTestResultType.ARHitTestResultTypeFeaturePoint
				}; 

				foreach (ARHitTestResultType resultType in resultTypes)
				{
					if (HitTestWithResultType (point, resultType))
					{
						return;
					}
				}
			}
		}
	}
#endif
}
