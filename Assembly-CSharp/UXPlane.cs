using UnityEngine;

public class UXPlane : UXGUIElement, IUXContainer
{
	[SerializeField]
	private bool _applyColorOnAwake;

	[SerializeField]
	private Color _color = Color.white;

	public string materialProperty = "_MainColor";

	public UXPlane()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		if ((Object)(object)((Component)this).collider != (Object)null)
		{
			((Component)this).collider.enabled = visible;
		}
		if ((Object)(object)((Component)this).renderer != (Object)null)
		{
			((Component)this).renderer.enabled = visible;
		}
	}

	public void SetColor(Color color, string colorProperty = "")
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (colorProperty.Equals(string.Empty))
		{
			colorProperty = materialProperty;
		}
		_color = color;
		((Component)this).renderer.material.SetColor(colorProperty, color);
	}

	public override void SetAlpha(float alpha, string colorProperty = "")
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (colorProperty.Equals(string.Empty))
		{
			colorProperty = materialProperty;
		}
		_color = ((Component)this).renderer.material.GetColor(colorProperty);
		_color.a = alpha;
		SetColor(_color, colorProperty);
	}

	public override void Awake()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.Awake();
		UXUtils.AddComponentIfNotExists<MeshFilter>(((Component)this).gameObject).mesh = BuildMesh();
		if (_applyColorOnAwake)
		{
			SetColor(_color, materialProperty);
		}
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		UXUtils.AddComponentIfNotExists<MeshFilter>(((Component)this).gameObject).mesh = BuildMesh();
	}
}
