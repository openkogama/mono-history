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
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKey("up"))
		{
			((Component)this).transform.Translate(Vector3.forward * Time.deltaTime * (float)speed);
		}
		if (Input.GetKey("down"))
		{
			((Component)this).transform.Translate(Vector3.forward * Time.deltaTime * (float)(-speed));
		}
		if (Input.GetKey("left"))
		{
			((Component)this).transform.Rotate(Vector3.up * Time.deltaTime * (float)(-rotationSpeed));
		}
		if (Input.GetKey("right"))
		{
			((Component)this).transform.Rotate(Vector3.up * Time.deltaTime * (float)rotationSpeed);
		}
		if (Input.GetKey("w"))
		{
			((Component)this).transform.Translate(Vector3.forward * Time.deltaTime * (float)speed);
		}
		if (Input.GetKey("s"))
		{
			((Component)this).transform.Translate(Vector3.forward * Time.deltaTime * (float)(-speed));
		}
		if (Input.GetKey("a"))
		{
			((Component)this).transform.Translate(Vector3.right * Time.deltaTime * (float)(-sidewaysSpeed));
		}
		if (Input.GetKey("d"))
		{
			((Component)this).transform.Translate(Vector3.right * Time.deltaTime * (float)sidewaysSpeed);
		}
	}
}
