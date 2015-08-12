using UnityEngine;

public class LavaAnimator : MaterialAnimator
{
	public Vector2 speedA = new Vector2(-0.9f, -0.4f);

	public Vector2 speedB = new Vector2(0.49f, 0.56f);

	public float speed = 0.5f;

	private void Update()
	{
		TargetMaterial.SetTextureOffset("_NoiseATex", TargetMaterial.GetTextureOffset("_NoiseATex") + speedA * Time.deltaTime * speed);
		TargetMaterial.SetTextureOffset("_NoiseBTex", TargetMaterial.GetTextureOffset("_NoiseBTex") + speedB * Time.deltaTime * speed);
	}

	private void OnDisable()
	{
		TargetMaterial.SetTextureOffset("_NoiseATex", Vector2.zero);
		TargetMaterial.SetTextureOffset("_NoiseBTex", Vector2.zero);
	}
}
