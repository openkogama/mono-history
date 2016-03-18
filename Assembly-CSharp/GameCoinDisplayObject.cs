using UnityEngine;

public class GameCoinDisplayObject : MonoBehaviour
{
	[SerializeField]
	private TextMesh textMesh1;

	[SerializeField]
	private TextMesh textMesh2;

	[SerializeField]
	private GameObject coinMesh;

	private float visibilityDistance = 25f;

	private float scaleTime = 1f;

	private bool visible;

	[SerializeField]
	private Renderer textMeshRenderer1;

	[SerializeField]
	private Renderer textMeshRenderer2;

	[SerializeField]
	private Renderer coinMeshRenderer;

	private GameCoinStringRenderer stringRenderer;

	private int amount;

	public void Start()
	{
		coinMeshRenderer.enabled = false;
		textMeshRenderer1.enabled = false;
		textMeshRenderer2.enabled = false;
		visible = false;
		transform.localScale = Vector3.zero;
	}

	public void Update()
	{
		if (MVGameControllerBase.WOCM.AvatarLocal != null)
		{
			ChangeLOD((transform.position - MVGameControllerBase.WOCM.AvatarLocal.Transform.position).magnitude);
		}
	}

	public void SetAmount(int amount)
	{
		this.amount = amount;
		string text = string.Empty;
		if (amount > 0)
		{
			text = amount.ToString();
		}
		textMesh1.text = text;
		textMesh2.text = text;
	}

	public void Destroy()
	{
		Object.Destroy(textMesh1);
		Object.Destroy(textMesh2);
		Object.Destroy(coinMesh);
	}

	public void ChangeLOD(float distance)
	{
		if (distance <= visibilityDistance && amount > 0)
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
		StartCoroutine(pTween.To(scaleTime, 0f, 1f, (float t) =>
		{
			SetScale(t);
		}));
		textMeshRenderer1.enabled = true;
		textMeshRenderer2.enabled = true;
		coinMeshRenderer.enabled = true;
		visible = true;
	}

	private void Hide()
	{
		StartCoroutine(pTween.To(scaleTime, 1f, 0f, (float t) =>
		{
			SetScale(t);
		}));
		visible = false;
	}

	private void SetScale(float value)
	{
		transform.localScale = new Vector3(value, value, value);
		if (value <= 0f && !visible)
		{
			textMeshRenderer1.enabled = false;
			textMeshRenderer2.enabled = false;
			coinMeshRenderer.enabled = false;
		}
	}
}
