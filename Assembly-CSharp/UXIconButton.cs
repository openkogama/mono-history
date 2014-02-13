using UnityEngine;

[AddComponentMenu("UX/Elements/Button (icon)")]
public class UXIconButton : UXBaseButton
{
	public Color color = Color.white;

	public UXIconButton()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	protected override void Initialize()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		UXMouseOverColorFade component = ((Component)this).GetComponent<UXMouseOverColorFade>();
		if ((Object)(object)component != (Object)null)
		{
			component.materials.Add(((Component)this).renderer.material);
		}
		ColorIcon(color);
	}

	public void ColorIcon(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		this.color = color;
		UXMouseOverColorFade component = ((Component)this).GetComponent<UXMouseOverColorFade>();
		if ((Object)(object)component != (Object)null)
		{
			component.SetNewColor(color);
		}
	}

	public void ChangeMaterial(Material material)
	{
		UXMouseOverColorFade component = ((Component)this).GetComponent<UXMouseOverColorFade>();
		if ((Object)(object)component != (Object)null)
		{
			component.materials.Remove(((Component)this).renderer.material);
		}
		((Component)this).renderer.material = material;
		if ((Object)(object)component != (Object)null)
		{
			component.materials.Add(((Component)this).renderer.material);
			component.UpdateMaterials();
		}
	}
}
