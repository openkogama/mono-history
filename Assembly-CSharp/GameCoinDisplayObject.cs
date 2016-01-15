using UnityEngine;

public class GameCoinDisplayObject : MonoBehaviour
{
	public TextMesh textMesh1;

	public TextMesh textMesh2;

	public GameObject coinMesh;

	public Transform stringAttachPoint;

	private float visibilityDistance = 25f;

	private float scaleTime = 1f;

	private bool visible;

	private Renderer textMeshRenderer1;

	private Renderer textMeshRenderer2;

	private Renderer coinMeshRenderer;

	private GameCoinStringRenderer stringRenderer;

	private int amount;

	public Renderer TextMeshRenderer1
	{
		get
		{
			if (textMeshRenderer1 == null)
			{
				textMeshRenderer1 = textMesh1.GetComponent<Renderer>();
			}
			return textMeshRenderer1;
		}
	}

	public Renderer TextMeshRenderer2
	{
		get
		{
			if (textMeshRenderer2 == null)
			{
				textMeshRenderer2 = textMesh2.GetComponent<Renderer>();
			}
			return textMeshRenderer2;
		}
	}

	public Renderer CoinMeshRenderer
	{
		get
		{
			if (coinMeshRenderer == null)
			{
				coinMeshRenderer = coinMesh.GetComponent<Renderer>();
			}
			return coinMeshRenderer;
		}
	}

	public void Start()
	{
		CoinMeshRenderer.enabled = false;
		TextMeshRenderer1.enabled = false;
		TextMeshRenderer2.enabled = false;
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
		TextMeshRenderer1.enabled = true;
		TextMeshRenderer2.enabled = true;
		CoinMeshRenderer.enabled = true;
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
			TextMeshRenderer1.enabled = false;
			TextMeshRenderer2.enabled = false;
			CoinMeshRenderer.enabled = false;
		}
	}
}
