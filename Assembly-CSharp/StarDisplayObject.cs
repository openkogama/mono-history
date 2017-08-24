using UnityEngine;

public class StarDisplayObject : MonoBehaviour
{
	[SerializeField]
	private TextMesh frontText;

	[SerializeField]
	private TextMesh backText;

	[SerializeField]
	private StreamedSharedMaterialHandler handler;

	private void Start()
	{
		handler.StartTextureStreaming();
	}

	public void Destroy()
	{
		Object.Destroy(frontText);
		Object.Destroy(backText);
	}

	public void SetAmount(int starAmount)
	{
		frontText.text = starAmount.ToString();
		backText.text = starAmount.ToString();
	}

	public void SetScale(Vector3 size)
	{
		transform.localScale = size;
	}
}
