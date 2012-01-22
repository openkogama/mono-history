using UnityEngine;

public class AudioScriptTestB : AudioScript
{
	public AudioClip[] triggers;

	public void TestPrint()
	{
		MonoBehaviour.print((object)"TestPrint: B");
	}

	public override void Test()
	{
	}
}
