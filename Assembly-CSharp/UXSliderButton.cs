using UnityEngine;

public class UXSliderButton : UXPlane
{
	public void Start()
	{
		ignoreClipping = true;
	}

	public override void SetSize(float width, float height)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.SetSize(width, height);
		BoxCollider val = ((Component)this).gameObject.GetComponent<BoxCollider>();
		if ((Object)(object)val == (Object)null)
		{
			val = ((Component)this).gameObject.AddComponent<BoxCollider>();
		}
		val.size = new Vector3(width, height, 0.1f);
		SetVisible(Visible);
	}
}
