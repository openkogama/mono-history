using UnityEngine;

[AddComponentMenu("UX/Window")]
public class UXWindow : MonoBehaviour
{
	public float width = 33f;

	public float height = 22f;

	public float headerInsert = 2f;

	public float headerHeight = 2f;

	public float headerWidth = 11f;

	public float headerTextInsert = 0.8f;

	public string headerText;

	public float cornerInsert = 4f;

	public float uvUnit = 1f;

	public UXHorizontal horizontalAlign = UXHorizontal.Center;

	public UXVertical verticalAlign = UXVertical.Middle;

	private UXText uiHeaderText;

	public GameObject textPrefab;

	private float BUTTON_TEXT_Z = -1f;

	private Vector3 center;

	public void Start()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		center = new Vector3(UXEnums.GetRatio(horizontalAlign) * width, UXEnums.GetRatio(verticalAlign) * height, 0f);
		MeshFilter val = ((Component)this).gameObject.AddComponent<MeshFilter>();
		val.mesh = BuildMesh();
		Object val2 = Object.Instantiate((Object)(object)textPrefab, Vector3.zero, Quaternion.identity);
		uiHeaderText = ((GameObject)((val2 is GameObject) ? val2 : null)).GetComponent<UXText>();
		((Component)uiHeaderText).transform.parent = ((Component)this).transform;
		((Component)uiHeaderText).transform.localScale = Vector3.one;
		uiHeaderText.verticalAlign = UXVertical.Middle;
		uiHeaderText.horizontalAlign = UXHorizontal.Left;
		((Component)uiHeaderText).transform.localPosition = new Vector3(headerInsert + headerTextInsert, height + headerHeight / 2f, BUTTON_TEXT_Z) - center;
		uiHeaderText.Text = headerText;
	}

	private Mesh BuildMesh()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected Obj, but got Unknown
		Vector3[] array = new Vector3[10];
		int[] array2 = new int[18];
		Vector2[] array3 = new Vector2[array.Length];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, height, 0f) - center;
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(width, height, 0f) - center;
		ref Vector3 reference3 = ref array[2];
		reference3 = new Vector3(0f, cornerInsert, 0f) - center;
		ref Vector3 reference4 = ref array[3];
		reference4 = new Vector3(width, cornerInsert, 0f) - center;
		ref Vector3 reference5 = ref array[4];
		reference5 = new Vector3(0f, 0f, 0f) - center;
		ref Vector3 reference6 = ref array[5];
		reference6 = new Vector3(width - cornerInsert, 0f, 0f) - center;
		ref Vector3 reference7 = ref array[6];
		reference7 = new Vector3(headerInsert, height + headerHeight, 0f) - center;
		ref Vector3 reference8 = ref array[7];
		reference8 = new Vector3(headerInsert + headerWidth, height + headerHeight, 0f) - center;
		ref Vector3 reference9 = ref array[8];
		reference9 = new Vector3(headerInsert, height, 0f) - center;
		ref Vector3 reference10 = ref array[9];
		reference10 = new Vector3(headerInsert + headerWidth + headerHeight, height, 0f) - center;
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector2 reference11 = ref array3[i];
			reference11 = new Vector2(array[i].x / uvUnit, array[i].y / uvUnit);
		}
		array2[0] = 2;
		array2[1] = 0;
		array2[2] = 1;
		array2[3] = 2;
		array2[4] = 1;
		array2[5] = 3;
		array2[6] = 4;
		array2[7] = 2;
		array2[8] = 3;
		array2[9] = 4;
		array2[10] = 3;
		array2[11] = 5;
		array2[12] = 8;
		array2[13] = 6;
		array2[14] = 7;
		array2[15] = 8;
		array2[16] = 7;
		array2[17] = 9;
		Mesh val = new Mesh();
		((Object)val).name = "UIWindow Mesh";
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
