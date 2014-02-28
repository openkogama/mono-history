using UnityEngine;

public class MVGUIHandler : MonoBehaviour
{
	public GameObject playGUI;

	public GameObject editGUI;

	public GameObject characterEditGUI;

	public void Awake()
	{
		UXUtils.FindGUIObjectOfType<UXScreen>();
		UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		UXUtils.FindGUIObjectOfType<UXToolTip>();
	}
}
