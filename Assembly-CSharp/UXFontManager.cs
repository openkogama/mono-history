using UnityEngine;

public class UXFontManager : MonoBehaviour
{
	public Font[] Fonts;

	public Material[] Materials;

	public Font GetFont(UXTextSize textSize)
	{
		return Fonts[(int)textSize];
	}

	public Material GetFontMaterial(UXTextSize textSize)
	{
		return Materials[(int)textSize];
	}
}
