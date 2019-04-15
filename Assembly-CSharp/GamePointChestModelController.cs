using UnityEngine;

public class GamePointChestModelController : MonoBehaviour
{
	public GameObject closedMesh;

	public GameObject openMesh;

	[SerializeField]
	private Animation openingAnimation;

	[SerializeField]
	private Renderer openingRenderer;

	[SerializeField]
	private GreyOutObjectScript greyOutScript;

	[SerializeField]
	private float fadeStartTime;

	[SerializeField]
	private float fadeEndTime;

	private bool isOpening;

	private float openingStartTime;

	public void Open()
	{
		openMesh.SetActive(value: true);
		closedMesh.SetActive(value: false);
		openingAnimation.Play();
		openingStartTime = Time.time;
		isOpening = true;
		greyOutScript.GreyOut();
	}

	public void Close()
	{
		openMesh.SetActive(value: false);
		closedMesh.SetActive(value: true);
		isOpening = false;
		Color color = openingRenderer.material.color;
		color.a = 1f;
		openingRenderer.material.color = color;
		greyOutScript.GreyIn();
	}

	public void Disable()
	{
		Color color = openingRenderer.material.color;
		color.a = 0f;
		openingRenderer.material.color = color;
		isOpening = false;
		openMesh.SetActive(value: false);
		closedMesh.SetActive(value: false);
	}

	public bool IsVisible()
	{
		return openMesh.activeSelf || closedMesh.activeSelf;
	}

	private void Update()
	{
		if (!isOpening)
		{
			return;
		}
		float num = Time.time - openingStartTime;
		if (num > fadeStartTime)
		{
			float num2 = 1f - (num - fadeStartTime) / (fadeEndTime - fadeStartTime);
			Color color = openingRenderer.material.color;
			color.a = num2;
			openingRenderer.material.color = color;
			if (num2 <= 0f)
			{
				isOpening = false;
				openMesh.SetActive(value: false);
			}
		}
	}
}
