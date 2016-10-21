using UnityEngine;

public class InGameControls : MonoBehaviour
{
	[SerializeField]
	private InGameButtons inGameButtonsPrefab;

	[SerializeField]
	private GameObject avatarJoystickPrefab;

	public InGameButtons InGameButtons;

	private void Awake()
	{
		InGameButtons = Object.Instantiate(inGameButtonsPrefab);
		InGameButtons.transform.SetParent(transform, worldPositionStays: false);
		GameObject gameObject = Object.Instantiate(avatarJoystickPrefab);
		gameObject.transform.SetParent(transform, worldPositionStays: false);
	}

	private void Update()
	{
	}
}
