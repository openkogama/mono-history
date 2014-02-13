using UnityEngine;

public class MovementMap
{
	private Vector3 moveDirection = Vector3.zero;

	private bool jump;

	private bool run;

	private bool use;

	private bool fire;

	private bool drop;

	public Vector3 Direction
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return moveDirection;
		}
	}

	public bool Fire
	{
		get
		{
			return fire;
		}
		set
		{
			fire = value;
		}
	}

	public bool Drop => drop;

	public bool Run => run;

	public bool Jump => jump;

	public bool Use => use;

	public MovementMap()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	public void HandleInputState()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		moveDirection = Vector3.zero;
		if (MVInputWrapper.GetKey((KeyCode)119) || MVInputWrapper.GetKey((KeyCode)273))
		{
			moveDirection += Vector3.forward;
		}
		else if (MVInputWrapper.GetKey((KeyCode)115) || MVInputWrapper.GetKey((KeyCode)274))
		{
			moveDirection -= Vector3.forward;
		}
		if (MVInputWrapper.GetKey((KeyCode)97) || MVInputWrapper.GetKey((KeyCode)276))
		{
			moveDirection -= Vector3.right;
		}
		else if (MVInputWrapper.GetKey((KeyCode)100) || MVInputWrapper.GetKey((KeyCode)275))
		{
			moveDirection += Vector3.right;
		}
		run = MVInputWrapper.GetKey((KeyCode)304) || MVInputWrapper.GetKey((KeyCode)303);
		jump = MVInputWrapper.GetKey((KeyCode)32);
		use = false;
		if (MVInputWrapper.GetKeyUp((KeyCode)101))
		{
			use = true;
		}
		fire = false;
		if (MVInputWrapper.GetKey((KeyCode)323))
		{
			fire = true;
		}
		drop = false;
		if (MVInputWrapper.GetKeyUp((KeyCode)113))
		{
			drop = true;
		}
	}
}
