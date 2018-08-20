using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ShortcutManager : MonoBehaviour, IShortcutKeyRegister, IShortcutKeyUnRegister, IEventSystemHandler
{
	private class ShortcutKey : IEquatable<ShortcutKey>
	{
		public readonly KogamaControls kogamaControl;

		public readonly KeyState keyState;

		public readonly UnityAction callback;

		public ShortcutKey(KogamaControls kogamaControl, KeyState keyState, UnityAction callback)
		{
			this.kogamaControl = kogamaControl;
			this.keyState = keyState;
			this.callback = callback;
		}

		public bool Equals(ShortcutKey other)
		{
			if (other.kogamaControl == kogamaControl)
			{
				return true;
			}
			return false;
		}
	}

	private Dictionary<KogamaControls, Stack<ShortcutKey>> shortCutKeys = new Dictionary<KogamaControls, Stack<ShortcutKey>>();

	public void RegisterShortcutKey(KogamaControls kogamaControl, KeyState keyState, UnityAction callback)
	{
		ShortcutKey t = new ShortcutKey(kogamaControl, keyState, callback);
		if (!shortCutKeys.ContainsKey(kogamaControl))
		{
			shortCutKeys.Add(kogamaControl, new Stack<ShortcutKey>());
		}
		shortCutKeys[kogamaControl].Push(t);
	}

	public void UnRegisterShortcutKey(KogamaControls kogamaControl, KeyState keyState)
	{
		if (!shortCutKeys.ContainsKey(kogamaControl))
		{
			Debug.LogError("Couldn't find shortcut key");
			return;
		}
		shortCutKeys[kogamaControl].Pop();
		if (shortCutKeys[kogamaControl].Count == 0)
		{
			shortCutKeys.Remove(kogamaControl);
		}
	}

	private void Update()
	{
		if (MVInputWrapper.IsShortcutKeysSuppressed)
		{
			return;
		}
		foreach (KeyValuePair<KogamaControls, Stack<ShortcutKey>> shortCutKey in shortCutKeys)
		{
			bool flag = false;
			foreach (ShortcutKey item in shortCutKey.Value)
			{
				switch (item.keyState)
				{
				case KeyState.Down:
					if (MVInputWrapper.GetBooleanControlDown(item.kogamaControl))
					{
						flag = true;
					}
					break;
				case KeyState.Up:
					if (MVInputWrapper.GetBooleanControlUp(item.kogamaControl))
					{
						flag = true;
					}
					break;
				case KeyState.Pressed:
					if (MVInputWrapper.GetBooleanControl(item.kogamaControl))
					{
						flag = true;
					}
					break;
				}
				if (flag)
				{
					item.callback();
					break;
				}
			}
		}
	}
}
