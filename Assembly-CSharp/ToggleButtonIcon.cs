using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ToggleButtonIcon : MonoBehaviour
{
	public Sprite onIcon;

	public Sprite offIcon;

	public bool initialState;

	private bool on;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(() =>
		{
			Click();
		});
		on = initialState;
		SetIcon();
	}

	public void Click()
	{
		on = !on;
		SetIcon();
	}

	private void SetIcon()
	{
		if (on)
		{
			GetComponent<Image>().sprite = onIcon;
		}
		else
		{
			GetComponent<Image>().sprite = offIcon;
		}
	}
}
