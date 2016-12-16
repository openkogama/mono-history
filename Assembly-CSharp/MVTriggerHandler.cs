using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MVTriggerHandler : MonoBehaviour
{
	private Dictionary<int, TriggerBoxEvents> triggerBoxEvents = new Dictionary<int, TriggerBoxEvents>();

	private Dictionary<int, TriggerBoxEvents> newTriggerBoxEvents = new Dictionary<int, TriggerBoxEvents>();

	private Collider triggingCollider;

	private bool fixedUpdatedWasExecuted;

	private bool wasResetThisFrame;

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

	private List<int> GetMissingKeysInDictionary(Dictionary<int, TriggerBoxEvents>.KeyCollection keys, Dictionary<int, TriggerBoxEvents> dictionary)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < keys.Count; i++)
		{
			int num = keys.ElementAt(i);
			if (!dictionary.ContainsKey(num))
			{
				list.Add(num);
			}
		}
		return list;
	}

	private void Update()
	{
		if (!fixedUpdatedWasExecuted)
		{
			return;
		}
		fixedUpdatedWasExecuted = false;
		wasResetThisFrame = false;
		List<int> missingKeysInDictionary = GetMissingKeysInDictionary(triggerBoxEvents.Keys, newTriggerBoxEvents);
		List<int> missingKeysInDictionary2 = GetMissingKeysInDictionary(newTriggerBoxEvents.Keys, triggerBoxEvents);
		for (int i = 0; i < missingKeysInDictionary.Count; i++)
		{
			int key = missingKeysInDictionary[i];
			triggerBoxEvents[key].OnMVTriggerExit(TriggingCollider);
			triggerBoxEvents.Remove(key);
		}
		for (int j = 0; j < missingKeysInDictionary2.Count; j++)
		{
			int key2 = missingKeysInDictionary2[j];
			triggerBoxEvents.Add(key2, newTriggerBoxEvents[key2]);
			triggerBoxEvents[key2].OnMVTriggerEnter(TriggingCollider);
			if (wasResetThisFrame)
			{
				break;
			}
		}
		newTriggerBoxEvents.Clear();
	}

	public void Reset()
	{
		wasResetThisFrame = true;
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
