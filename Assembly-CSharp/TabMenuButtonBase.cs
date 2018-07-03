using UnityEngine;

public abstract class TabMenuButtonBase : MonoBehaviour
{
	public abstract void Initialize(int tabId, string categoryName);

	public abstract void SetAsDeselected();

	public abstract void SetAsSelected();
}
