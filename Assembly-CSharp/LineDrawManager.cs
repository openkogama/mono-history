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

	private static Material lineMaterial;

	private static LineDrawManager instance;

	private Link tempLink;

	private ObjectLink tempObjectLink;

	private GameObject tempLinkObject;

	private Queue<LinkLine> linkLines = new Queue<LinkLine>();

	public static LineDrawManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (LineDrawManager)UnityEngine.Object.FindObjectOfType(typeof(LineDrawManager));
				if (instance == null)
				{
					Debug.LogError("LineDrawManager object could not be found");
					return null;
				}
			}
			return instance;
		}
	}

	private void Start()
	{
		CreateLineMaterial();
	}

	private void OnPostRender()
	{
		DrawEnqueuedLines();
		Camera component = MVGameController.Game.CameraController.GetComponent<Camera>();
		if (!(component != null))
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
				zero = MVGameController.WOCM.GetWorldObjectClient(tempLink.outputWOID).GetOutputConnectorPos();
				color = new Color(0f, 0f, 1f, 1f);
			}
			else
			{
				zero = component.ScreenToWorldPoint(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y, component.nearClipPlane));
			}
			if (tempLink.inputWOID > 0)
			{
				zero2 = MVGameController.WOCM.GetWorldObjectClient(tempLink.inputWOID).GetInputConnectorPos();
				color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				zero2 = component.ScreenToWorldPoint(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y, component.nearClipPlane));
			}
			DrawLine(zero, zero2, color);
		}
		if (tempObjectLink != null)
		{
			Vector3 objectConnectorPos = MVGameController.WOCM.GetWorldObjectClient(tempObjectLink.objectConnectorWOID).GetObjectConnectorPos();
			Vector3 to = component.ScreenToWorldPoint(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y, component.nearClipPlane));
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
			tempLinkObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/LinkObject"));
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

	public void ShowLink(Link link, GameObject linkGameObject)
	{
		if (MVGameController.GameMode == MVGameMode.Edit)
		{
			Vector3 outputConnectorPos = MVGameController.WOCM.GetWorldObjectClient(link.outputWOID).GetOutputConnectorPos();
			Vector3 inputConnectorPos = MVGameController.WOCM.GetWorldObjectClient(link.inputWOID).GetInputConnectorPos();
			LineRenderer componentInChildren = linkGameObject.GetComponentInChildren<LineRenderer>();
			componentInChildren.SetPosition(0, outputConnectorPos);
			componentInChildren.SetPosition(1, inputConnectorPos);
			BoxCollider componentInChildren2 = componentInChildren.gameObject.GetComponentInChildren<BoxCollider>();
			componentInChildren2.gameObject.transform.position = outputConnectorPos + (inputConnectorPos - outputConnectorPos) / 2f;
			componentInChildren2.gameObject.transform.rotation = Quaternion.LookRotation((inputConnectorPos - outputConnectorPos).normalized);
			float magnitude = (inputConnectorPos - outputConnectorPos).magnitude;
			magnitude -= 0.5f;
			magnitude = Mathf.Max(magnitude, 0.2f);
			componentInChildren2.transform.localScale = new Vector3(0.2f, 0.2f, magnitude);
			componentInChildren2.gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2((inputConnectorPos - outputConnectorPos).magnitude / 2f, 1f);
			if (link.isSet)
			{
				componentInChildren2.gameObject.GetComponent<Renderer>().material.color = Color.green;
				Vector2 mainTextureOffset = componentInChildren2.gameObject.GetComponent<Renderer>().material.mainTextureOffset;
				mainTextureOffset.x -= Time.deltaTime * 1.5f;
				componentInChildren2.gameObject.GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset;
			}
			else
			{
				componentInChildren2.gameObject.GetComponent<Renderer>().material.color = Color.grey;
			}
		}
	}

	public void ShowObjectLink(ObjectLink objectLink, GameObject objectLinkGameObject)
	{
		if (MVGameController.GameMode == MVGameMode.Edit)
		{
			Vector3 objectConnectorPos = MVGameController.WOCM.GetWorldObjectClient(objectLink.objectConnectorWOID).GetObjectConnectorPos();
			Vector3 vector = MVGameController.WOCM.GetWorldObjectClient(objectLink.objectWOID).WorldPosition;
			if (MVGameController.WOCM.GetWorldObjectClient(objectLink.objectWOID) is MVCubeModelBase)
			{
				vector = (MVGameController.WOCM.GetWorldObjectClient(objectLink.objectWOID) as MVCubeModelBase).GetWorldCenterPos();
			}
			LineRenderer componentInChildren = objectLinkGameObject.GetComponentInChildren<LineRenderer>();
			componentInChildren.SetPosition(0, objectConnectorPos);
			componentInChildren.SetPosition(1, vector);
			BoxCollider componentInChildren2 = componentInChildren.gameObject.GetComponentInChildren<BoxCollider>();
			componentInChildren2.gameObject.transform.position = objectConnectorPos + (vector - objectConnectorPos) / 2f;
			componentInChildren2.gameObject.transform.rotation = Quaternion.LookRotation((vector - objectConnectorPos).normalized);
			float magnitude = (vector - objectConnectorPos).magnitude;
			magnitude -= 0.5f;
			magnitude = Mathf.Max(magnitude, 0.2f);
			componentInChildren2.transform.localScale = new Vector3(0.2f, 0.2f, magnitude);
			componentInChildren2.gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2((vector - objectConnectorPos).magnitude / 2f, 1f);
			componentInChildren2.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
		}
	}

	public void DrawPendingLink(Link link)
	{
		Vector3 outputConnectorPos = MVGameController.WOCM.GetWorldObjectClient(link.outputWOID).GetOutputConnectorPos();
		Vector3 inputConnectorPos = MVGameController.WOCM.GetWorldObjectClient(link.inputWOID).GetInputConnectorPos();
		Color color = new Color(0f, 1f, 0f, 1f);
		linkLines.Enqueue(new LinkLine(outputConnectorPos, inputConnectorPos, color));
	}

	public void DrawPendingObjectLink(ObjectLink objectLink)
	{
		Vector3 objectConnectorPos = MVGameController.WOCM.GetWorldObjectClient(objectLink.objectConnectorWOID).GetObjectConnectorPos();
		Vector3 worldPosition = MVGameController.WOCM.GetWorldObjectClient(objectLink.objectWOID).WorldPosition;
		Color color = new Color(1f, 1f, 0f, 1f);
		linkLines.Enqueue(new LinkLine(objectConnectorPos, worldPosition, color));
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
		if (MVGameController.GameMode != MVGameMode.Edit)
		{
			return;
		}
		try
		{
			if (MVGameController.Game.CameraController != null)
			{
				Camera component = MVGameController.Game.CameraController.GetComponent<Camera>();
				if (component != null && (component.cullingMask & (1 << LayerMask.NameToLayer("Logic"))) != 0)
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

	private static void CreateLineMaterial()
	{
		if (!lineMaterial)
		{
			lineMaterial = new Material(Shader.Find("LineDrawShader"));
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}
}
