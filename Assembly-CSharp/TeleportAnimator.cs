using UnityEngine;

public class TeleportAnimator : MonoBehaviour
{
	private const string texString = "_MainTex";

	[SerializeField]
	private Renderer teleportRenderer;

	private Material _material;

	private void Awake()
	{
		_material = teleportRenderer.material;
	}

	private void Update()
	{
		_material.SetTextureOffset("_MainTex", new Vector2(Time.time * 0.1f, 0f));
	}
}
