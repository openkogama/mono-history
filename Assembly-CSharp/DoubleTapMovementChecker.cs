using UnityEngine;

public class DoubleTapMovementChecker
{
	private const float doubleTapThreshold = 0.3f;

	private KogamaControls[] movementControls = new KogamaControls[4]
	{
		KogamaControls.EditMoveForward,
		KogamaControls.EditMoveBackwards,
		KogamaControls.EditMoveLeft,
		KogamaControls.EditMoveRight
	};

	private KogamaControls lastMovement;

	private float timer;

	private bool doubleTap;

	public bool DoubleTap => doubleTap;

	public void FrameUpdate()
	{
		if (MVInputWrapper.GetBooleanControlDown(lastMovement) && Time.realtimeSinceStartup - timer < 0.3f)
		{
			doubleTap = true;
			timer = Time.realtimeSinceStartup;
		}
		else if (!doubleTap)
		{
			KogamaControls[] array = movementControls;
			foreach (KogamaControls control in array)
			{
				if (MVInputWrapper.GetBooleanControlDown(control))
				{
					lastMovement = control;
					timer = Time.realtimeSinceStartup;
					break;
				}
			}
		}
		else
		{
			if (MVInputWrapper.GetBooleanControl(lastMovement))
			{
				return;
			}
			doubleTap = false;
			KogamaControls[] array2 = movementControls;
			foreach (KogamaControls control2 in array2)
			{
				if (MVInputWrapper.GetBooleanControl(control2))
				{
					lastMovement = control2;
					doubleTap = true;
					break;
				}
			}
		}
	}
}
