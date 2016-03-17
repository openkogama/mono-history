using UnityEngine;

public class UIStackDebugElement : MonoBehaviour
{
	[SerializeField]
	private UIPushOption pushOptions;

	[SerializeField]
	private UIGroupFlags group;

	public void Initialize(UIPushOption pushOption = UIPushOption.None, UIGroupFlags group = UIGroupFlags.Default)
	{
		pushOptions = pushOption;
		this.group = group;
	}
}
