using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearGrabObject : MonoBehaviour {
    public GameObject reticle;

#if USES_OPEN_VR
    private GameObject objectInHand;
    private float distanceToObject;

    // Use this for initialization
    void Start () {
        OVRTouchpad.Create();
        OVRTouchpad.TouchHandler += HandleTouchHandler;
        HideReticle();
    }

    void FixedUpdate()
    {
        if (objectInHand != null)
        {
            HideReticle();
            objectInHand.transform.position = Vector3.Lerp(objectInHand.transform.position, transform.forward * distanceToObject + transform.position, Time.deltaTime * 2);
            objectInHand.transform.rotation = Quaternion.Lerp(objectInHand.transform.rotation, transform.rotation, Time.deltaTime * 2);
        } else {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10, ~LayerMask.NameToLayer("Ignore Raycast"), QueryTriggerInteraction.Ignore))
            {
                ShowRecticle(hit);
            }
            else
            {
                HideReticle();
            }
        }
    }

    void HandleTouchHandler(object sender, System.EventArgs e) {
        OVRTouchpad.TouchArgs touchArgs = (OVRTouchpad.TouchArgs)e;
        if (touchArgs.TouchType == OVRTouchpad.TouchEvent.SingleTap) {
            if (this.objectInHand != null){
                this.ReleaseObject();
            } else {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 10, ~LayerMask.NameToLayer("Ignore Raycast"), QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.gameObject.GetComponent<Rigidbody>() && hit.collider.gameObject.GetComponent<Rigidbody>().isKinematic == false) {
                        GrabObject(hit.collider.gameObject);
                    }
                }
            }
        } else if (touchArgs.TouchType == OVRTouchpad.TouchEvent.Right) {
            distanceToObject -= 0.3f;
        } else if (touchArgs.TouchType == OVRTouchpad.TouchEvent.Left) {
            distanceToObject += 0.3f;
        }

        distanceToObject = Mathf.Min(10, Mathf.Max(-10f, distanceToObject));
    }

    /// <summary>
    ///  Interaction
    /// </summary>
    void GrabObject(GameObject obj) {
        distanceToObject = transform.InverseTransformPoint(obj.transform.position).magnitude;
        objectInHand = obj;
        Rigidbody body = obj.GetComponent<Rigidbody>();
        body.isKinematic = true;
    }

    void ReleaseObject() {
        Rigidbody body = objectInHand.GetComponent<Rigidbody>();
        body.isKinematic = false;
        objectInHand = null;
    }

    /// <summary>
    ///  Reticle UI
    /// </summary>
    void ShowRecticle(RaycastHit hit) {
        reticle.transform.position = hit.point - transform.forward * 0.05f;
        reticle.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
        reticle.SetActive(true);
    }

    void HideReticle() {
        reticle.SetActive(false);
    }
#endif
}
