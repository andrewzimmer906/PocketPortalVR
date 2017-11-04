# Get the most up to date documentation here
[Documentation](https://docs.google.com/document/d/12Xsh5DVseKkccc0v0BIWNQEPooZdKWDbotQck1QOvF0/edit?usp=sharing)

# Important! Using the demo scenes
The demo scenes use external libraries to enable basic movement controls, integrated VR rendering, and ARKit support. 

**Please read the instructions below for your particular use case!!**

## ARKit Demo
The *ARKit Demo* uses the ARKit asset from unity which you can download for free here:

[Unity ARKit Plugin](https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-arkit-plugin-92515)


To correctly use ARKit follow these steps.

1. Import the [Unity ARKit Plugin](https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-arkit-plugin-92515).  Do this BEFORE importing Pocket Portal.
2. Import `Pocket Portal`
3. Open the ARKit Demo scene in `Pocket Portal/Demo/Scenes`
4. Open `Window -> Portal State Manager` and switch to `ARKit (Apple Only)`
5. Build and run the demo using the iOS target. (Make sure to add open scenes).
7. Run your project on an ARKit supported device (A9 / A10 processor, iPhone 6s and up).
8. Move your device around slowly to detect the ground plane. Once it's detected you can tap to place the portal & other dimesion at the correct location for your demo.

__Note:__ ARKit uses a greenscreen to composite the real world at the far camera plane, this means that **skyboxes are not currently supported**.  My best advice is to use a "fake" skybox like in the ARKit demo scene as a work around.  A 360 video sphere works too!

## FPS Demo
The *FPSDemo* uses a FPS Controller from Unity standard assets.  To run this scene you'll need to download this asset through the unity menus:
    
    Assets -> Import Package -> Characters
    
Once that's completed, make sure you are in the window / mac standalone build and run the Unity Scene.  

__Note:__ The other build settings (iOS for example) don't support controlling the FPS Character from the keyboard.

## SteamVR Demo
The *SteamVRDemo* uses the Steam VR Camera rig from the Steam VR Asset package which you can download for free here:

[Steam VR Asset](https://www.assetstore.unity3d.com/en/#!/content/32647)

1. Import the [Steam VR Asset](https://www.assetstore.unity3d.com/en/#!/content/32647)
2. Import `Pocket Portal`
3. Open `Window -> Portal State Manager` and switch to `Steam VR`
4. Run your project with your Vive or Rift connected!

__Note:__ Make sure to add a skybox component to your main camera if you want to define custom skyboxes for different dimensions.

## VRTK Kit Demo
VRTK Kit allows you to quickly swap between Steam VR and the OpenVR frameworks.  Steam VR works on Vive and Rift, while OpenVR (Occulus Utilities) are Rift only.

1. Open the demo scene `VRTKKit demo`.
2. Load your preferred framework and make sure it shows up correctly in the VRTK_SDK Manager.
3. Tag your controllers `[VRTK_Scripts]/LeftController` and `[VRTK_Scripts]/RightController` and set that tag in the "Ignore Rigidbody Tag" if you'd like to manipulate your portal with touch controllers. This will keep the portal from deforming away from your controllers.

**You may need to reimport the `[VRTK_SDK MANAGER]` to ensure that your scene runs correctly. Make very sure to follow the steps below if this is the case.**

##### STEAM VR
4. Make sure to set the Portal `MainCamera` to the `eyes` camera inside of SteamVR.
5. Add a skybox to your eyes camera.

##### OPEN VR
4. Add the "GearCameraRenderInfo" to your `CenterEyeAnchor` camera inside of OcculusVR.
5. Make sure to set the Portal `MainCamera` to the `CenterEyeAnchor` inside of OcculusVR.
6. Add a skybox to your eyes camera.


NOTE: Make sure to add a skybox component to your main camera if you want to define custom skyboxes for different dimensions.

## GearVR Demo
The *GearVRDemo* uses the Open VR Camera rig from the Open VR API package which you can download for free by selecting "Oculus Utilities for Unity 5":

[Open VR API](https://developer.oculus.com/documentation/unity/latest/concepts/book-unity-gsg/)

1. Download the API
2. Import `Pocket Portal`
3. Setup your Android Device for Gear VR development.
4. Open `Window -> Portal State Manager` and switch to `Open VR`
5. Build and execute the `GearVRDemo` in Pocket Portal.

### Important 
If you demo a VR scene, you'll also need to enable support for that particular library by opening

    Window -> Portal State Manager
and selecting your platform.
 

# How to set up a multi-dimensional scene

The best way is to use one of the demo scenes and copy assets around, but if you'd like to set up your own, the steps are below.

## Create your dimensions.

The 'Dimension' script is added to objects to define many worlds coexisting in the same space, but only accesible through portals. The easiest way to create one is to use a demo scene, but if you'd like to make one from scratch.

1. Create an empty game object.
2. Add the 'Dimension' component to it.
3. Set 'Initial Dimension' to true if you'd like your player to start in this dimension.
4. Add your main camera as a child of the Initial Dimension object.

NOTE: The dimensions use the last layers in your layer mask, so if you are using all 32 layers they will not work.

## Create your portals

The 'Portal' script is responsible for rendering to another dimension and allowing you to transition between dimensions.  

1. Grab a portal from the Portal-Prefabs folder and add it to your scene.
2. Inside the prefab, there is an object called 'portal' that contains the portal script components.
3. Make sure the 'main camera' variable is set to your main camera.
4. Set 'Dimension 1' and 'Dimension 2' to the dimensions you'd like to connect with the portal.

NOTE: Make sure to add a skybox component to your main camera if you want to define custom skyboxes for different dimensions.

## Getting ready for Steam VR

The portal code works in both normal and stereo (VR) camera modes. If you are using Steam VR setup is identical to above with one caveat.

1. Make sure to use the 'eyes' camera as your main camera in the portal "Main Camera' variable.
2. Add a skybox to your eyes camera.
3. Tag your controllers and set that tag in the "Ignore Rigidbody Tag" if you'd like to manipulate your portal with touch controllers. This will keep the portal from deforming away from your controllers.

NOTE: Make sure to add a skybox component to your main camera if you want to define custom skyboxes for different dimensions.

## Getting ready for Open VR

The portal code works in both normal and stereo (VR) camera modes. If you are using Gear VR (or Rift) setup is identical to above with a couple of additional steps.

1. Make sure to use the 'CenterEyeAnchor' camera as your main camera in the portal "Main Camera' variable.
2. Add the "GearCameraRenderInfo" to your 'CenterEyeAnchor' camera.
3. Add the "GearGrabObject" component to your 'CenterEyeAnchor' camera.
4. Add a skybox to your 'CenterEyeAnchor' camera.

NOTE: Make sure to add a skybox component to your main camera if you want to define custom skyboxes for different dimensions.

## Using VRTK Kit

VRTK Kit allows you to quickly swap between Steam VR and the OpenVR frameworks.  Steam VR works on Vive and Rift, while OpenVR (Occulus Utilities) are Rift only.

1. Open the demo scene `VRTKKit demo`.
2. Load your preferred framework and make sure it shows up correctly in the VRTK_SDK Manager.
3. Tag your controllers `[VRTK_Scripts]/LeftController` and `[VRTK_Scripts]/RightController` and set that tag in the "Ignore Rigidbody Tag" if you'd like to manipulate your portal with touch controllers. This will keep the portal from deforming away from your controllers.

NOTE: You may need to reimport the `[VRTK_SDK MANAGER]` to ensure that your scene runs correctly. Make very sure to follow the steps below if this is the case.

#### STEAM VR
4. Make sure to set the Portal `MainCamera` to the `eyes` camera inside of SteamVR.
5. Add a skybox to your eyes camera.

#### OPEN VR
4. Add the "GearCameraRenderInfo" to your `CenterEyeAnchor` camera inside of OcculusVR.
5. Make sure to set the Portal `MainCamera` to the `CenterEyeAnchor` inside of OcculusVR.
6. Add a skybox to your eyes camera.


NOTE: Make sure to add a skybox component to your main camera if you want to define custom skyboxes for different dimensions.

## Support

If you have any additional questions or comments, please contact me directly at andrewzimmer906@gmail.com and I'll do my best to help you out.

Cheers!
