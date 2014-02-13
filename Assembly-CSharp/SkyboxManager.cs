using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
	public delegate void SkyboxColorChangedDelegate(Color newColor);

	public static Color defaultColor = new Color(95f / 255f, 180f / 255f, 254f / 255f);

	public static float defaultSunAngle = 80f;

	public static float defaultFogDensity = 0.007f;

	public SkyboxColorChangedDelegate OnSkyboxColorChanged;

	public Color currentColor = defaultColor;

	public float currentSunAngle = defaultSunAngle;

	public float currentFogDensity = defaultFogDensity;

	public List<MVSkybox> mvSkyboxes = new List<MVSkybox>();

	public AnimationCurve luminanceToAmbientScale;

	public AnimationCurve luminanceToSkyboxFactor;

	public Color brightAmbient = new Color(1f, 1f, 1f, 1f);

	private Light mainLight;

	private Color targetColor;

	private float targetSunAngle;

	private float targetFogDensity;

	private bool initialized;

	public SkyboxManager()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
	}

	static SkyboxManager()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
	}

	public void RefreshColor()
	{
		if (initialized)
		{
			ComputeSkyboxSettings(out targetColor, out targetSunAngle, out targetFogDensity);
			((MonoBehaviour)this).StopCoroutine("DoAnimate");
			((MonoBehaviour)this).StartCoroutine("DoAnimate");
		}
	}

	private void Start()
	{
		MVGameController instance = MVGameController.Instance;
		instance.OnPostGameInit = (MVGameController.OnPostGameInitDelegate)Delegate.Combine(instance.OnPostGameInit, (MVGameController.OnPostGameInitDelegate)(() =>
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)mainLight))
			{
				GameObject val = GameObject.Find("Main Directional Light");
				if (Object.op_Implicit((Object)(object)val))
				{
					mainLight = val.light;
				}
			}
			ComputeSkyboxSettings(out targetColor, out targetSunAngle, out targetFogDensity);
			SetColor(targetColor, targetSunAngle, targetFogDensity);
			initialized = true;
		}));
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
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
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<MVSkybox> enumerable = mvSkyboxes.Where((MVSkybox s) => s.SkyboxActive);
		int num = enumerable.Count();
		if (num == 0)
		{
			color = defaultColor;
			fogDensity = defaultFogDensity;
			sunAngle = defaultSunAngle;
			return;
		}
		Color val = Color.black;
		foreach (MVSkybox item in enumerable)
		{
			val += item.SkyboxColor / (float)num;
		}
		val.a = 1f;
		float num2 = enumerable.Select((MVSkybox s) => s.SunAngle).Average();
		float num3 = enumerable.Select((MVSkybox s) => s.FogDensity).Average();
		color = val;
		sunAngle = num2;
		fogDensity = num3;
	}

	private void SetColor(Color color, float sunAngle, float fogDensity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		currentColor = color;
		currentSunAngle = sunAngle;
		currentFogDensity = fogDensity;
		float grayscale = color.grayscale;
		float num = Mathf.SmoothStep(0f, 0.7f, grayscale) / 0.7f;
		Color val = Color.Lerp(color, brightAmbient, num);
		float num2 = Mathf.SmoothStep(0.9f, 0f, grayscale) * 0.85f + 0.4f;
		RenderSettings.fogColor = color;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.ambientLight = num2 * val;
		if (Object.op_Implicit((Object)(object)Camera.main))
		{
			Camera.main.backgroundColor = color;
		}
		if (Object.op_Implicit((Object)(object)mainLight))
		{
			((Component)mainLight).transform.rotation = Quaternion.Euler(sunAngle, 45f, 0f);
		}
		if (OnSkyboxColorChanged != null)
		{
			OnSkyboxColorChanged(currentColor);
		}
	}
}
