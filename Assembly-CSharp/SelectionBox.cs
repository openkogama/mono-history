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
		((Component)this).gameObject.layer = LayerMask.NameToLayer("UIItems");
	}

	public void Init(IModelingConstraint constraint, string layer = "UIItems")
	{
	}

	private void Update()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		if (isFadingOut)
		{
			if (Time.time - fadeOutBeginTime > fadeOutTime)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
				return;
			}
			float num = (fadeOutTime - (Time.time - fadeOutBeginTime)) / fadeOutTime * baseAlpha;
			if (currentAlpha > num)
			{
				Color color = ((Component)this).gameObject.renderer.material.GetColor("_Color");
				color.a = num;
				currentAlpha = num;
				((Component)this).gameObject.renderer.material.SetColor("_Color", color);
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
				Color color2 = ((Component)this).gameObject.renderer.material.GetColor("_Color");
				color2.a = num2;
				currentAlpha = num2;
				((Component)this).gameObject.renderer.material.SetColor("_Color", color2);
			}
		}
	}

	public void FadeIn(float fadeInTime, string material, Vector3[] corners)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected Obj, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		isFadingOut = false;
		isFadingIn = true;
		this.fadeInTime = fadeInTime;
		fadeInBeginTime = Time.time;
		MeshRenderer val = ((Component)this).gameObject.GetComponent<MeshRenderer>();
		float a = 0f;
		if ((Object)(object)val == (Object)null)
		{
			val = ((Component)this).gameObject.AddComponent<MeshRenderer>();
		}
		else
		{
			a = currentAlpha;
		}
		((Renderer)val).material = (Material)Resources.Load(material);
		MeshFilter val2 = ((Component)this).gameObject.GetComponent<MeshFilter>();
		if ((Object)(object)val2 == (Object)null)
		{
			val2 = ((Component)this).gameObject.AddComponent<MeshFilter>();
		}
		((Object)((Renderer)val).material).hideFlags = (HideFlags)4;
		Color color = ((Renderer)val).material.GetColor("_Color");
		baseAlpha = color.a;
		color.a = a;
		((Renderer)val).material.SetColor("_Color", color);
		val2.mesh.Clear();
		SharedCubeFunctions.AddCubeMesh(val2.mesh, corners, insideOut: false);
		SharedCubeFunctions.AddCubeMeshCubeLines(val2.mesh, corners, 0.01f);
	}

	public void FadeOutDestroy(float fadeOutTime)
	{
		this.fadeOutTime = fadeOutTime;
		isFadingOut = true;
		fadeOutBeginTime = Time.time;
	}
}
