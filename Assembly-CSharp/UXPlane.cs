using UnityEngine;

public class UXPlane : UXGUIElement, IUXContainer
{
	[SerializeField]
	private bool _applyColorOnAwake;

	[SerializeField]
	private Color _color = Color.white;

	public string materialProperty = "_MainColor";

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = visible;
		}
		if (GetComponent<Renderer>() != null)
		{
			GetComponent<Renderer>().enabled = visible;
		}
	}

	public void SetColor(Color color, string colorProperty = "")
	{
		if (colorProperty.Equals(string.Empty))
		{
			colorProperty = materialProperty;
		}
		_color = color;
		GetComponent<Renderer>().material.SetColor(colorProperty, color);
	}

	public override void SetAlpha(float alpha, string colorProperty = "")
	{
		if (colorProperty.Equals(string.Empty))
		{
			colorProperty = materialProperty;
		}
		_color = GetComponent<Renderer>().material.GetColor(colorProperty);
		_color.a = alpha;
		SetColor(_color, colorProperty);
	}

	public override void Awake()
	{
		base.Awake();
		BuildMesh(UXUtils.AddComponentIfNotExists<MeshFilter>(gameObject).mesh);
		if (_applyColorOnAwake)
		{
			SetColor(_color, materialProperty);
		}
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		BuildMesh(UXUtils.AddComponentIfNotExists<MeshFilter>(gameObject).mesh);
	}
}
