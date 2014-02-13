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
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected Obj, but got Unknown
			if ((Object)(object)_instance == (Object)null)
			{
				GameObject val = new GameObject("TexturePreviewer");
				_instance = val.AddComponent<TexturePreviewer>();
			}
			return _instance;
		}
	}
}
