using UnityEngine;

[AddComponentMenu("UX/Elements/Button (icon)")]
public class UXIconButton : UXBaseButton
{
	public Color color = Color.white;

	protected override void Initialize()
	{
		base.Initialize();
		UXMouseOverColorFade component = GetComponent<UXMouseOverColorFade>();
		if (component != null)
		{
			component.materials.Add(GetComponent<Renderer>().material);
		}
		ColorIcon(color);
	}

	public void ColorIcon(Color color)
	{
		this.color = color;
		UXMouseOverColorFade component = GetComponent<UXMouseOverColorFade>();
		if (component != null)
		{
			component.SetNewColor(color);
		}
	}

	public void ChangeMaterial(Material material)
	{
		UXMouseOverColorFade component = GetComponent<UXMouseOverColorFade>();
		if (component != null)
		{
			component.materials.Remove(GetComponent<Renderer>().material);
		}
		GetComponent<Renderer>().material = material;
		if (component != null)
		{
			component.materials.Add(GetComponent<Renderer>().material);
			component.UpdateMaterials();
		}
	}
}
