using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class LineDrawManager : MonoBehaviour
{
	private class LinkLine
	{
		public Vector3 startPos;

		public Vector3 endPos;

		public Color color;

		public LinkLine(Vector3 startPos, Vector3 endPos, Color color)
		{
			this.startPos = startPos;
			this.endPos = endPos;
			this.color = color;
		}
	}

	[SerializeField]
	private Material lineMaterial;

	private Link tempLink;

	private ObjectLink tempObjectLink;

	private LinkObjectScript tempLinkObject;

	private Queue<LinkLine> linkLines = new Queue<LinkLine>();

	protected void OnPostRender()
	{
		DrawEnqueuedLines();
		Camera mainCamera = MVGameControllerBase.MainCameraManager.MainCamera;
		if (!(mainCamera != null))
		{
			return;
		}
		if (tempLink != null)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Color color = new Color(0f, 0f, 1f, 1f);
			if (tempLink.outputWOID > 0)
			{
				zero = MVGameControllerBase.WOCM.GetWorldObjectClient(tempLink.outputWOID).GetOutputConnectorPos();
				color = new Color(0f, 0f, 1f, 1f);
			}
			else
			{
				zero = mainCamera.ScreenToWorldPoint(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y, mainCamera.nearClipPlane));
			}
			if (tempLink.inputWOID > 0)
			{
				zero2 = MVGameControllerBase.WOCM.GetWorldObjectClient(tempLink.inputWOID).GetInputConnectorPos();
				color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				zero2 = mainCamera.ScreenToWorldPoint(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y, mainCamera.nearClipPlane));
			}
			DrawLine(zero, zero2, color);
		}
		if (tempObjectLink != null)
		{
			Vector3 objectConnectorPos = MVGameControllerBase.WOCM.GetWorldObjectClient(tempObjectLink.objectConnectorWOID).GetObjectConnectorPos();
			Vector3 to = mainCamera.ScreenToWorldPoint(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y, mainCamera.nearClipPlane));
			Color color2 = new Color(1f, 1f, 0f, 1f);
			DrawLine(objectConnectorPos, to, color2);
		}
	}

	public void SetTempLink(Link link)
	{
		tempLink = link;
		if (link != null)
		{
			if (tempLinkObject != null)
			{
				UnityEngine.Object.Destroy(tempLinkObject);
			}
			tempLinkObject = UnityEngine.Object.Instantiate(PrefabPool.Instance.LinkObject);
		}
		else if (tempLinkObject != null)
		{
			UnityEngine.Object.Destroy(tempLinkObject);
			tempLinkObject = null;
		}
	}

	public void SetTempObjectLink(ObjectLink link)
	{
		tempObjectLink = link;
	}

	public void DrawLineDirect(Vector3 from, Vector3 to, Color color)
	{
		lineMaterial.SetPass(0);
		GL.Begin(1);
		GL.Color(color);
		GL.Vertex3(from.x, from.y, from.z);
		GL.Vertex3(to.x, to.y, to.z);
		GL.End();
	}

	private void DrawLine(Vector3 from, Vector3 to, Color color)
	{
		if (MVGameControllerBase.GameMode != MVGameMode.Edit)
		{
			return;
		}
		try
		{
			if (MVGameControllerBase.MainCameraManager != null)
			{
				Camera mainCamera = MVGameControllerBase.MainCameraManager.MainCamera;
				if (mainCamera != null && (mainCamera.cullingMask & (1 << LayerMask.NameToLayer("Logic"))) != 0)
				{
					lineMaterial.SetPass(0);
					GL.Begin(1);
					GL.Color(color);
					GL.Vertex3(from.x, from.y, from.z);
					GL.Vertex3(to.x, to.y, to.z);
					GL.End();
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("LineDrawManager exception: " + ex);
		}
	}

	private void DrawEnqueuedLines()
	{
		while (linkLines.Count > 0)
		{
			DrawLine(linkLines.Dequeue());
		}
	}

	private void DrawLine(LinkLine linkLine)
	{
		DrawLine(linkLine.startPos, linkLine.endPos, linkLine.color);
	}
}
