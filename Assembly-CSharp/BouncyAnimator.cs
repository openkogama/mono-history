using System;
using UnityEngine;

public class BouncyAnimator : MaterialAnimator
{
	private void Update()
	{
		float num = 0.05f;
		float num2 = 1f;
		float num3 = 1f - num * (1f - Mathf.Abs(Mathf.Sin((float)Math.PI * 2f * num2 * Time.time)));
		float num4 = (1f - num3) / 2f;
		TargetMaterial.SetTextureOffset("_BouncyTex", new Vector2(num4, num4));
		TargetMaterial.SetTextureScale("_BouncyTex", new Vector2(num3, num3));
	}

	private void OnDisable()
	{
		TargetMaterial.SetTextureOffset("_BouncyTex", Vector2.zero);
		TargetMaterial.SetTextureScale("_BouncyTex", Vector2.one);
	}
}
