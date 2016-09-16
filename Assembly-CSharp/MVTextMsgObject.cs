using UnityEngine;

public class MVTextMsgObject : ObjectPrefab
{
	[SerializeField]
	private TextMesh textMesh;

	[SerializeField]
	private Renderer textMeshRenderer;

	[SerializeField]
	private GameObject visualObject;

	public GameObject VisualObject => visualObject;

	public TextMesh TextMesh => textMesh;

	public Renderer TextMeshRenderer => textMeshRenderer;
}
