using UnityEngine;

public class FirstTimeEventSetActiveOnEnable : FirstTimeActivatableElementBase
{
	public override bool CanShow => true;

	public override void OnShow()
	{
		Debug.LogWarning("This class does not implement skippable functionality");
		FirstTimeEventManager.SetFirstTimeEvent(firstTimeEvent);
		Object.Destroy(this);
	}
}
