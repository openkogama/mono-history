using UnityEngine;

public class UXSliderButton : UXPlane
{
	public void Start()
	{
		ignoreClipping = true;
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
		if (boxCollider == null)
		{
			boxCollider = gameObject.AddComponent<BoxCollider>();
		}
		boxCollider.size = new Vector3(width, height, 0.1f);
		SetVisible(Visible);
	}
}
