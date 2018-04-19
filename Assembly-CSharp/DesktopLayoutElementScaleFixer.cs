using UnityEngine;
using UnityEngine.UI;

public class DesktopLayoutElementScaleFixer : MonoBehaviour
{
	[SerializeField]
	private LayoutElement element;

	private void Start()
	{
		float num = (float)Screen.height / 1440f;
		float num2 = (float)Screen.width / 1920f;
		element.preferredHeight *= num;
		element.preferredWidth *= num2;
		element.minHeight *= num;
		element.minWidth *= num2;
	}

	private void OnValidate()
	{
		if (element != null)
		{
			element = gameObject.GetComponent<LayoutElement>();
		}
	}
}
