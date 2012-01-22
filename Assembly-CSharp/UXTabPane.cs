using System;
using UnityEngine;

[RequireComponent(typeof(UXMouseClickObject))]
[AddComponentMenu("UX/Elements/Tab Pane")]
public class UXTabPane : MonoBehaviour
{
	public delegate void OnTabClickDelegate(int tabid);

	public int tabId;

	public string headerText;

	public GameObject textPrefab;

	private UXText uiHeaderText;

	public float headerTextInsert = 0.8f;

	public UXGroup tabGroup;

	private float BUTTON_TEXT_Z = -1f;

	private float uvUnit = 1f;

	private float headerHeight;

	private float headerInsert;

	public float headerWidth;

	private Material selected;

	public Material notSelected;

	private Vector3 center;

	public OnTabClickDelegate OnTabClick;

	public void Start()
	{
		selected = ((Component)this).renderer.material;
	}

	public void Show()
	{
		((Component)this).renderer.material = selected;
		tabGroup.Visible = true;
	}

	public void Hide()
	{
		((Component)this).renderer.material = notSelected;
		tabGroup.Visible = false;
	}

	public float GetTabInsert()
	{
		return headerInsert;
	}

	public void BuildTab(float headerInsert, float headerHeight)
	{
		this.headerInsert = headerInsert;
		this.headerHeight = headerHeight;
		GenerateTab();
	}

	private void GenerateTab()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		center = new Vector3(UXEnums.GetRatio(UXHorizontal.Left) * headerWidth, UXEnums.GetRatio(UXVertical.Top) * headerHeight, 0f);
		Object val = Object.Instantiate((Object)(object)textPrefab, Vector3.zero, Quaternion.identity);
		uiHeaderText = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
		((Component)uiHeaderText).transform.parent = ((Component)this).transform;
		((Component)uiHeaderText).transform.localScale = Vector3.one;
		uiHeaderText.verticalAlign = UXVertical.Middle;
		uiHeaderText.horizontalAlign = UXHorizontal.Left;
		((Component)uiHeaderText).transform.localPosition = new Vector3(headerInsert + headerTextInsert, headerHeight / 2f, BUTTON_TEXT_Z) - center;
		uiHeaderText.Text = headerText;
		MeshFilter val2 = ((Component)this).gameObject.AddComponent<MeshFilter>();
		val2.mesh = BuildMesh();
		if ((Object)(object)((Component)this).GetComponent<BoxCollider>() == (Object)null)
		{
			((Component)this).gameObject.AddComponent<BoxCollider>();
		}
		UXMouseClickObject component = ((Component)this).GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, new UXMouseClickObject.OnClickDelegate(HandleOnClick));
	}

	private Mesh BuildMesh()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected Obj, but got Unknown
		Vector3[] array = new Vector3[4];
		int[] array2 = new int[6];
		Vector2[] array3 = new Vector2[array.Length];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(headerInsert, headerHeight, 0f) - center;
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(headerInsert + headerWidth, headerHeight, 0f) - center;
		ref Vector3 reference3 = ref array[2];
		reference3 = new Vector3(headerInsert, 0f, 0f) - center;
		ref Vector3 reference4 = ref array[3];
		reference4 = new Vector3(headerInsert + headerWidth, 0f, 0f) - center;
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector2 reference5 = ref array3[i];
			reference5 = new Vector2(array[i].x / uvUnit, array[i].y / uvUnit);
		}
		array2[0] = 2;
		array2[1] = 0;
		array2[2] = 1;
		array2[3] = 2;
		array2[4] = 1;
		array2[5] = 3;
		Mesh val = new Mesh();
		((Object)val).name = "UITabPane Mesh";
		val.vertices = array;
		val.uv = array3;
		val.triangles = array2;
		val.RecalculateNormals();
		val.RecalculateBounds();
		return val;
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
		if (OnTabClick != null)
		{
			OnTabClick(tabId);
		}
	}
}
