using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;

public class ARFoundationPlacement : MonoBehaviour
{
    public ARSessionOrigin sessionOrigin;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            print("SCREEN TOUCHED!");
            List<ARRaycastHit> results = new List<ARRaycastHit>();
            sessionOrigin.Raycast(Input.GetTouch(0).position, results, UnityEngine.Experimental.XR.TrackableType.Planes);

            if (results.Count > 0) {
                print("Plane HIT!!");
                ARRaycastHit hit = results[0];
                this.gameObject.transform.position = hit.pose.position;
            }
        }
    }
}
