using UnityEngine;

namespace Util;

public class TexturePreviewer : MonoBehaviour
{
	private static TexturePreviewer _instance;

	public Texture tex;

	public static TexturePreviewer Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("TexturePreviewer");
				_instance = gameObject.AddComponent<TexturePreviewer>();
			}
			return _instance;
		}
	}
}
