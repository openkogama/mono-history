using System;
using UnityEngine;

public class BouncyAnimator : MaterialAnimator
{
	private void Update()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.05f;
		float num2 = 1f;
		float num3 = 1f - num * (1f - Mathf.Abs(Mathf.Sin((float)Math.PI * 2f * num2 * Time.time)));
		float num4 = (1f - num3) / 2f;
		TargetMaterial.SetTextureOffset("_BouncyTex", new Vector2(num4, num4));
		TargetMaterial.SetTextureScale("_BouncyTex", new Vector2(num3, num3));
	}

	private void OnDisable()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		TargetMaterial.SetTextureOffset("_BouncyTex", Vector2.zero);
		TargetMaterial.SetTextureScale("_BouncyTex", Vector2.one);
	}
}
