using System;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverColorFade))]
[AddComponentMenu("UX/Elements/Button")]
public class UXButton : MonoBehaviour
{
	public delegate void OnClickDelegate();

	public float width = 11f;

	public float height = 2f;

	public float insertLow = 2f;

	public float insertHigh = 2f;

	public string text;

	public float uvUnit = 1f;

	public GameObject textPrefab;

	public Color color = Color.white;

	public float textAngleDelta;

	public UXHorizontal horizontalAlign;

	public UXVertical verticalAlign = UXVertical.Bottom;

	public OnClickDelegate OnClick;

	private UXText uiText;

	private float BUTTON_TEXT_Z = -1f;

	private Vector3 center;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			uiText.Text = text;
		}
	}

	public UXButton()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Start()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		center = new Vector3(UXEnums.GetRatio(horizontalAlign) * width, UXEnums.GetRatio(verticalAlign) * height, 0f);
		Mesh mesh = BuildMesh();
		((Component)this).gameObject.AddComponent<MeshFilter>().mesh = mesh;
		((Component)this).gameObject.AddComponent<BoxCollider>();
		Object val = Object.Instantiate((Object)(object)textPrefab, Vector3.zero, Quaternion.identity);
		uiText = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
		((Component)uiText).transform.parent = ((Component)this).transform;
		((Component)uiText).transform.localPosition = new Vector3(width / 2f, height / 2f, BUTTON_TEXT_Z) - center;
		((Component)uiText).transform.localScale = Vector3.one;
		((Component)uiText).transform.localRotation = Quaternion.Euler(0f, 0f, textAngleDelta);
		Text = text;
		UXMouseClickObject component = ((Component)this).GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, new UXMouseClickObject.OnClickDelegate(HandleOnClick));
		UXMouseOverColorFade component2 = ((Component)this).GetComponent<UXMouseOverColorFade>();
		component2.materials.Add(((Component)this).renderer.material);
	}

	public void HandleOnClick(UXMouseClickObject mouseClickObject, Vector3 mousePositionWorld)
	{
		NotifyOnClick();
	}

	public void FireOnClick()
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

	public Mesh BuildMesh()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected Obj, but got Unknown
		Vector3[] array = new Vector3[4];
		int[] array2 = new int[6];
		Vector2[] array3 = new Vector2[array.Length];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, 0f, 0f);
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(width, 0f, 0f);
		ref Vector3 reference3 = ref array[2];
		reference3 = new Vector3(width - insertHigh, height, 0f);
		ref Vector3 reference4 = ref array[3];
		reference4 = new Vector3(insertLow, height, 0f);
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector2 reference5 = ref array3[i];
			reference5 = new Vector2(array[i].x / uvUnit, array[i].y / uvUnit);
		}
		for (int j = 0; j < array.Length; j++)
		{
			ref Vector3 reference6 = ref array[j];
			reference6 = array[j] - center;
		}
		array2[0] = 0;
		array2[1] = 2;
		array2[2] = 1;
		array2[3] = 0;
		array2[4] = 3;
		array2[5] = 2;
		Mesh val = new Mesh();
		((Object)val).name = "UIButton Mesh";
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
