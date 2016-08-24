using Gamestrap;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(ShadowEffect))]
[RequireComponent(typeof(GradientEffect))]
[RequireComponent(typeof(Graphic))]
public class EffectStyleObject : MonoBehaviour
{
	[SerializeField]
	private EffectStyle effectStyle;

	[SerializeField]
	public Graphic graphic;

	[SerializeField]
	public ShadowEffect shadow;

	[SerializeField]
	public Outline outline;

	[SerializeField]
	public GradientEffect gradient;

	private void Awake()
	{
		Styles.SetStyle(this, effectStyle);
	}

	private void Reset()
	{
		graphic = GetComponent<Graphic>();
		shadow = GetComponent<ShadowEffect>();
		outline = GetComponent<Outline>();
		gradient = GetComponent<GradientEffect>();
		Styles.SetStyle(this, effectStyle);
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			Styles.SetStyle(this, effectStyle);
		}
	}
}
