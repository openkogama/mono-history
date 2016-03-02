using System.Collections;
using UnityEngine.Events;

public static class WaitForFrames
{
	public static IEnumerator Frames(int frameCount, UnityAction callback)
	{
		while (frameCount > 0)
		{
			frameCount--;
			yield return null;
		}
		callback();
	}
}
