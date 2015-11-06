using UnityEngine;

public class GUIDevLoginType : MonoBehaviour
{
	[SerializeField]
	private bool usePromotionalUI;

	[SerializeField]
	private GameObject devUI;

	[SerializeField]
	private GameObject promotionUI;

	private void Start()
	{
		if (usePromotionalUI)
		{
			promotionUI.gameObject.SetActive(value: true);
		}
		else
		{
			devUI.gameObject.SetActive(value: true);
		}
	}
}
