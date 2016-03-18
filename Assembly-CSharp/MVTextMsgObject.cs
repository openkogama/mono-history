using UnityEngine;

public class MVTextMsgObject : ObjectPrefab
{
	[SerializeField]
	private TextMesh textMesh;

	[SerializeField]
	private Renderer textMeshRenderer;

	public TextMesh TextMesh => textMesh;

	public Renderer TextMeshRenderer => textMeshRenderer;
}
