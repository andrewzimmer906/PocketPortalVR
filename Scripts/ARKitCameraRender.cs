using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ARKitCameraRender : MonoBehaviour {
	
	public Material m_ClearMaterial;
	private CommandBuffer m_VideoCommandBuffer;
	private bool bCommandBufferInitialized;

	public void Start()
	{		
		bCommandBufferInitialized = false;
	}

	void InitializeCommandBuffer()
	{
		m_VideoCommandBuffer = new CommandBuffer(); 
		m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, m_ClearMaterial);
		GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
		bCommandBufferInitialized = true;

	}

	void OnDestroy()
	{
		GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
		bCommandBufferInitialized = false;
	}

	#if !UNITY_EDITOR
	public void OnPreRender()
	{
		if (!bCommandBufferInitialized) {
			InitializeCommandBuffer ();
		}
	}
	#else

	public void OnPreRender()
	{

	if (!bCommandBufferInitialized) {
	InitializeCommandBuffer ();
	}
	}

	#endif
}
