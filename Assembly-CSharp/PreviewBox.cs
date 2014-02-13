using UnityEngine;

public class PreviewBox : MonoBehaviour
{
	private void Start()
	{
		((Component)this).gameObject.layer = LayerMask.NameToLayer("UIItems");
	}

	public void Show(string material, Vector3[] corners)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected Obj, but got Unknown
		MeshRenderer val = ((Component)this).gameObject.GetComponent<MeshRenderer>();
		if ((Object)(object)val == (Object)null)
		{
			val = ((Component)this).gameObject.AddComponent<MeshRenderer>();
		}
		((Renderer)val).material = (Material)Resources.Load(material);
		MeshFilter val2 = ((Component)this).gameObject.GetComponent<MeshFilter>();
		if ((Object)(object)val2 == (Object)null)
		{
			val2 = ((Component)this).gameObject.AddComponent<MeshFilter>();
		}
		((Object)((Renderer)val).material).hideFlags = (HideFlags)4;
		val2.mesh.Clear();
		SharedCubeFunctions.AddCubeMeshCubeLines(val2.mesh, corners, 0.2f);
	}

	public void DestroyBox()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
