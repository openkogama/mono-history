using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UX/Elements/Tab Window")]
public class UXTabWindow : MonoBehaviour
{
	public delegate void OnTabSelectedDelegate(int tabId);

	private float uvUnit = 1f;

	private Vector3 center;

	public List<UXTabPane> tabs;

	public float height = 15f;

	public float width = 10f;

	public float headerHeight = 2f;

	public bool adjustWidthToTabs = true;

	public float tabDistance = 0.3f;

	public OnTabSelectedDelegate OnTabSelect;

	public void Start()
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		UXTabPane uXTabPane = null;
		float num = 0f;
		foreach (UXTabPane tab in tabs)
		{
			tab.Hide();
			float headerInsert = ((!((Object)(object)uXTabPane != (Object)null)) ? 0f : (uXTabPane.GetTabInsert() + uXTabPane.headerWidth + tabDistance));
			tab.BuildTab(headerInsert, headerHeight);
			tab.OnTabClick = (UXTabPane.OnTabClickDelegate)Delegate.Combine(tab.OnTabClick, new UXTabPane.OnTabClickDelegate(OnTabClick));
			uXTabPane = tab;
			if (tabs.IndexOf(tab) == 0)
			{
				tab.Show();
			}
			num += tab.headerWidth;
		}
		num += tabDistance * (float)(tabs.Count - 1);
		if (adjustWidthToTabs)
		{
			width = num;
		}
		center = new Vector3(UXEnums.GetRatio(UXHorizontal.Left) * width, UXEnums.GetRatio(UXVertical.Top) * height, 0f);
		MeshFilter val = ((Component)this).gameObject.AddComponent<MeshFilter>();
		val.mesh = BuildMesh();
	}

	private Mesh BuildMesh()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected Obj, but got Unknown
		Vector3[] array = new Vector3[4];
		int[] array2 = new int[6];
		Vector2[] array3 = new Vector2[array.Length];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, height - headerHeight, 0f) - center;
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(width, height - headerHeight, 0f) - center;
		ref Vector3 reference3 = ref array[2];
		reference3 = new Vector3(0f, 0f - headerHeight, 0f) - center;
		ref Vector3 reference4 = ref array[3];
		reference4 = new Vector3(width, 0f - headerHeight, 0f) - center;
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
		((Object)val).name = "UITabWindow Mesh";
		val.vertices = array;
		val.uv = array3;
		val.triangles = array2;
		val.RecalculateNormals();
		val.RecalculateBounds();
		return val;
	}

	public void OnTabClick(int tabId)
	{
		foreach (UXTabPane tab in tabs)
		{
			tab.Hide();
			if (tab.tabId == tabId)
			{
				tab.Show();
			}
		}
		if (OnTabSelect != null)
		{
			OnTabSelect(tabId);
		}
	}
}
