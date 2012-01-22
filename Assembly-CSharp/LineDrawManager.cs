using System;
using MV.WorldObject;
using UnityEngine;

public class LineDrawManager : MonoBehaviour
{
	private static Material lineMaterial;

	private static LineDrawManager instance;

	private Link tempLink;

	private GameObject tempLinkObject;

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

	private void Start()
	{
		CreateLineMaterial();
	}

	private void Update()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		foreach (Link value in MVGameController.Instance.WOCM.Links.Values)
		{
			Vector3 outputConnectorPos = MVGameController.Instance.WOCM.WorldObjects[value.outputWOID].GetOutputConnectorPos();
			Vector3 inputConnectorPos = MVGameController.Instance.WOCM.WorldObjects[value.inputWOID].GetInputConnectorPos();
			LineRenderer componentInChildren = MVGameController.Instance.WOCM.LinkObjects[value.id].GetComponentInChildren<LineRenderer>();
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
			if (value.isSet)
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

	private void FindPointForTempLink()
	{
	}

	private void OnPostRender()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!((Object)(object)MVGameController.Instance.WOCM.WeCamera != (Object)null))
			{
				return;
			}
			Camera camera = ((Component)MVGameController.Instance.WOCM.WeCamera).camera;
			if (!((Object)(object)camera != (Object)null) || (camera.cullingMask & (1 << LayerMask.NameToLayer("Logic"))) == 0)
			{
				return;
			}
			lineMaterial.SetPass(0);
			GL.Begin(1);
			GL.Color(new Color(0f, 1f, 0f, 1f));
			foreach (Link pendingLink in MVGameController.Instance.WOCM.PendingLinks)
			{
				Vector3 outputConnectorPos = MVGameController.Instance.WOCM.WorldObjects[pendingLink.outputWOID].GetOutputConnectorPos();
				Vector3 inputConnectorPos = MVGameController.Instance.WOCM.WorldObjects[pendingLink.inputWOID].GetInputConnectorPos();
				GL.Vertex3(outputConnectorPos.x, outputConnectorPos.y, outputConnectorPos.z);
				GL.Vertex3(inputConnectorPos.x, inputConnectorPos.y, inputConnectorPos.z);
			}
			if (tempLink != null)
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				if (tempLink.outputWOID > 0)
				{
					zero = MVGameController.Instance.WOCM.WorldObjects[tempLink.outputWOID].GetOutputConnectorPos();
					GL.Color(new Color(0f, 0f, 1f, 1f));
				}
				else
				{
					zero = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
				}
				if (tempLink.inputWOID > 0)
				{
					zero2 = MVGameController.Instance.WOCM.WorldObjects[tempLink.inputWOID].GetInputConnectorPos();
					GL.Color(new Color(1f, 0f, 0f, 1f));
				}
				else
				{
					zero2 = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
				}
				GL.Vertex3(zero.x, zero.y, zero.z);
				GL.Vertex3(zero2.x, zero2.y, zero2.z);
			}
			GL.End();
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)("LineDrawManager exception: " + ex));
		}
	}
}
