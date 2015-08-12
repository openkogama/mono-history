using UnityEngine;

public class SelectionBox : MonoBehaviour
{
	private float fadeOutTime = 10f;

	private float fadeOutBeginTime;

	private bool isFadingOut;

	private float fadeInTime = 10f;

	private float fadeInBeginTime;

	private bool isFadingIn;

	private float baseAlpha;

	private float currentAlpha;

	private void Start()
	{
		gameObject.layer = LayerMask.NameToLayer("UIItems");
	}

	public void Init(IModelingConstraint constraint, string layer = "UIItems")
	{
	}

	private void Update()
	{
		if (isFadingOut)
		{
			if (Time.time - fadeOutBeginTime > fadeOutTime)
			{
				Object.Destroy(gameObject);
				return;
			}
			float num = (fadeOutTime - (Time.time - fadeOutBeginTime)) / fadeOutTime * baseAlpha;
			if (currentAlpha > num)
			{
				Color color = gameObject.GetComponent<Renderer>().material.GetColor("_Color");
				color.a = num;
				currentAlpha = num;
				gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
			}
		}
		else if (isFadingIn)
		{
			float num2 = 0f;
			if (Time.time - fadeInBeginTime <= fadeInTime)
			{
				num2 = (Time.time - fadeInBeginTime) / fadeInTime * baseAlpha;
			}
			else
			{
				num2 = baseAlpha;
				isFadingIn = false;
			}
			if (currentAlpha < num2)
			{
				Color color2 = gameObject.GetComponent<Renderer>().material.GetColor("_Color");
				color2.a = num2;
				currentAlpha = num2;
				gameObject.GetComponent<Renderer>().material.SetColor("_Color", color2);
			}
		}
	}

	public void FadeIn(float fadeInTime, string material, Vector3[] corners)
	{
		isFadingOut = false;
		isFadingIn = true;
		this.fadeInTime = fadeInTime;
		fadeInBeginTime = Time.time;
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		float a = 0f;
		if (meshRenderer == null)
		{
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
		}
		else
		{
			a = currentAlpha;
		}
		meshRenderer.material = (Material)Resources.Load(material);
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}
		meshRenderer.material.hideFlags = HideFlags.DontSave;
		Color color = meshRenderer.material.GetColor("_Color");
		baseAlpha = color.a;
		color.a = a;
		meshRenderer.material.SetColor("_Color", color);
		meshFilter.mesh.Clear();
		SharedCubeFunctions.AddCubeMesh(meshFilter.mesh, corners, insideOut: false);
		SharedCubeFunctions.AddCubeMeshCubeLines(meshFilter.mesh, corners, 0.01f);
	}

	public void FadeOutDestroy(float fadeOutTime)
	{
		this.fadeOutTime = fadeOutTime;
		isFadingOut = true;
		fadeOutBeginTime = Time.time;
	}
}
