/*
 * Copyright (c) 2017 VR Stuff
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

#if USES_AR_KIT
using UnityEngine.XR.iOS;
#endif

enum TriggerAxis
{
	X,
	Y,
	Z
};

class RigidbodyCollider
{
	public Collider collider;
	public bool triggerZDirection;
}

public class Portal : MonoBehaviour
{
	[Tooltip("The \"From\" dimension for this portal.")]
	public Dimension dimension1;

	[Tooltip("The \"To\" dimension for this portal.")]
	public Dimension dimension2;

	#if USES_STEAM_VR
	[Tooltip("The Main Camera. On Vive this is [CameraRig] -> Camera (Head) -> Camera (Eye)")]
	#else
	#if USES_OPEN_VR
	[Tooltip("The Main Camera. On Gear VR this is OVRCameraRig -> TrackingSpace -> CenterEyeAnchor")]
	#else
	[Tooltip("The Main Camera. For FPS this is FPSController -> FirstPersonCharacter")]
	#endif
	#endif
	public Camera mainCamera;

    [Tooltip("Increase this value to increase quality. Lower this value to increase performance. Default is 1.")]
    public float renderQuality = 1f;

    [Tooltip("The maximum distance at which the portal begins to deform away from the main camera to avoid clipping.")]
	public float maximumDeformRange = 1f;

	[Tooltip("The power with which the portal deforms away from the camera. If you see a flicker as you pass through, increase this variable.")]
	public float deformPower = .5f;

	[Tooltip("The portal deforms away from rigidbodies, but if you give them a tag and set it here, it will ignore them. This is really good for the Vive Controllers.")]
	public string ignoreRigidbodyTag;

	[Tooltip("This mask defines the parts of the scene that are visible in all dimensions. Always set the layers individually, don't use Everything.")]
	public LayerMask alwaysVisibleMask;

	[Tooltip("Oblique Projection clips the camera exactly to the portal rectangle. This is really good if you've got nearby objects. Unfortunately, it also screws with the skybox on Gear VR.")]
	public bool enableObliqueProjection = false;

	[TextArea]
	[Tooltip(" ")]
	public string Notes = "Hover over each variable to get tooltips with more information on what they do. Quick Tip: " +
		"Don't set the visible mask to Everything. Select each option you want to be always visible.";
	
	private float minimumDeformRangeSquared;
	private bool isDeforming = false;

	private Renderer meshRenderer;
	private MeshFilter meshFilter;
	private MeshDeformer meshDeformer;

	private bool dimensionSwitched;
	private bool triggerZDirection;

	private List<RigidbodyCollider> colliders = new List<RigidbodyCollider> ();

	// Rendering & VR Support
	private Camera renderCam;
	private Skybox camSkybox;
	private RenderTexture leftTexture;
#if USES_STEAM_VR || USES_OPEN_VR
	private RenderTexture rightTexture;
#endif

    private float portalSwitchDistance = 0.03f;

	void Awake() {
		#if USES_STEAM_VR
		Debug.Log("This build is set up to run with Steam VR (Vive / Rift). To enable another headset or run without VR please edit your settings in Window -> Portal State Manager.");
		#else
		#if USES_OPEN_VR
		Debug.Log("This build is set up to run with Open VR (Rift / Gear VR). To enable another headset or run without VR please edit your settings in Window -> Portal State Manager.");
		#else
		#if USES_AR_KIT
		Debug.Log("This build is set up to with ARKit. Please make sure to also import the Unity ARKit Plugin from the Asset Store.");
		#else
		Debug.Log("This build is set up to run without VR or ARKit. To enable VR / AR support please edit your settings in Window -> Portal State Manager.");
#endif
#endif
#endif

        #if USES_OPEN_VR
        Shader.SetGlobalInt("OpenVRRender", 1);
        #else
        Shader.SetGlobalInt("OpenVRRender", 0);
        #endif
    }

    // Use this for initialization
    void Start () {
		meshRenderer = GetComponent<Renderer> ();
		meshFilter = GetComponent<MeshFilter> ();
		meshDeformer = GetComponent<MeshDeformer> ();

		this.gameObject.layer = FromDimension ().layer;

		dimension1.connectedPortals.Add (this);
		dimension2.connectedPortals.Add (this);

        dimension1.showChildrenWithTag(this.ignoreRigidbodyTag);
        dimension2.showChildrenWithTag(this.ignoreRigidbodyTag);

		minimumDeformRangeSquared = maximumDeformRange * maximumDeformRange;

		Vector3 convertedPoint = transform.InverseTransformPoint (mainCamera.transform.position);
		triggerZDirection = (convertedPoint.z > 0);

		if (!mainCamera.GetComponent<MainCameraLayerManager> ()) {
			mainCamera.gameObject.AddComponent<MainCameraLayerManager> ();  // this allows us to alter layers before / after a render!
		}
	}

	/* Rendering and Display */
	void OnWillRenderObject () {
		// Create the textures and camera if they don't exist.
		if (!leftTexture) {
			Vector2 texSize = new Vector2 (mainCamera.pixelWidth, mainCamera.pixelHeight);
			leftTexture = new RenderTexture ((int)(texSize.x * renderQuality), (int)(texSize.y * renderQuality), 16);
#if USES_STEAM_VR || USES_OPEN_VR
			rightTexture = new RenderTexture ((int)(texSize.x * renderQuality), (int)(texSize.y * renderQuality), 16);
#endif
			renderCam = new GameObject (gameObject.name + " render camera", typeof(Camera), typeof(Skybox)).GetComponent<Camera> ();

			#if USES_AR_KIT
			if (mainCamera.GetComponent<UnityARVideo> ()) {
				renderCam.clearFlags = CameraClearFlags.SolidColor;
				ARKitCameraRender component = renderCam.gameObject.AddComponent<ARKitCameraRender> ();
				component.m_ClearMaterial = mainCamera.GetComponent<UnityARVideo> ().m_ClearMaterial;
			}
			#endif

			renderCam.name = gameObject.name + " render camera";
			renderCam.tag = "Untagged";

			if (renderCam.GetComponent<Skybox> ()) {
				camSkybox = renderCam.GetComponent<Skybox> ();	
			} else {
				renderCam.gameObject.AddComponent<Skybox> ();
				camSkybox = renderCam.GetComponent<Skybox> ();	
			}

			CameraExtensions.ClearCameraComponents (renderCam.GetComponent<Camera>());

            // remove child objects to better support VRTKKit
            /*foreach(Transform child in renderCam.transform)
            {
                Destroy(child.gameObject);
            }
			*/

			renderCam.hideFlags = HideFlags.HideInHierarchy;
			renderCam.enabled = false;
		}

		if (ToDimension ().customSkybox) {
			camSkybox.material = ToDimension ().customSkybox;
		}

		meshRenderer.material.SetFloat ("_RecursiveRender", (gameObject.layer != Camera.current.gameObject.layer) ? 1 : 0);
		RenderPortal (Camera.current);
	}

	private void RenderPortal (Camera camera)
	{
#if UNITY_EDITOR
		if (camera.name == "SceneCamera")
			return;
#endif

		if (!this.enableObliqueProjection) {
			Vector3 deltaTransform = transform.position - camera.transform.position;
			renderCam.nearClipPlane = Mathf.Max (deltaTransform.magnitude - meshRenderer.bounds.size.magnitude, 0.01f);
		}
			
#if USES_STEAM_VR || USES_OPEN_VR
		if (camera.stereoEnabled) {  // IE: If we're in VR

            /* Open VR Special */
	#if USES_STEAM_VR
			this.RenderSteamVR (camera);
	#endif

	/* Gear VR Special */
	#if USES_OPEN_VR
			this.RenderOpenVR (camera);
	#endif
        }
        else {  // We're rendering in mono regardless
			this.RenderMono (camera);
		}
#else
	this.RenderMono (camera);   // We force mono in things like ARKit & Hololens
#endif

	}

	private void RenderSteamVR(Camera camera) {
#if USES_STEAM_VR
		if (camera.stereoTargetEye == StereoTargetEyeMask.Both || camera.stereoTargetEye == StereoTargetEyeMask.Left) {
			Vector3 eyePos = camera.transform.TransformPoint (SteamVR.instance.eyes [0].pos);
			Quaternion eyeRot = camera.transform.rotation * SteamVR.instance.eyes [0].rot;
			Matrix4x4 projectionMatrix = GetSteamVRProjectionMatrix (camera, Valve.VR.EVREye.Eye_Left);
			RenderTexture target = leftTexture;

			RenderPlane (renderCam, target, eyePos, eyeRot, projectionMatrix);
			meshRenderer.material.SetTexture ("_LeftTex", target);
		}

		if (camera.stereoTargetEye == StereoTargetEyeMask.Both || camera.stereoTargetEye == StereoTargetEyeMask.Right) {
			Vector3 eyePos = camera.transform.TransformPoint (SteamVR.instance.eyes [1].pos);
			Quaternion eyeRot = camera.transform.rotation * SteamVR.instance.eyes [1].rot;
			Matrix4x4 projectionMatrix = GetSteamVRProjectionMatrix (camera, Valve.VR.EVREye.Eye_Right);
			RenderTexture target = rightTexture;

			RenderPlane (renderCam, target, eyePos, eyeRot, projectionMatrix);
			meshRenderer.material.SetTexture ("_RightTex", target);
		}
#endif
	}

	private void RenderOpenVR(Camera camera) {
#if USES_OPEN_VR
		Vector3 leftPosition = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.LeftEye)) * InputTracking.GetLocalPosition(VRNode.LeftEye);
		Vector3 rightPosition = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.RightEye)) * InputTracking.GetLocalPosition(VRNode.RightEye);
		Vector3 offset = (leftPosition - rightPosition) * 0.5f;

		Matrix4x4 m = camera.cameraToWorldMatrix;
		rightPosition = m.MultiplyPoint(-offset);
		leftPosition = m.MultiplyPoint(offset);

		RenderPlane(renderCam, leftTexture, leftPosition, InputTracking.GetLocalRotation(VRNode.LeftEye), camera.projectionMatrix);
		meshRenderer.material.SetTexture("_LeftTex", leftTexture);

		RenderPlane(renderCam, rightTexture, rightPosition, InputTracking.GetLocalRotation(VRNode.RightEye), camera.projectionMatrix);
		meshRenderer.material.SetTexture("_RightTex", rightTexture);

