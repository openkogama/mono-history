using UnityEngine;

public class AudioScriptTestMain : MonoBehaviour
{
	private AudioScript[] scripts;

	private void Awake()
	{
		scripts = ((Component)this).GetComponents<AudioScript>();
		AudioScript[] array = scripts;
		foreach (AudioScript audioScript in array)
		{
			audioScript.Test();
			MonoBehaviour.print((object)((object)audioScript).GetType().Name);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
