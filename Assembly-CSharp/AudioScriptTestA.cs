using UnityEngine;

public class AudioScriptTestA : AudioScript
{
	public AudioClip loop;

	public void TestPrint()
	{
		MonoBehaviour.print((object)"TestPrint: A");
	}
}
