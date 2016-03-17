using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShortcutHandler : MonoBehaviour
{
	[SerializeField]
	private KogamaControls kogamaControl;

	[SerializeField]
	private KeyState keyState;

	[SerializeField]
	private Button button;

	private void Start()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
		{
			x.RegisterShortcutKey(kogamaControl, keyState, Callback);
		});
	}

	private void Reset()
	{
		button = GetComponent<Button>();
	}

	private void Callback()
	{
		button.onClick.Invoke();
	}

	private void OnDestroy()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyUnRegister x, BaseEventData y) =>
		{
			x.UnRegisterShortcutKey(kogamaControl, keyState);
		});
	}
}
