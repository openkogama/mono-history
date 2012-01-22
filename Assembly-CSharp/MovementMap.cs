using System.Collections.Generic;
using UnityEngine;

public class MovementMap
{
	private Dictionary<NetworkInputKeyCodes, Vector3> map = new Dictionary<NetworkInputKeyCodes, Vector3>();

	private bool shift;

	private bool jump;

	public Vector3 Direction
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.zero;
			foreach (Vector3 value in map.Values)
			{
				val += value;
			}
			return val;
		}
	}

	public bool Shift => shift;

	public bool Jump
	{
		get
		{
			return jump;
		}
		set
		{
			jump = value;
		}
	}

	public void Add(NetworkInputKeyCodes keyCode, Vector3 direction)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!map.ContainsKey(keyCode))
		{
			map.Add(keyCode, direction);
		}
	}

	public void Remove(NetworkInputKeyCodes keyCode)
	{
		map.Remove(keyCode);
	}

	public void Reset()
	{
		map.Clear();
		shift = false;
		jump = false;
	}

	public void Update(NetworkInputActionCodes actionCode, NetworkInputKeyCodes keyCode)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (keyCode == NetworkInputKeyCodes.Left && actionCode == NetworkInputActionCodes.Down)
		{
			Add(NetworkInputKeyCodes.Left, Vector3.left);
		}
		if (keyCode == NetworkInputKeyCodes.Right && actionCode == NetworkInputActionCodes.Down)
		{
			Add(NetworkInputKeyCodes.Right, Vector3.right);
		}
		if (keyCode == NetworkInputKeyCodes.Up && actionCode == NetworkInputActionCodes.Down)
		{
			Add(NetworkInputKeyCodes.Up, Vector3.forward);
		}
		if (keyCode == NetworkInputKeyCodes.Down && actionCode == NetworkInputActionCodes.Down)
		{
			Add(NetworkInputKeyCodes.Down, Vector3.back);
		}
		if (actionCode == NetworkInputActionCodes.Up)
		{
			Remove(keyCode);
		}
		if (keyCode == NetworkInputKeyCodes.Shift && actionCode == NetworkInputActionCodes.Up)
		{
			shift = false;
		}
		if (keyCode == NetworkInputKeyCodes.Shift && actionCode == NetworkInputActionCodes.Down)
		{
			shift = true;
		}
		if (keyCode == NetworkInputKeyCodes.Jump && actionCode == NetworkInputActionCodes.Down)
		{
			jump = true;
		}
		if (keyCode == NetworkInputKeyCodes.Jump && actionCode == NetworkInputActionCodes.Up)
		{
			jump = false;
		}
	}
}
