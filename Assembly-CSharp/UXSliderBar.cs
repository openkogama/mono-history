using UnityEngine;

[AddComponentMenu("UX/Elements/Subparts/Slider bar")]
[RequireComponent(typeof(UXMouseClickObject))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class UXSliderBar : MonoBehaviour
{
	public int width;

	public int height;

	public UXSlideOrientation slideOrientation;

	private void Awake()
	{
		Mesh mesh = BuildMesh();
		((Component)this).GetComponent<MeshFilter>().mesh = mesh;
		((Component)this).gameObject.AddComponent<BoxCollider>();
	}

	private Mesh BuildMesh()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected Obj, but got Unknown
		Vector3[] array = new Vector3[4];
		int[] array2 = new int[6];
		Vector2[] array3 = new Vector2[array.Length];
		if (slideOrientation == UXSlideOrientation.Horizontal)
		{
			ref Vector3 reference = ref array[0];
			reference = new Vector3(0f, 0f, 0f);
			ref Vector3 reference2 = ref array[1];
			reference2 = new Vector3((float)width, 0f, 0f);
			ref Vector3 reference3 = ref array[2];
			reference3 = new Vector3((float)width, (float)(-height), 0f);
			ref Vector3 reference4 = ref array[3];
			reference4 = new Vector3(0f, (float)(-height), 0f);
		}
		else
		{
			ref Vector3 reference5 = ref array[0];
			reference5 = new Vector3((float)height, 0f, 0f);
			ref Vector3 reference6 = ref array[1];
			reference6 = new Vector3((float)height, (float)(-width), 0f);
			ref Vector3 reference7 = ref array[2];
			reference7 = new Vector3(0f, (float)(-width), 0f);
			ref Vector3 reference8 = ref array[3];
			reference8 = new Vector3(0f, 0f, 0f);
		}
		ref Vector2 reference9 = ref array3[0];
		reference9 = new Vector2(0f, 0f);
		ref Vector2 reference10 = ref array3[1];
		reference10 = new Vector2(1f, 0f);
		ref Vector2 reference11 = ref array3[2];
		reference11 = Vector2.op_Implicit(new Vector3(1f, 1f));
		ref Vector2 reference12 = ref array3[3];
		reference12 = Vector2.op_Implicit(new Vector3(0f, 1f));
		array2[0] = 0;
		array2[1] = 1;
		array2[2] = 2;
		array2[3] = 0;
		array2[4] = 2;
		array2[5] = 3;
		Mesh val = new Mesh();
		((Object)val).name = "UISliderBar Mesh";
		val.vertices = array;
		val.uv = array3;
		val.triangles = array2;
		val.RecalculateNormals();
		val.RecalculateBounds();
		return val;
	}
}
