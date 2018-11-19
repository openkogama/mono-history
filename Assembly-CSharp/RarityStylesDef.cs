using System;
using UnityEngine;

[Serializable]
public class RarityStylesDef
{
	[SerializeField]
	public AccessoryRarity rarity;

	[SerializeField]
	public int priceRange;

	[SerializeField]
	public int levelRange;

	[SerializeField]
	public Color backgroundColor;

	[SerializeField]
	public Color glowColor;
}
