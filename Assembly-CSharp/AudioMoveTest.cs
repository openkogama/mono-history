using UnityEngine;

public class AudioMoveTest : MonoBehaviour
{
	public int speed = 30;

	public int rotationSpeed = 100;

	public int sidewaysSpeed = 30;

	private void Start()
	{
	}

	private void Update()
	{
		if (MVInputWrapper.DebugGetKey("up"))
		{
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
		}
		if (MVInputWrapper.DebugGetKey("down"))
		{
			transform.Translate(Vector3.forward * Time.deltaTime * -speed);
		}
		if (MVInputWrapper.DebugGetKey("left"))
		{
			transform.Rotate(Vector3.up * Time.deltaTime * -rotationSpeed);
		}
		if (MVInputWrapper.DebugGetKey("right"))
		{
			transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
		}
		if (MVInputWrapper.DebugGetKey("w"))
		{
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
		}
		if (MVInputWrapper.DebugGetKey("s"))
		{
			transform.Translate(Vector3.forward * Time.deltaTime * -speed);
		}
		if (MVInputWrapper.DebugGetKey("a"))
		{
			transform.Translate(Vector3.right * Time.deltaTime * -sidewaysSpeed);
		}
		if (MVInputWrapper.DebugGetKey("d"))
		{
			transform.Translate(Vector3.right * Time.deltaTime * sidewaysSpeed);
		}
	}
}
