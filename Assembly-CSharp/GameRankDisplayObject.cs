using MV.Common;
using UnityEngine;

public class GameRankDisplayObject : MonoBehaviour
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

	public void SetAmount(GamePassTier requiredRank)
	{
		TextMesh textMesh = frontText;
		int num = (int)requiredRank;
		textMesh.text = num.ToString();
		TextMesh textMesh2 = backText;
		int num2 = (int)requiredRank;
		textMesh2.text = num2.ToString();
	}

	public void SetScale(Vector3 size)
	{
		transform.localScale = size;
	}
}
