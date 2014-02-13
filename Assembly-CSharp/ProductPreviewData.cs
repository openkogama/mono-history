using UnityEngine;

public class ProductPreviewData : DialogData
{
	public GameObject productPreview;

	public override void ApplyDataToElement(GameObject element)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Transform component = element.GetComponent<Transform>();
		Vector3 localPosition = productPreview.transform.localPosition;
		Vector3 localScale = productPreview.transform.localScale;
		Quaternion localRotation = productPreview.transform.localRotation;
		productPreview.transform.parent = component;
		productPreview.transform.localPosition = localPosition;
		productPreview.transform.localScale = localScale;
		productPreview.transform.localRotation = localRotation;
	}
}
