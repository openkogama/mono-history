using System.Collections.Generic;
using UnityEngine;

public class AvatarFader
{
	private readonly Shader normalShader;

	private readonly Shader fadeShader;

	private readonly Dictionary<string, Shader> normalShaders = new Dictionary<string, Shader>();

	private readonly Dictionary<string, Shader> fadeShaders = new Dictionary<string, Shader>();

	private Renderer[] avatarRenders = new Renderer[0];

	private bool fading;

	private Transform transform;

	public AvatarFader(Transform transform)
	{
		this.transform = transform;
		normalShader = MVGameControllerBase.MaterialLoader.AvatarShader;
		fadeShader = MVGameControllerBase.MaterialLoader.AvatarTransparentShader;
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
		normalShaders["Diffuse with vertex colors transparent"] = Shader.Find("Diffuse with vertex colors");
		fadeShaders["Diffuse with vertex colors"] = Shader.Find("Diffuse with vertex colors transparent");
		normalShaders["Legacy Shaders/Diffuse"] = Shader.Find("Legacy Shaders/Diffuse");
		fadeShaders["Legacy Shaders/Diffuse"] = Shader.Find("Diffuse with vertex colors transparent");
	}

	public void SetTransparency(float fadeFactor)
	{
		if (fadeFactor == 1f && !fading)
		{
			return;
		}
		fading = fadeFactor < 1f;
		avatarRenders = transform.GetComponentsInChildren<Renderer>();
		Renderer[] array = avatarRenders;
		foreach (Renderer renderer in array)
		{
			if (renderer.name == "ChargeSphere")
			{
				continue;
			}
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				Shader shader = GetShader(material.shader, fading);
				material.shader = shader;
				if (material.HasProperty("_Color"))
				{
					Color color = material.color;
					color.a = fadeFactor;
					material.color = color;
				}
				else if (material.HasProperty("_TintColor"))
				{
					Color color2 = material.GetColor("_TintColor");
					color2.a = fadeFactor;
					material.SetColor("_TintColor", color2);
				}
			}
		}
	}

	private Shader GetShader(Shader currShader, bool fading)
	{
		Shader value = null;
		if (normalShaders.ContainsKey(currShader.name) || fadeShaders.ContainsKey(currShader.name))
		{
			Dictionary<string, Shader> dictionary = ((!fading) ? normalShaders : fadeShaders);
			dictionary.TryGetValue(currShader.name, out value);
			value = value ?? currShader;
		}
		else if (value == null)
		{
			value = ((!fading) ? normalShader : fadeShader);
		}
		return value;
	}
}
