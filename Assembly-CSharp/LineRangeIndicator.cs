using UnityEngine;

public class LineRangeIndicator : MonoBehaviour
{
	[SerializeField]
	[Header("Configuration")]
	private float lineDotDensity = 4.1f;

	[SerializeField]
	private float lineWidth = 0.6f;

	[Header("Dependencies")]
	[SerializeField]
	private MeshRenderer rangeIndicator;

	[SerializeField]
	private MeshRenderer rangeIndicator_backside;

	[SerializeField]
	private Material lineDotMaterial;

	private Material materialCopy;

	protected void Awake()
	{
		CopyMaterial();
	}

	private void CopyMaterial()
	{
		materialCopy = new Material(lineDotMaterial);
		rangeIndicator.material = materialCopy;
		rangeIndicator_backside.material = materialCopy;
	}

	public void SetRange(float range)
	{
		if (range <= 0f || lineWidth <= 0f)
		{
			rangeIndicator.gameObject.SetActive(value: false);
			return;
		}
		rangeIndicator.gameObject.SetActive(value: true);
		Vector3 localScale = new Vector3(range * 0.2f, 1f, lineWidth * 0.2f);
		localScale.x *= rangeIndicator.transform.lossyScale.x / rangeIndicator.transform.localScale.x;
		localScale.z *= rangeIndicator.transform.lossyScale.z / rangeIndicator.transform.localScale.z;
		rangeIndicator.transform.localScale = localScale;
		materialCopy.SetTextureScale("_MainTex", new Vector2(lineDotDensity * range, 1f));
	}
}
