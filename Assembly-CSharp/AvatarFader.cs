using System;
using System.Collections.Generic;
using UnityEngine;

public class AvatarFader : MonoBehaviour
{
	[Serializable]
	public struct ShaderFaderInstruction
	{
		public string originShader;

		public Shader replacingShader;
	}

	private Shader normalShader;

	private Shader fadeShader;

	[SerializeField]
	private ShaderFaderInstruction[] normalShaders;

	[SerializeField]
	private ShaderFaderInstruction[] fadeShaders;

	private Transform bodyTransform;

	private Dictionary<string, Shader> normalShadersDictionary;

	private Dictionary<string, Shader> fadeShadersDictionary;

	private Renderer[] avatarRenders = new Renderer[0];

	private bool fading;

	public Transform BodyTransform
	{
		get
		{
			return bodyTransform;
		}
		set
		{
			bodyTransform = value;
		}
	}

	private void Awake()
	{
		normalShader = MVGameControllerBase.MaterialLoader.AvatarShader;
		fadeShader = MVGameControllerBase.MaterialLoader.AvatarTransparentShader;
		normalShadersDictionary = new Dictionary<string, Shader>(normalShaders.Length);
		fadeShadersDictionary = new Dictionary<string, Shader>(fadeShaders.Length);
		int num = normalShaders.Length;
		for (int i = 0; i < num; i++)
		{
			normalShadersDictionary.Add(normalShaders[i].originShader, normalShaders[i].replacingShader);
		}
		num = fadeShaders.Length;
		for (int j = 0; j < num; j++)
		{
			fadeShadersDictionary.Add(fadeShaders[j].originShader, fadeShaders[j].replacingShader);
		}
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
		if (normalShadersDictionary.ContainsKey(currShader.name) || fadeShadersDictionary.ContainsKey(currShader.name))
		{
			Dictionary<string, Shader> dictionary = ((!fading) ? normalShadersDictionary : fadeShadersDictionary);
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
