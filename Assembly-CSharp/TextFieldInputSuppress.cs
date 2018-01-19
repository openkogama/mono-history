using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class TextFieldInputSuppress : MonoBehaviour
{
	[SerializeField]
	private InputField inputField;

	private void Update()
	{
		if (inputField.isFocused)
		{
			MVInputWrapper.SuppressAllInput();
		}
	}

	private void Reset()
	{
		inputField = GetComponent<InputField>();
	}
}
