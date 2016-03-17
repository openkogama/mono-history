using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class SettingsDropdown : MonoBehaviour
{
	[SerializeField]
	private Dropdown dropdown;

	private string key;

	private List<int> possibleWOData;

	public void Initialize(string key, int value, string[] options, List<int> possibleWOData)
	{
		this.key = key;
		this.possibleWOData = possibleWOData;
		List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
		for (int i = 0; i < options.Length; i++)
		{
			Dropdown.OptionData optionData = new Dropdown.OptionData();
			optionData.text = options[i];
			list.Add(optionData);
		}
		dropdown.options = list;
		int value2 = possibleWOData.IndexOf(value);
		dropdown.value = value2;
		dropdown.onValueChanged.AddListener(ValueChanged);
	}

	private void ValueChanged(int value)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleSettingChanged handler, BaseEventData data) =>
		{
			handler.OnSettingChanged(key, possibleWOData[value]);
		});
	}

	private void Reset()
	{
		dropdown = GetComponent<Dropdown>();
	}
}
