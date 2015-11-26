using UnityEngine;

public class StarDisplayObject : MonoBehaviour
{
	[SerializeField]
	private TextMesh frontText;

	[SerializeField]
	private TextMesh backText;

	private Renderer starRenderer;

	private float visibilityDistance = 25f;

	public void Initialize()
	{
		starRenderer = GetComponent<Renderer>();
	}

	private void Update()
	{
		if (MVGameControllerBase.WOCM.AvatarLocal != null)
		{
			ChangeLOD((transform.position - MVGameControllerBase.WOCM.AvatarLocal.Transform.position).magnitude);
		}
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

	public void ChangeLOD(float distance)
	{
		if (distance <= visibilityDistance)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Show()
	{
		starRenderer.enabled = true;
		frontText.GetComponent<Renderer>().enabled = true;
		backText.GetComponent<Renderer>().enabled = true;
	}

	private void Hide()
	{
		starRenderer.enabled = false;
		frontText.GetComponent<Renderer>().enabled = false;
		backText.GetComponent<Renderer>().enabled = false;
	}
}
