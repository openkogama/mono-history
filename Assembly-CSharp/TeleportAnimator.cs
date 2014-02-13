using UnityEngine;

public class TeleportAnimator : MonoBehaviour
{
	private Material _material;

	private void Awake()
	{
		_material = ((Component)this).renderer.material;
	}

	private void Update()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		_material.SetTextureOffset("_MainTex", new Vector2(Time.time * 0.1f, 0f));
	}
}
