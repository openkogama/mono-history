using UnityEngine;

public class GameCoinDisplayObject : MonoBehaviour
{
	public TextMesh textMesh1;

	public TextMesh textMesh2;

	public GameObject coinMesh;

	public Transform stringAttachPoint;

	private float rotSpeed = 10f;

	private float visibilityDistance = 25f;

	private float scaleTime = 1f;

	private bool visible;

	private GameObject objectRoot;

	private GameCoinStringRenderer stringRenderer;

	private int amount;

	public GameObject ObjectRoot
	{
		set
		{
			objectRoot = value;
		}
	}

	public void Start()
	{
		coinMesh.GetComponent<Renderer>().enabled = false;
		textMesh1.GetComponent<Renderer>().enabled = false;
		textMesh2.GetComponent<Renderer>().enabled = false;
		visible = false;
		stringRenderer = (GameCoinStringRenderer)Object.FindObjectsOfType(typeof(GameCoinStringRenderer))[0];
		if (stringRenderer == null)
		{
			Debug.LogError("GameCOinStringRenderer not found in scene!");
		}
		transform.localScale = Vector3.zero;
	}

	public void Update()
	{
		if (visible)
		{
			transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
		}
		if (transform.localScale.x > 0.8f)
		{
			stringRenderer.AddString(objectRoot.transform.position, stringAttachPoint.position, Color.white);
		}
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
		Object.Destroy(this);
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
		textMesh1.GetComponent<Renderer>().enabled = true;
		textMesh2.GetComponent<Renderer>().enabled = true;
		coinMesh.GetComponent<Renderer>().enabled = true;
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
			textMesh1.GetComponent<Renderer>().enabled = false;
			textMesh2.GetComponent<Renderer>().enabled = false;
			coinMesh.GetComponent<Renderer>().enabled = false;
		}
	}
}