#endif
	}

	private void RenderMono(Camera camera) {
		RenderTexture target = leftTexture;
		RenderPlane (renderCam, target, camera.transform.position, camera.transform.rotation, camera.projectionMatrix);

		meshRenderer.material.SetTexture ("_LeftTex", target);
		meshRenderer.material.SetFloat ("_RecursiveRender", 1);  // Using Recursive render here will force the shader to only read from the LeftTex texture
	}

	protected void RenderPlane (Camera portalCamera, RenderTexture targetTexture, Vector3 camPosition, Quaternion camRotation, Matrix4x4 camProjectionMatrix) {
		// Copy camera position/rotation/projection data into the reflectionCamera
		portalCamera.transform.position = camPosition;
		portalCamera.transform.rotation = camRotation;
		portalCamera.targetTexture = targetTexture;
		portalCamera.ResetWorldToCameraMatrix ();

		// Change the project matrix to use oblique culling (only show things BEHIND the portal)
		Vector3 pos = transform.position;
		Vector3 normal = transform.forward;
        bool isForward = transform.InverseTransformPoint (portalCamera.transform.position).z < 0;
		Vector4 clipPlane = CameraSpacePlane( portalCamera, pos, normal, isForward ? 1.0f : -1.0f );
		Matrix4x4 projection = camProjectionMatrix;
		if (this.enableObliqueProjection) {
			CalculateObliqueMatrix (ref projection, clipPlane);
		}
		portalCamera.projectionMatrix = projection;

		// Hide the other dimensions
		portalCamera.enabled = false;
		portalCamera.cullingMask = 0;

		CameraExtensions.LayerCullingShow (portalCamera, ToDimension ().layer);
		CameraExtensions.LayerCullingShowMask (portalCamera, alwaysVisibleMask);

		// Update values that are used to generate the Skybox and whatnot.
		portalCamera.farClipPlane = mainCamera.farClipPlane;
		portalCamera.nearClipPlane = mainCamera.nearClipPlane;
		portalCamera.orthographic = mainCamera.orthographic;
		portalCamera.fieldOfView = mainCamera.fieldOfView;
		portalCamera.aspect = mainCamera.aspect;
		portalCamera.orthographicSize = mainCamera.orthographicSize;

		portalCamera.Render ();

		portalCamera.targetTexture = null;
	}

	// Creates a clip plane for the projection matrix that clips to the portal.
	private static void CalculateObliqueMatrix (ref Matrix4x4 projection, Vector4 clipPlane) {
		Vector4 q = projection.inverse * new Vector4(
			sgn(clipPlane.x),
			sgn(clipPlane.y),
			1.0f,
			1.0f
		);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot (clipPlane, q)));

		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
	}

	// Given position/normal of the plane, calculates plane in camera space.
	private Vector4 CameraSpacePlane (Camera cam, Vector3 pos, Vector3 normal, float sideSign) {
        Vector3 offsetPos = pos + normal * portalSwitchDistance * (triggerZDirection ? -1 : 1);
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint( offsetPos );
		Vector3 cnormal = m.MultiplyVector( normal ).normalized * sideSign;
		return new Vector4( cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos,cnormal) );
	}

	// Extended sign: returns -1, 0 or 1 based on sign of a
	private static float sgn(float a) {
		if (a > 0.0f) return 1.0f;
		if (a < 0.0f) return -1.0f;
		return 0.0f;
	}

