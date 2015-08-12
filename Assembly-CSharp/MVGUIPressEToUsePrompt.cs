using UnityEngine;

public class MVGUIPressEToUsePrompt : UXViewScript
{
	public UXPlane uxPlane;

	public float fadeTime = 1f;

	public Material normalMat;

	public Material enoughMat;

	public Material insufficientMat;

	public override void OnInitialize()
	{
		base.OnInitialize();
	}

	public void Show(ShowUseOption option)
	{
		switch (option)
		{
		case ShowUseOption.Normal:
			uxPlane.SetMaterial(normalMat);
			break;
		case ShowUseOption.GameCoinsEnough:
			uxPlane.SetMaterial(enoughMat);
			break;
		case ShowUseOption.GameCoinsInsufficient:
			uxPlane.SetMaterial(insufficientMat);
			break;
		}
		View.Show();
	}

	public void Hide()
	{
		View.Hide();
	}

	public override void OnShow()
	{
		uxPlane.SetVisible(visible: true);
		StopAllCoroutines();
		StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			uxPlane.SetAlpha(t, string.Empty);
		}));
	}

	public override void OnHide()
	{
		StopAllCoroutines();
		StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			uxPlane.SetAlpha(t, string.Empty);
			if (t == 0f)
			{
				uxPlane.SetVisible(visible: false);
			}
		}));
	}
}
