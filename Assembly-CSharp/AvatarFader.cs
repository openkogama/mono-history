using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarFader : MonoBehaviour, IEventSystemHandler, IFadeParent
{
	[Serializable]
	public struct ShaderFaderInstruction
	{
		public string originShader;

		public Shader originalShader;

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

	private string colorProperty = "_Color";

	private string tintProperty = "_TintColor";

	private List<Material> avatarMaterials = new List<Material>();

	private bool fading;

	private bool changedShaders;

	private bool prevFading;

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

	private void Start()
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
		Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			Material[] materials = componentsInChildren[k].materials;
			for (int l = 0; l < materials.Length; l++)
			{
				avatarMaterials.Add(materials[l]);
			}
		}
	}

	public void SetTransparency(float fadeFactor)
	{
		if (fadeFactor == 1f && !fading)
		{
			return;
		}
		fading = fadeFactor < 1f;
		if (fadeFactor == 1f)
		{
			prevFading = false;
			changedShaders = false;
		}
		for (int i = 0; i < avatarMaterials.Count; i++)
		{
			if (avatarMaterials[i] == null)
			{
				avatarMaterials.RemoveAt(i);
				i--;
				continue;
			}
			if (!prevFading)
			{
				Shader shader = GetShader(avatarMaterials[i].shader, fading);
				avatarMaterials[i].shader = shader;
			}
			if (avatarMaterials[i].HasProperty(colorProperty))
			{
				Color color = avatarMaterials[i].color;
				color.a = fadeFactor;
				avatarMaterials[i].color = color;
			}
			else if (avatarMaterials[i].HasProperty(tintProperty))
			{
				Color color2 = avatarMaterials[i].GetColor(tintProperty);
				color2.a = fadeFactor;
				avatarMaterials[i].SetColor(tintProperty, color2);
			}
		}
		if (changedShaders)
		{
			prevFading = true;
		}
	}

	private Shader GetShader(Shader currShader, bool fading)
	{
		Shader value = null;
		if (fading)
		{
			changedShaders = true;
			if (!fadeShadersDictionary.TryGetValue(currShader.name, out value))
			{
				value = value ?? currShader;
			}
		}
		else if (!normalShadersDictionary.TryGetValue(currShader.name, out value))
		{
			value = value ?? currShader;
		}
		if (value == null)
		{
			value = ((!fading) ? normalShader : fadeShader);
		}
		return value;
	}

	public void AddFadeMaterial(Material addRenderer)
	{
		avatarMaterials.Add(addRenderer);
	}

	public void RemoveFadeMaterial(Material removeMaterial)
	{
		avatarMaterials.Remove(removeMaterial);
	}
}
