using System.Collections.Generic;
using UnityEngine;

public class MVTriggerHandler : MonoBehaviour
{
	private Dictionary<int, TriggerBoxEvents> triggerBoxEvents = new Dictionary<int, TriggerBoxEvents>();

	private Dictionary<int, TriggerBoxEvents> newTriggerBoxEvents = new Dictionary<int, TriggerBoxEvents>();

	private Collider triggingCollider;

	private bool fixedUpdatedWasExecuted;

	public Collider TriggingCollider
	{
		get
		{
			if (triggingCollider == null)
			{
				triggingCollider = GetComponent<Collider>();
			}
			return triggingCollider;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!enabled)
		{
			return;
		}
		TriggerBoxEvents component = other.GetComponent<TriggerBoxEvents>();
		if (component != null)
		{
			int instanceID = component.gameObject.GetInstanceID();
			if (!newTriggerBoxEvents.ContainsKey(instanceID))
			{
				newTriggerBoxEvents.Add(instanceID, component);
			}
		}
	}

	private void FixedUpdate()
	{
		fixedUpdatedWasExecuted = true;
	}

	private void Update()
	{
		if (!fixedUpdatedWasExecuted)
		{
			return;
		}
		fixedUpdatedWasExecuted = false;
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, TriggerBoxEvents> triggerBoxEvent in triggerBoxEvents)
		{
			if (!newTriggerBoxEvents.ContainsKey(triggerBoxEvent.Key))
			{
				list.Add(triggerBoxEvent.Key);
			}
		}
		List<int> list2 = new List<int>();
		foreach (KeyValuePair<int, TriggerBoxEvents> newTriggerBoxEvent in newTriggerBoxEvents)
		{
			if (!triggerBoxEvents.ContainsKey(newTriggerBoxEvent.Key))
			{
				list2.Add(newTriggerBoxEvent.Key);
			}
		}
		foreach (int item in list)
		{
			triggerBoxEvents[item].OnMVTriggerExit(TriggingCollider);
			triggerBoxEvents.Remove(item);
		}
		foreach (int item2 in list2)
		{
			triggerBoxEvents.Add(item2, newTriggerBoxEvents[item2]);
			triggerBoxEvents[item2].OnMVTriggerEnter(TriggingCollider);
		}
		newTriggerBoxEvents.Clear();
	}

	public void Reset()
	{
		foreach (KeyValuePair<int, TriggerBoxEvents> triggerBoxEvent in triggerBoxEvents)
		{
			triggerBoxEvent.Value.OnMVTriggerExit(TriggingCollider);
		}
		newTriggerBoxEvents.Clear();
		triggerBoxEvents.Clear();
	}

	private void OnDisable()
	{
		Reset();
	}

	private void OnDestroy()
	{
		Reset();
	}

	private void Start()
	{
		if (TriggingCollider == null)
		{
			Debug.LogError("Did not find collider");
		}
	}
}
