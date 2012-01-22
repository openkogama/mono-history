using UnityEngine;

public class LavaAnimator : MaterialAnimator
{
	public Vector2 speedA = new Vector2(-0.9f, -0.4f);

	public Vector2 speedB = new Vector2(0.49f, 0.56f);

	public float speed = 0.5f;

	public LavaAnimator()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		TargetMaterial.SetTextureOffset("_NoiseATex", TargetMaterial.GetTextureOffset("_NoiseATex") + speedA * Time.deltaTime * speed);
		TargetMaterial.SetTextureOffset("_NoiseBTex", TargetMaterial.GetTextureOffset("_NoiseBTex") + speedB * Time.deltaTime * speed);
	}

	private void OnDisable()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		TargetMaterial.SetTextureOffset("_NoiseATex", Vector2.zero);
		TargetMaterial.SetTextureOffset("_NoiseBTex", Vector2.zero);
	}
}
