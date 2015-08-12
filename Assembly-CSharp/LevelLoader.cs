using System;
using System.Collections;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
	public void LoadLevel(string levelName, LoadMode loadMode, Action callback)
	{
		StartCoroutine(WaitForLevel(levelName, loadMode, callback));
	}

	private IEnumerator WaitForLevel(string levelName, LoadMode loadMode, Action callback)
	{
		while (!Application.CanStreamedLevelBeLoaded(levelName))
		{
			yield return null;
		}
		AsyncOperation asyncOperation;
		switch (loadMode)
		{
		default:
			yield break;
		case LoadMode.Overwrite:
			asyncOperation = Application.LoadLevelAsync(levelName);
			break;
		case LoadMode.Additive:
			asyncOperation = Application.LoadLevelAdditiveAsync(levelName);
			break;
		}
		yield return asyncOperation;
		callback();
	}
}