#if USES_STEAM_VR
	public static Matrix4x4 GetSteamVRProjectionMatrix (Camera cam, Valve.VR.EVREye eye)
	{
	Valve.VR.HmdMatrix44_t proj = SteamVR.instance.hmd.GetProjectionMatrix (eye, cam.nearClipPlane, cam.farClipPlane);
	Matrix4x4 m = new Matrix4x4 ();
	m.m00 = proj.m0;
	m.m01 = proj.m1;
	m.m02 = proj.m2;
	m.m03 = proj.m3;
	m.m10 = proj.m4;
	m.m11 = proj.m5;
	m.m12 = proj.m6;
	m.m13 = proj.m7;
	m.m20 = proj.m8;
	m.m21 = proj.m9;
	m.m22 = proj.m10;
	m.m23 = proj.m11;
	m.m30 = proj.m12;
	m.m31 = proj.m13;
	m.m32 = proj.m14;
	m.m33 = proj.m15;
	return m;
	}
#endif

	/* Main Character Moving Through Portal */
	void Update ()
	{
		if (mainCamera.gameObject.layer != this.FromDimension ().layer) {
			return;  // don't transition if we are in different worlds.
		}

		Vector3 portalSize = meshFilter.mesh.bounds.size;
		bool shouldDeform = 
			(Mathf.Pow (transform.InverseTransformDirection (mainCamera.transform.position - this.transform.position).z, 2) <= minimumDeformRangeSquared) && // z direction is close
			Mathf.Abs (transform.InverseTransformDirection (mainCamera.transform.position - this.transform.position).x) <= (portalSize.x * transform.lossyScale.x) / 2f &&
			Mathf.Abs (transform.InverseTransformDirection (mainCamera.transform.position - this.transform.position).y) <= (portalSize.y * transform.lossyScale.y) / 2f;

		if (shouldDeform) {
			DeformPortalWithTransform (mainCamera.transform);
		} else if (isDeforming) {
			isDeforming = false;
			meshDeformer.ClearDeformingForce ();
		}
	}

	private void DeformPortalWithTransform (Transform otherTransform)
	{
		Vector3 convertedPoint = transform.InverseTransformPoint(otherTransform.position);

		if ((convertedPoint.z > 0) != triggerZDirection && Mathf.Abs(convertedPoint.z) > portalSwitchDistance) {
			triggerZDirection = (convertedPoint.z > 0);
			if (isDeforming) {  // if we're not deforming before this, the user could have walked AROUND the portal.
				SwitchDimensions ();
			}
		}

		meshDeformer.AddDeformingForce (otherTransform.position, deformPower, triggerZDirection);
		isDeforming = true;
	}

	private void SwitchDimensions ()
	{
		DimensionChanger.SwitchCameraRender (this.mainCamera, FromDimension ().layer, ToDimension ().layer, ToDimension ().customSkybox);
		DimensionChanger.SwitchDimensions (this.mainCamera.gameObject, FromDimension (), ToDimension ());
		ToDimension ().SwitchConnectingPortals ();
	}

	public void SwitchPortalDimensions ()
	{
		dimensionSwitched = !dimensionSwitched;
		gameObject.layer = FromDimension ().layer;
	}

	/* Objects with colliders */
	void OnTriggerStay (Collider other)
	{
		RigidbodyCollider curCollider = null;
		foreach (RigidbodyCollider col in colliders) {
			if (col.collider == other) {
				curCollider = col;
				break;
			}
		}

		if (curCollider != null) {
			Vector3 convertedPoint = transform.InverseTransformPoint (other.transform.position);
			if ((convertedPoint.z > 0) != curCollider.triggerZDirection) {
				if (other.gameObject.layer == FromDimension ().layer) {
					DimensionChanger.SwitchDimensions (other.gameObject, FromDimension (), ToDimension ());
				} else {
					DimensionChanger.SwitchDimensions (other.gameObject, ToDimension (), FromDimension ());
				}
			}

			if (!isDeforming) { // don't deform if we are right up against it
				Vector3 transformPosition = other.transform.position;
				if (Mathf.Abs (convertedPoint.z) < maximumDeformRange) {
					convertedPoint.z += triggerZDirection ? maximumDeformRange : -maximumDeformRange;
					transformPosition = transform.TransformPoint (convertedPoint);
				}
				meshDeformer.AddDeformingForce (transformPosition, deformPower);
			}
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.GetComponent<Rigidbody> () &&
			CameraExtensions.CameraForObject (other.gameObject) != mainCamera &&
			(ignoreRigidbodyTag == "" || !other.gameObject.CompareTag (ignoreRigidbodyTag))) {
			RigidbodyCollider collider = new RigidbodyCollider ();
			collider.collider = other;
			collider.triggerZDirection = (transform.InverseTransformPoint (other.transform.position).z > 0);
			colliders.Add (collider);
		}
	}

	void OnTriggerExit (Collider other)
	{
		RigidbodyCollider curCollider = null;
		foreach (RigidbodyCollider col in colliders) {
			if (col.collider == other) {
				curCollider = col;
				break;
			}
		}
		if (curCollider != null) {
			colliders.Remove (curCollider);
			if (colliders.Count == 0 && !isDeforming) {
				meshDeformer.ClearDeformingForce ();
			}
		}
	}

	/* Convenience */
	public Dimension ToDimension ()
	{
		if (dimensionSwitched) {
			return dimension1;
		} else {
			return dimension2;
		}
	}

	public Dimension FromDimension ()
	{
		if (dimensionSwitched) {
			return dimension2;
		} else {
			return dimension1;
		}
	}
}
