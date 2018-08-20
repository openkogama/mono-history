using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro.Helpers;

public class Singleton<T> : MonoBehaviour where T : Component
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				T[] array = Object.FindObjectsOfType<T>();
				if (array.Length == 1)
				{
					_instance = array[0];
				}
				else if (array.Length > 1)
				{
					Debug.LogError(string.Concat(typeof(T), ": There is more than 1 instance in the scene."));
				}
				else
				{
					Debug.LogError(string.Concat(typeof(T), ": Instance doesn't exist in the scene."));
				}
			}
			return _instance;
		}
	}
}
