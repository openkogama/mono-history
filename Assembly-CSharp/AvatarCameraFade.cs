using System.Collections.Generic;
using UnityEngine;

public class AvatarCameraFade : MonoBehaviour
{
	public Transform cameraTfm;

	public float fadeStartDistance = 4f;

	public float fadeEndDistance = 2f;

	public Shader normalShader;

	public Shader fadeShader;

	private Transform avatarTfm;

	private Renderer[] avatarRenders = new Renderer[0];

	private bool fading;

	private Dictionary<string, Shader> normalShaders = new Dictionary<string, Shader>();

	private Dictionary<string, Shader> fadeShaders = new Dictionary<string, Shader>();

	private void Awake()
	{
		normalShaders["Particles/Alpha Blended Premultiply Write Alpha"] = Shader.Find("Particles/Alpha Blended Premultiply Write Alpha");
		fadeShaders["Particles/Alpha Blended Premultiply Write Alpha"] = Shader.Find("Particles/Alpha Blended Premultiply Write Alpha");
		normalShaders["Particles/Alpha Blended Write Alpha"] = Shader.Find("Particles/Alpha Blended Write Alpha");
		fadeShaders["Particles/Alpha Blended Write Alpha"] = Shader.Find("Particles/Alpha Blended Write Alpha");
		normalShaders["Particles/Additive"] = Shader.Find("Particles/Additive");
		fadeShaders["Particles/Additive"] = Shader.Find("Particles/Additive");
		normalShaders["Particles/Additive Write Alpha"] = Shader.Find("Particles/Additive Write Alpha");
		fadeShaders["Particles/Additive Write Alpha"] = Shader.Find("Particles/Additive Write Alpha");
		normalShaders["Particles/Additive (Soft) Write Alpha"] = Shader.Find("Particles/Additive (Soft) Write Alpha");
		fadeShaders["Particles/Additive (Soft) Write Alpha"] = Shader.Find("Particles/Additive (Soft) Write Alpha");
		normalShaders["Particles/Multiply Write Alpha"] = Shader.Find("Particles/Multiply Write Alpha");
		fadeShaders["Particles/Multiply Write Alpha"] = Shader.Find("Particles/Multiply Write Alpha");
		normalShaders["Custom/Laser Beam Additive"] = Shader.Find("Custom/Laser Beam Additive");
		fadeShaders["Custom/Laser Beam Additive"] = Shader.Find("Custom/Laser Beam Additive");
	}

	private void Update()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)avatarTfm == (Object)null)
		{
			MVAvatarLocal avatarLocal = MVGameController.Instance.WOCM.AvatarLocal;
			if (avatarLocal == null)
			{
				return;
			}
			avatarTfm = avatarLocal.GameObject.transform;
		}
		float num = Vector3.Distance(avatarTfm.position, cameraTfm.position);
		float num2 = Mathf.Clamp01((num - fadeEndDistance) / (fadeStartDistance - fadeEndDistance));
		if (num2 == 1f && !fading)
		{
			return;
		}
		fading = num2 < 1f;
		avatarRenders = ((Component)avatarTfm).GetComponentsInChildren<Renderer>();
		Renderer[] array = avatarRenders;
		foreach (Renderer val in array)
		{
			if (((Object)val).name == "100 percent" || ((Object)val).name == "Progress" || ((Object)val).name == "ChargeSphere")
			{
				continue;
			}
			Material[] materials = val.materials;
			foreach (Material val2 in materials)
			{
				Shader shader = GetShader(val2.shader, fading);
				val2.shader = shader;
				if (val2.HasProperty("_Color"))
				{
					Color color = val2.color;
					color.a = num2;
					val2.color = color;
				}
				else if (val2.HasProperty("_TintColor"))
				{
					Color color2 = val2.GetColor("_TintColor");
					color2.a = num2;
					val2.SetColor("_TintColor", color2);
				}
			}
		}
	}

	private Shader GetShader(Shader currShader, bool fading)
	{
		Shader value = null;
		if (normalShaders.ContainsKey(((Object)currShader).name) || fadeShaders.ContainsKey(((Object)currShader).name))
		{
			Dictionary<string, Shader> dictionary = ((!fading) ? normalShaders : fadeShaders);
			dictionary.TryGetValue(((Object)currShader).name, out value);
			value = value ?? currShader;
		}
		else if ((Object)(object)value == (Object)null)
		{
			value = ((!fading) ? normalShader : fadeShader);
		}
		return value;
	}
}
