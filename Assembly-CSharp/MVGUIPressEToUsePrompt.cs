using UnityEngine;

public class MVGUIPressEToUsePrompt : UXViewScript
{
	public UXPlane uxPlane;

	public float fadeTime = 1f;

	public override void OnInitialize()
	{
		base.OnInitialize();
	}

	public override void OnShow()
	{
		uxPlane.SetVisible(visible: true);
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			uxPlane.SetAlpha(t, "_MainColor");
		}));
	}

	public override void OnHide()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			uxPlane.SetAlpha(t, "_MainColor");
			if (t == 0f)
			{
				uxPlane.SetVisible(visible: false);
			}
		}));
	}
}
