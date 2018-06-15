using System;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro.DotParams;

[Serializable]
public class CelestialParam : DotParam
{
	public Color TintColor = Color.gray;

	public Color LightColor = Color.gray;

	[Range(0f, 8f)]
	public float LightIntencity = 1f;
}
