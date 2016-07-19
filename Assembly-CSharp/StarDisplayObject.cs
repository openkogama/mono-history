using UnityEngine;

public class StarDisplayObject : MonoBehaviour
{
	[SerializeField]
	private TextMesh frontText;

	[SerializeField]
	private TextMesh backText;

	private Renderer starRenderer;

	private Renderer frontRenderer;

	private Renderer backRenderer;

	private float visibilityDistance = 25f;

	private bool visible;

	public Renderer StarRenderer
	{
		get
		{
			if (starRenderer == null)
			{
				starRenderer = GetComponent<Renderer>();
			}
			return starRenderer;
		}
	}

	public Renderer FrontRenderer
	{
		get
		{
			if (frontRenderer == null)
			{
				frontRenderer = frontText.GetComponent<Renderer>();
			}
			return frontRenderer;
		}
	}

	public Renderer BackRenderer
	{
		get
		{
			if (backRenderer == null)
			{
				backRenderer = backText.GetComponent<Renderer>();
			}
			return backRenderer;
		}
	}

	public void Initialize()
	{
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
			if (!visible)
			{
				Show();
			}
		}
		else if (visible)
		{
			Hide();
		}
	}

	private void Show()
	{
		StarRenderer.enabled = true;
		FrontRenderer.enabled = true;
		BackRenderer.enabled = true;
		visible = true;
	}

	private void Hide()
	{
		StarRenderer.enabled = false;
		FrontRenderer.enabled = false;
		BackRenderer.enabled = false;
		visible = false;
	}
}
