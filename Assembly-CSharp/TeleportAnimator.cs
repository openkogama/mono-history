using UnityEngine;

public class TeleportAnimator : MonoBehaviour
{
	private Material _material;

	private void Awake()
	{
		_material = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		_material.SetTextureOffset("_MainTex", new Vector2(Time.time * 0.1f, 0f));
	}
}
