using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonShowHide : MonoBehaviour
{
	public bool startShowGroup;

	public GameObject[] showHideGroup;

	private bool show;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(() =>
		{
			Click();
		});
		show = startShowGroup;
		ShowHideUpdate();
	}

	public void Click()
	{
		show = !show;
		ShowHideUpdate();
	}

	private void ShowHideUpdate()
	{
		GameObject[] array = showHideGroup;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(show);
		}
	}
}
