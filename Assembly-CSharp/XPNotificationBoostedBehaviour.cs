using Gamestrap;
using UnityEngine;
using UnityEngine.UI;

public class XPNotificationBoostedBehaviour : MonoBehaviour
{
	[SerializeField]
	private Text xpText;

	[SerializeField]
	private Text unboostedXpText;

	[SerializeField]
	private ProgressBar xpFillBar;

	[SerializeField]
	private ProgressBar xpFillBarBackground;

	[SerializeField]
	private RectTransform boostEffectTransform;

	[SerializeField]
	private GradientEffect arrowGradientEffect;

	[SerializeField]
	private GameObject glowEffectGameObject;

	[SerializeField]
	private XpBoostParticlePreviewer xpBoostParticlesPreviewPrefab;

	[SerializeField]
	private RawImage xpBoostParticlesRawImage;

	[SerializeField]
	private AnimationCurve boostEffectCurve;

	[SerializeField]
	private AnimationCurve glowEffectCurve;

	[SerializeField]
	private AnimationCurve gradientEffectCurve;

	[SerializeField]
	private float slideOutSpeed;

	private const float fillImageFillingStartTime = 1.125f;

	private const float fillImageFillingFinishTime = 2.29167f;

	private const float fillImageBackgroundFillingStartTime = 1f;

	private const float fillImageBackgroundFillingFinishTime = 2.29167f;

	private const float boostEffectsStartTime = 1.2f;

	private const float gradientEffectsStartTime = 1.2f;

	private const float glowEffectStartTime = 1.7817f;

	private const float glowEnableTime = 2.28167f;

	private const float slideOutBoostEffectsStartTime = 2.29167f;

	private const float boostParticlesStartTime = 1.2f;

	private const float gradientEffectTopDelay = 0.125f;

	private float boostEffectStatPositionX;

	private float startTime;

	private XpBoostParticlePreviewer xpBoostParticlesPreview;

	public void Initialize(int boostedXPAmount, int unboostedXpAmount)
	{
		if (xpBoostParticlesPreview == null)
		{
			xpBoostParticlesPreview = Object.Instantiate(xpBoostParticlesPreviewPrefab);
			xpBoostParticlesPreview.Initialize(600, 160, CameraClearFlags.Color, LayerFlags.Preview, new Vector3(0f, 0f, 0f), new Vector3(10000f, -10000f, -10000f));
			xpBoostParticlesRawImage.texture = xpBoostParticlesPreview.PreviewTexture;
		}
		startTime = Time.time;
		xpText.text = boostedXPAmount + " XP!";
		unboostedXpText.text = unboostedXpAmount + " XP";
		if (boostEffectStatPositionX == 0f)
		{
			boostEffectStatPositionX = boostEffectTransform.localPosition.x + (boostEffectTransform.rect.width - boostEffectTransform.pivot.x * boostEffectTransform.rect.width);
		}
		Reset();
	}

	private void Update()
	{
		float num = Time.time - startTime;
		if (num > 1.125f)
		{
			DoFillImageFillingEffect(num);
		}
		if (num > 1.2f)
		{
			DoBoostEffect(num);
		}
		if (num > 1.2f)
		{
			DoGradientEffect(num);
		}
		if (num > 1.7817f)
		{
			DoGlowEffect(num);
		}
		if (num > 2.29167f)
		{
			DoSlideOutEffect();
		}
		if (num > 1.2f)
		{
			DoParticleEffect();
		}
	}

	private void DoFillImageFillingEffect(float timeSinceStart)
	{
		xpFillBar.Progress = (timeSinceStart - 1.125f) / 1.1666701f;
		xpFillBarBackground.Progress = (timeSinceStart - 1f) / 1.2916701f;
	}

	private void DoBoostEffect(float timeSinceStart)
	{
		if (!boostEffectTransform.gameObject.activeSelf)
		{
			boostEffectTransform.gameObject.SetActive(value: true);
		}
		if (unboostedXpText.gameObject.activeSelf)
		{
			unboostedXpText.gameObject.SetActive(value: false);
		}
		float time = timeSinceStart - 1.2f;
		float num = boostEffectCurve.Evaluate(time);
		Vector3 localScale = new Vector3(num, num, boostEffectTransform.localScale.z);
		boostEffectTransform.localScale = localScale;
	}

	private void DoGlowEffect(float timeSinceStart)
	{
		if (timeSinceStart > 2.28167f && !glowEffectGameObject.activeSelf)
		{
			glowEffectGameObject.SetActive(value: true);
		}
		float time = timeSinceStart - 1.7817f;
		float num = glowEffectCurve.Evaluate(time);
		Vector3 localScale = new Vector3(1f + (num - 1f) / 2.5f, num, glowEffectGameObject.transform.localScale.z);
		glowEffectGameObject.transform.localScale = localScale;
	}

	private void DoGradientEffect(float timeSinceStart)
	{
		float num = timeSinceStart - 1.2f;
		Color bottom = arrowGradientEffect.bottom;
		bottom.a = gradientEffectCurve.Evaluate(num);
		arrowGradientEffect.bottom = bottom;
		bottom = arrowGradientEffect.top;
		bottom.a = gradientEffectCurve.Evaluate(num - 0.125f);
		arrowGradientEffect.top = bottom;
		arrowGradientEffect.gameObject.SetActive(value: false);
		arrowGradientEffect.gameObject.SetActive(value: true);
	}

	private void DoSlideOutEffect()
	{
		if (!xpText.gameObject.activeSelf)
		{
			xpText.gameObject.SetActive(value: true);
		}
		Vector3 position = boostEffectTransform.position;
		position.x += slideOutSpeed * Time.deltaTime;
		boostEffectTransform.position = position;
	}

	private void DoParticleEffect()
	{
		if (!xpBoostParticlesPreview.IsParticlesPlaying)
		{
			xpBoostParticlesPreview.StartParticleSystem();
		}
	}

	private void Reset()
	{
		Vector3 localPosition = boostEffectTransform.localPosition;
		localPosition.x = boostEffectStatPositionX - (boostEffectTransform.rect.width - boostEffectTransform.pivot.x * boostEffectTransform.rect.width);
		boostEffectTransform.localPosition = localPosition;
		if (boostEffectTransform.gameObject.activeSelf)
		{
			boostEffectTransform.gameObject.SetActive(value: false);
		}
		xpFillBar.Progress = 0f;
		xpFillBarBackground.Progress = 0f;
		Vector3 localScale = new Vector3(1f, 1f, glowEffectGameObject.transform.localScale.z);
		glowEffectGameObject.transform.localScale = localScale;
		if (glowEffectGameObject.activeSelf)
		{
			glowEffectGameObject.SetActive(value: false);
		}
		Color bottom = arrowGradientEffect.bottom;
		bottom.a = 0f;
		arrowGradientEffect.bottom = bottom;
		arrowGradientEffect.top = bottom;
		if (xpText.gameObject.activeSelf)
		{
			xpText.gameObject.SetActive(value: false);
		}
		if (!unboostedXpText.gameObject.activeSelf)
		{
			unboostedXpText.gameObject.SetActive(value: true);
		}
		if (xpBoostParticlesPreview.IsParticlesPlaying)
		{
			xpBoostParticlesPreview.StopParticleSystem();
		}
	}
}
