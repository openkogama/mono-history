using UnityEngine;

public class GameCoinDisplayObject : MonoBehaviour
{
	[SerializeField]
	private TextMesh textMesh1;

	[SerializeField]
	private TextMesh textMesh2;

	[SerializeField]
	private GameObject coinMesh;

	public void SetAmount(int amount)
	{
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

	public void SetScale(Vector3 size)
	{
		transform.localScale = size;
	}
}
