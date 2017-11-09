using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
	public delegate void SkyboxColorChangedDelegate(Color newColor);

	[Header("Configuration")]
	public static Color defaultColor = new Color(95f / 255f, 180f / 255f, 254f / 255f);

	public static float defaultSunAngle = 80f;

	public static float defaultFogDensity = 0.007f;

	public Color currentColor = defaultColor;

	public float currentSunAngle = defaultSunAngle;

	public float currentFogDensity = defaultFogDensity;

	public List<MVSkybox> mvSkyboxes = new List<MVSkybox>();

	public Color brightAmbient = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	private float skyContrast = 0.1f;

	[SerializeField]
	private AnimationCurve lightDuskDawnFalloff;

	public SkyboxColorChangedDelegate OnSkyboxColorChanged;

	[SerializeField]
	[Header("Dependencies")]
	private Light sunLight;

	[SerializeField]
	private Camera targetCamera;

	[SerializeField]
	private MeshRenderer horizontalPlane;

	private Color targetColor;

	private float targetSunAngle;

	private float targetFogDensity;

	private bool initialized;

	public void RefreshColor()
	{
		if (initialized)
		{
			ComputeSkyboxSettings(out targetColor, out targetSunAngle, out targetFogDensity);
			StopCoroutine("DoAnimate");
			StartCoroutine("DoAnimate");
		}
	}

	private void Start()
	{
		MVGameControllerBase.OnPostGameInit = (MVGameControllerBase.OnPostGameInitDelegate)Delegate.Combine(MVGameControllerBase.OnPostGameInit, (MVGameControllerBase.OnPostGameInitDelegate)(() =>
		{
			ComputeSkyboxSettings(out targetColor, out targetSunAngle, out targetFogDensity);
			SetColor(targetColor, targetSunAngle, targetFogDensity);
			initialized = true;
		}));
	}

	private IEnumerator DoAnimate()
	{
		float t = 0f;
		while (t <= 1f)
		{
			Color c = Color.Lerp(currentColor, targetColor, t);
			float s = Mathf.Lerp(currentSunAngle, targetSunAngle, t);
			float d = Mathf.Lerp(currentFogDensity, targetFogDensity, t);
			t += Time.deltaTime * 0.01f;
			SetColor(c, s, d);
			yield return 0;
		}
	}

	private void ComputeSkyboxSettings(out Color color, out float sunAngle, out float fogDensity)
	{
		IEnumerable<MVSkybox> enumerable = mvSkyboxes.Where((MVSkybox s) => s.SkyboxActive);
		int num = enumerable.Count();
		if (num == 0)
		{
			color = defaultColor;
			fogDensity = defaultFogDensity;
			sunAngle = defaultSunAngle;
			return;
		}
		float num2 = enumerable.Select((MVSkybox s) => s.SunAngle).Average();
		sunAngle = num2;
		Color black = Color.black;
		foreach (MVSkybox item in enumerable)
		{
			black += item.SkyboxColor / num;
		}
		black.a = 1f;
		color = black;
		float num3 = enumerable.Select((MVSkybox s) => s.FogDensity).Average();
		fogDensity = num3;
	}

	private void SetColor(Color color, float sunAngle, float fogDensity)
	{
		currentColor = color;
		currentSunAngle = sunAngle;
		currentFogDensity = fogDensity;
		float grayscale = color.grayscale;
		float t = Mathf.SmoothStep(0f, 0.7f, grayscale) / 0.7f;
		Color color2 = Color.Lerp(color, brightAmbient, t);
		float num = Mathf.SmoothStep(0.9f, 0f, grayscale) * 0.85f + 0.4f;
		RenderSettings.fogColor = color;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.ambientLight = num * color2;
		sunLight.transform.rotation = Quaternion.Euler(sunAngle, 45f, 0f);
		float num2 = Mathf.Abs(currentSunAngle - 90f);
		if (num2 > 90f)
		{
			sunLight.intensity = 0f;
		}
		else
		{
			sunLight.intensity = lightDuskDawnFalloff.Evaluate(num2 / 90f);
		}
		Color color3 = color;
		float num3 = ((!(color3.r > 0.5f)) ? skyContrast : (0f - skyContrast));
		float num4 = ((!(color3.g > 0.5f)) ? skyContrast : (0f - skyContrast));
		float num5 = ((!(color3.b > 0.5f)) ? skyContrast : (0f - skyContrast));
		color3.r += num3;
		color.r -= num3;
		color3.g += num4;
		color.g -= num4;
		color3.b += num5;
		color.b -= num5;
		targetCamera.backgroundColor = color;
		horizontalPlane.material.SetColor("_Color", color3);
		if (OnSkyboxColorChanged != null)
		{
			OnSkyboxColorChanged(currentColor);
		}
	}
}
