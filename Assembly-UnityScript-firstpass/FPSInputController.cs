using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Character/FPS Input Controller")]
[RequireComponent(typeof(CharacterMotor))]
public class FPSInputController : MonoBehaviour
{
	private CharacterMotor motor;

	public override void Awake()
	{
		motor = (CharacterMotor)(object)((Component)this).GetComponent(typeof(CharacterMotor));
	}

	public override void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		if (val != Vector3.zero)
		{
			float magnitude = val.magnitude;
			val /= magnitude;
			magnitude = Mathf.Min(1f, magnitude);
			magnitude *= magnitude;
			val *= magnitude;
		}
		motor.inputMoveDirection = ((Component)this).transform.rotation * val;
		motor.inputJump = Input.GetButton("Jump");
	}

	public override void Main()
	{
	}
}
