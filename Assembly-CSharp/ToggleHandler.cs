using UnityEngine;
using UnityEngine.Events;

public abstract class ToggleHandler : MonoBehaviour
{
	public abstract void ExecuteToggleState(bool toggleState, UnityAction<bool> toggleCallback);
}
