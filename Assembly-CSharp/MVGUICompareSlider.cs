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
			if (ratioPlane != null && backgroundPlane != null)
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
			if (ratioPlane != null && backgroundPlane != null)
			{
				UpdateRatioDisplay();
			}
		}
	}

	public void Start()
	{
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
		ratioPlane.transform.localPosition = new Vector3(0f, 0f, -0.01f);
		UpdateRatioDisplay();
	}

	private UXPlane BuildPlane(string name)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		UXPlane uXPlane = gameObject.AddComponent<UXPlane>();
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
		if (!(backgroundPlane == null) && !(ratioPlane == null))
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
		if (ratio == 0f)
		{
			ratioPlane.SetVisible(visible: false);
		}
		ratioPlane.SetSize(Width * ratio, Height);
		requiredRatioPlane.transform.localPosition = new Vector3(Width * requiredRatio, 0f, -0.2f);
	}
}
