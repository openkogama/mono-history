using System;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverColorFade))]
[RequireComponent(typeof(UXMouseClickObject))]
[AddComponentMenu("UX/Elements/Button (icon)")]
public class UXIconButton : MonoBehaviour
{
	public delegate void OnClickDelegate();

	public OnClickDelegate OnClick;

	public float width = 5f;

	public float height = 5f;

	public UXHorizontal horizontalAlign;

	public UXVertical verticalAlign = UXVertical.Bottom;

	public bool clickFallThrough;

	private Vector3 center;

	public void Awake()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		center = new Vector3(UXEnums.GetRatio(horizontalAlign) * width, UXEnums.GetRatio(verticalAlign) * height, 0f);
		Mesh mesh = BuildMesh();
		MeshFilter val = ((Component)this).gameObject.AddComponent<MeshFilter>();
		val.mesh = mesh;
		if ((Object)(object)((Component)this).GetComponent<BoxCollider>() == (Object)null)
		{
			((Component)this).gameObject.AddComponent<BoxCollider>();
		}
		UXMouseOverColorFade component = ((Component)this).gameObject.GetComponent<UXMouseOverColorFade>();
		component.materials.Add(((Component)this).renderer.material);
		if (!clickFallThrough)
		{
			UXMouseClickObject component2 = ((Component)this).gameObject.GetComponent<UXMouseClickObject>();
			component2.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component2.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		}
		UXMouseClickObject component3 = ((Component)this).gameObject.GetComponent<UXMouseClickObject>();
		component3.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component3.OnClick, new UXMouseClickObject.OnClickDelegate(OnObjectClick));
	}

	private void OnObjectClick(UXMouseClickObject clickObject, Vector3 mousePositionWorld)
	{
		NotifyOnClick();
	}

	private void NotifyOnClick()
	{
		if (OnClick != null)
		{
			OnClick();
		}
	}

	private Mesh BuildMesh()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected Obj, but got Unknown
		Vector3[] array = new Vector3[4];
		int[] array2 = new int[6];
		Vector2[] array3 = new Vector2[array.Length];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, 0f, 0f) - center;
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(width, 0f, 0f) - center;
		ref Vector3 reference3 = ref array[2];
		reference3 = new Vector3(width, height, 0f) - center;
		ref Vector3 reference4 = ref array[3];
		reference4 = new Vector3(0f, height, 0f) - center;
		ref Vector2 reference5 = ref array3[0];
		reference5 = new Vector2(0f, 0f);
		ref Vector2 reference6 = ref array3[1];
		reference6 = new Vector2(1f, 0f);
		ref Vector2 reference7 = ref array3[2];
		reference7 = new Vector2(1f, 1f);
		ref Vector2 reference8 = ref array3[3];
		reference8 = new Vector2(0f, 1f);
		array2[0] = 0;
		array2[1] = 2;
		array2[2] = 1;
		array2[3] = 0;
		array2[4] = 3;
		array2[5] = 2;
		Mesh val = new Mesh();
		((Object)val).name = "UIIconButton Mesh";
		val.vertices = array;
		val.uv = array3;
		val.triangles = array2;
		val.RecalculateNormals();
		val.RecalculateBounds();
		return val;
	}

	public void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.grey;
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		center = new Vector3(UXEnums.GetRatio(horizontalAlign) * width, UXEnums.GetRatio(verticalAlign) * height, 0f);
		Vector3 val = localToWorldMatrix.MultiplyPoint(new Vector3(0f, 0f, 0f) - center);
		Vector3 val2 = localToWorldMatrix.MultiplyPoint(new Vector3(width, 0f, 0f) - center);
		Vector3 val3 = localToWorldMatrix.MultiplyPoint(new Vector3(width, height, 0f) - center);
		Vector3 val4 = localToWorldMatrix.MultiplyPoint(new Vector3(0f, height, 0f) - center);
		Gizmos.DrawLine(val, val2);
		Gizmos.DrawLine(val2, val3);
		Gizmos.DrawLine(val3, val4);
		Gizmos.DrawLine(val4, val);
	}
}
