using MV.WorldObject;
using UnityEngine;

public class ObjectEnabler : MonoBehaviour
{
	public MVObjectEnabler woObjectEnabler;

	private bool isEnabled;

	private float currentAlpha;

	private Material blah;

	public bool IsDrawingEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			isEnabled = value;
		}
	}

	private void DrawObject(MeshFilter[] previewMeshes)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		foreach (MeshFilter val in previewMeshes)
		{
			for (int j = 0; j < val.sharedMesh.subMeshCount; j++)
			{
				Graphics.DrawMesh(val.sharedMesh, ((Component)val).transform.localToWorldMatrix, blah, LayerMask.NameToLayer("Default"), Camera.main, j);
			}
		}
	}

	private void Awake()
	{
		Object val = Resources.Load("Materials/ObjectHidden", typeof(Material));
		Material val2 = (Material)(object)((val is Material) ? val : null);
		Object val3 = Object.Instantiate((Object)(object)val2);
		blah = (Material)(object)((val3 is Material) ? val3 : null);
	}

	private void OnDestroy()
	{
		Object.Destroy((Object)(object)blah);
	}

	private void Update()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		float num = ((!isEnabled) ? 0.15f : 0.05f);
		currentAlpha = Mathf.Lerp(currentAlpha, num, Time.deltaTime * 1f);
		Color color = blah.color;
		color.a = currentAlpha;
		blah.color = color;
		if (!(isEnabled | (currentAlpha != num)) || !woObjectEnabler.ShowingOutline)
		{
			return;
		}
		foreach (ObjectLink objectLinkRef in woObjectEnabler.ObjectLinkRefs)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(objectLinkRef.objectWOID);
			if (worldObjectClient is MVCubeModelInstance)
			{
				MVCubeModelInstance mVCubeModelInstance = worldObjectClient as MVCubeModelInstance;
				DrawObject(mVCubeModelInstance.MeshFilters);
			}
		}
	}
}
