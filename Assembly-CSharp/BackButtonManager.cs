using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class BackButtonManager
{
	private class BackButtonSubscriber
	{
		public BackButtonHandler handler;

		public KogamaControls button;

		public KeyState state;

		public UnityAction callback;

		public BackButtonSubscriber(BackButtonHandler handler, KogamaControls button, KeyState state, UnityAction callback)
		{
			this.handler = handler;
			this.button = button;
			this.state = state;
			this.callback = callback;
		}
	}

	private static List<BackButtonSubscriber> subscribers = new List<BackButtonSubscriber>();

	public static void Update()
	{
		if (subscribers.Count <= 0)
		{
			return;
		}
		BackButtonSubscriber backButtonSubscriber = subscribers[subscribers.Count - 1];
		switch (backButtonSubscriber.state)
		{
		case KeyState.Down:
			if (MVInputWrapper.GetBooleanControlDown(backButtonSubscriber.button))
			{
				backButtonSubscriber.callback();
			}
			break;
		case KeyState.Up:
			if (MVInputWrapper.GetBooleanControlUp(backButtonSubscriber.button))
			{
				backButtonSubscriber.callback();
			}
			break;
		case KeyState.Pressed:
			if (MVInputWrapper.GetBooleanControl(backButtonSubscriber.button))
			{
				backButtonSubscriber.callback();
			}
			break;
		}
	}

	public static void Subscribe(BackButtonHandler handler, KogamaControls button, KeyState state, UnityAction callback)
	{
		if (!ContainsHandler(handler))
		{
			subscribers.Add(new BackButtonSubscriber(handler, button, state, callback));
		}
	}

	public static void Unsubscribe(BackButtonHandler handler)
	{
		for (int num = subscribers.Count - 1; num > 0; num--)
		{
			if (subscribers[num].handler == handler)
			{
				subscribers.RemoveAt(num);
				break;
			}
		}
	}

	public static void PostDestroyCleanup()
	{
		if (subscribers.Count > 0)
		{
			Debug.LogWarningFormat("{0} subscribers are never unsubscribing.", subscribers.Count);
			subscribers.Clear();
		}
	}

	private static bool ContainsHandler(BackButtonHandler handler)
	{
		for (int i = 0; i < subscribers.Count; i++)
		{
			if (subscribers[i].handler == handler)
			{
				return true;
			}
		}
		return false;
	}
}
