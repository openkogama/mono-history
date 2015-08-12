using UnityEngine;

public class ProductPreviewData : DialogData
{
	public GameObject productPreview;

	public override void ApplyDataToElement(GameObject element)
	{
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
