using UnityEngine;

public class MVGUICompareSlider : UXGUIElement
{
	[SerializeField]
	private float ratio;

	[SerializeField]
	private float requiredRatio;

	public Color backgroundColor;

	public Color ratioColor;

	public Color requiredRatioColor;

	public Material planeMaterial;

	private UXPlane backgroundPlane;

	private UXPlane ratioPlane;

	private UXPlane requiredRatioPlane;

	public float Ratio
	{
		get
		{
			return ratio;
		}
		set
		{
			ratio = Mathf.Clamp01(value);
			if ((Object)(object)ratioPlane != (Object)null && (Object)(object)backgroundPlane != (Object)null)
			{
				UpdateRatioDisplay();
			}
		}
	}

	public float RequiredRatio
	{
		get
		{
			return requiredRatio;
		}
		set
		{
			requiredRatio = Mathf.Clamp01(value);
			if ((Object)(object)ratioPlane != (Object)null && (Object)(object)backgroundPlane != (Object)null)
			{
				UpdateRatioDisplay();
			}
		}
	}

	public void Start()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		backgroundPlane = BuildPlane("BackgroundPlane");
		ratioPlane = BuildPlane("RatioPlane");
		requiredRatioPlane = BuildPlane("RequiredRatioPlane");
		requiredRatioPlane.SetSize(0.1f, Height * 1.4f);
		backgroundPlane.SetColor(backgroundColor, string.Empty);
		ratioPlane.SetColor(ratioColor, string.Empty);
		requiredRatioPlane.SetColor(requiredRatioColor, string.Empty);
		backgroundPlane.SetVisible(Visible);
		ratioPlane.SetVisible(Visible && ratio > 0f);
		requiredRatioPlane.SetVisible(Visible && requiredRatio > 0f);
		backgroundPlane.SetSize(Width, Height);
		((Component)ratioPlane).transform.localPosition = new Vector3(0f, 0f, -0.01f);
		UpdateRatioDisplay();
	}

	private UXPlane BuildPlane(string name)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)this).transform;
		val.transform.localPosition = Vector3.zero;
		val.transform.localScale = Vector3.one;
		UXPlane uXPlane = val.AddComponent<UXPlane>();
		uXPlane.uses9PatchMaterial = uses9PatchMaterial;
		uXPlane.SetMaterial(planeMaterial);
		uXPlane.SetAlignment(UXHorizontal.Left, UXVertical.Middle);
		return uXPlane;
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		backgroundPlane.SetSize(Width, Height);
		UpdateRatioDisplay();
	}

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		if (!((Object)(object)backgroundPlane == (Object)null) && !((Object)(object)ratioPlane == (Object)null))
		{
			backgroundPlane.SetVisible(visible);
			ratioPlane.SetVisible(visible && ratio > 0f);
			requiredRatioPlane.SetVisible(Visible && requiredRatio > 0f);
		}
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		backgroundPlane.SetAlpha(alpha, materialProperty);
		ratioPlane.SetAlpha(alpha, materialProperty);
	}

	private void UpdateRatioDisplay()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (ratio == 0f)
		{
			ratioPlane.SetVisible(visible: false);
		}
		ratioPlane.SetSize(Width * ratio, Height);
		((Component)requiredRatioPlane).transform.localPosition = new Vector3(Width * requiredRatio, 0f, -0.2f);
	}
}
