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
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
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
			if ((Object)(object)instance == (Object)null)
			{
				instance = (LineDrawManager)(object)Object.FindObjectOfType(typeof(LineDrawManager));
				if ((Object)(object)instance == (Object)null)
				{
					Debug.LogError((object)"LineDrawManager object could not be found");
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		DrawEnqueuedLines();
		Camera camera = ((Component)MVGameController.Instance.Game.CameraController).camera;
		if (!((Object)(object)camera != (Object)null))
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
				zero = MVGameController.Instance.WOCM.GetWorldObjectClient(tempLink.outputWOID).GetOutputConnectorPos();
				color = new Color(0f, 0f, 1f, 1f);
			}
			else
			{
				zero = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
			}
			if (tempLink.inputWOID > 0)
			{
				zero2 = MVGameController.Instance.WOCM.GetWorldObjectClient(tempLink.inputWOID).GetInputConnectorPos();
				color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				zero2 = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
			}
			DrawLine(zero, zero2, color);
		}
		if (tempObjectLink != null)
		{
			Vector3 objectConnectorPos = MVGameController.Instance.WOCM.GetWorldObjectClient(tempObjectLink.objectConnectorWOID).GetObjectConnectorPos();
			Vector3 to = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
			Color color2 = new Color(1f, 1f, 0f, 1f);
			DrawLine(objectConnectorPos, to, color2);
		}
	}

	public void SetTempLink(Link link)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		tempLink = link;
		if (link != null)
		{
			if ((Object)(object)tempLinkObject != (Object)null)
			{
				Object.Destroy((Object)(object)tempLinkObject);
			}
			tempLinkObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/LinkObject"));
		}
		else if ((Object)(object)tempLinkObject != (Object)null)
		{
			Object.Destroy((Object)(object)tempLinkObject);
			tempLinkObject = null;
		}
	}

	public void SetTempObjectLink(ObjectLink link)
	{
		tempObjectLink = link;
	}

	public void ShowLink(Link link, GameObject linkGameObject)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.GameMode == MVGameMode.Edit)
		{
			Vector3 outputConnectorPos = MVGameController.Instance.WOCM.GetWorldObjectClient(link.outputWOID).GetOutputConnectorPos();
			Vector3 inputConnectorPos = MVGameController.Instance.WOCM.GetWorldObjectClient(link.inputWOID).GetInputConnectorPos();
			LineRenderer componentInChildren = linkGameObject.GetComponentInChildren<LineRenderer>();
			componentInChildren.SetPosition(0, outputConnectorPos);
			componentInChildren.SetPosition(1, inputConnectorPos);
			BoxCollider componentInChildren2 = ((Component)componentInChildren).gameObject.GetComponentInChildren<BoxCollider>();
			((Component)componentInChildren2).gameObject.transform.position = outputConnectorPos + (inputConnectorPos - outputConnectorPos) / 2f;
			Transform transform = ((Component)componentInChildren2).gameObject.transform;
			Vector3 val = inputConnectorPos - outputConnectorPos;
			transform.rotation = Quaternion.LookRotation(val.normalized);
			Vector3 val2 = inputConnectorPos - outputConnectorPos;
			float magnitude = val2.magnitude;
			magnitude -= 0.5f;
			magnitude = Mathf.Max(magnitude, 0.2f);
			((Component)componentInChildren2).transform.localScale = new Vector3(0.2f, 0.2f, magnitude);
			Material material = ((Component)componentInChildren2).gameObject.renderer.material;
			Vector3 val3 = inputConnectorPos - outputConnectorPos;
			material.mainTextureScale = new Vector2(val3.magnitude / 2f, 1f);
			if (link.isSet)
			{
				((Component)componentInChildren2).gameObject.renderer.material.color = Color.green;
				Vector2 mainTextureOffset = ((Component)componentInChildren2).gameObject.renderer.material.mainTextureOffset;
				mainTextureOffset.x -= Time.deltaTime * 1.5f;
				((Component)componentInChildren2).gameObject.renderer.material.mainTextureOffset = mainTextureOffset;
			}
			else
			{
				((Component)componentInChildren2).gameObject.renderer.material.color = Color.grey;
			}
		}
	}

	public void ShowObjectLink(ObjectLink objectLink, GameObject objectLinkGameObject)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.GameMode == MVGameMode.Edit)
		{
			Vector3 objectConnectorPos = MVGameController.Instance.WOCM.GetWorldObjectClient(objectLink.objectConnectorWOID).GetObjectConnectorPos();
			Vector3 val = MVGameController.Instance.WOCM.GetWorldObjectClient(objectLink.objectWOID).WorldPosition;
			if (MVGameController.Instance.WOCM.GetWorldObjectClient(objectLink.objectWOID) is MVCubeModelBase)
			{
				val = (MVGameController.Instance.WOCM.GetWorldObjectClient(objectLink.objectWOID) as MVCubeModelBase).GetWorldCenterPos();
			}
			LineRenderer componentInChildren = objectLinkGameObject.GetComponentInChildren<LineRenderer>();
			componentInChildren.SetPosition(0, objectConnectorPos);
			componentInChildren.SetPosition(1, val);
			BoxCollider componentInChildren2 = ((Component)componentInChildren).gameObject.GetComponentInChildren<BoxCollider>();
			((Component)componentInChildren2).gameObject.transform.position = objectConnectorPos + (val - objectConnectorPos) / 2f;
			Transform transform = ((Component)componentInChildren2).gameObject.transform;
			Vector3 val2 = val - objectConnectorPos;
			transform.rotation = Quaternion.LookRotation(val2.normalized);
			Vector3 val3 = val - objectConnectorPos;
			float magnitude = val3.magnitude;
			magnitude -= 0.5f;
			magnitude = Mathf.Max(magnitude, 0.2f);
			((Component)componentInChildren2).transform.localScale = new Vector3(0.2f, 0.2f, magnitude);
			Material material = ((Component)componentInChildren2).gameObject.renderer.material;
			Vector3 val4 = val - objectConnectorPos;
			material.mainTextureScale = new Vector2(val4.magnitude / 2f, 1f);
			((Component)componentInChildren2).gameObject.renderer.material.color = Color.yellow;
		}
	}

	public void DrawPendingLink(Link link)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector3 outputConnectorPos = MVGameController.Instance.WOCM.GetWorldObjectClient(link.outputWOID).GetOutputConnectorPos();
		Vector3 inputConnectorPos = MVGameController.Instance.WOCM.GetWorldObjectClient(link.inputWOID).GetInputConnectorPos();
		Color color = new Color(0f, 1f, 0f, 1f);
		linkLines.Enqueue(new LinkLine(outputConnectorPos, inputConnectorPos, color));
	}

	public void DrawPendingObjectLink(ObjectLink objectLink)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector3 objectConnectorPos = MVGameController.Instance.WOCM.GetWorldObjectClient(objectLink.objectConnectorWOID).GetObjectConnectorPos();
		Vector3 worldPosition = MVGameController.Instance.WOCM.GetWorldObjectClient(objectLink.objectWOID).WorldPosition;
		Color color = new Color(1f, 1f, 0f, 1f);
		linkLines.Enqueue(new LinkLine(objectConnectorPos, worldPosition, color));
	}

	private void DrawLine(Vector3 from, Vector3 to, Color color)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.GameMode != MVGameMode.Edit)
		{
			return;
		}
		try
		{
			if ((Object)(object)MVGameController.Instance.Game.CameraController != (Object)null)
			{
				Camera camera = ((Component)MVGameController.Instance.Game.CameraController).camera;
				if ((Object)(object)camera != (Object)null && (camera.cullingMask & (1 << LayerMask.NameToLayer("Logic"))) != 0)
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
			Debug.LogWarning((object)("LineDrawManager exception: " + ex));
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DrawLine(linkLine.startPos, linkLine.endPos, linkLine.color);
	}

	private static void CreateLineMaterial()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected Obj, but got Unknown
		if (!Object.op_Implicit((Object)(object)lineMaterial))
		{
			lineMaterial = new Material("Shader \"Lines/Colored Blended\" {SubShader { Pass {     Blend One OneMinusSrcAlpha     ZWrite Off Cull Off Fog { Mode Off }     BindChannels {      Bind \"vertex\", vertex Bind \"color\", color }} } }");
			((Object)lineMaterial).hideFlags = (HideFlags)13;
			((Object)lineMaterial.shader).hideFlags = (HideFlags)13;
		}
	}
}
