using UnityEngine;

public abstract class FirstTimeActivatableMessage : FirstTimeActivatableElementBase
{
	[SerializeField]
	private FirstTimeEventMessage firstTimeEventMessagePrefab;

	[SerializeField]
	private Transform firstTimeMessageParentTransform;

	protected FirstTimeEventMessage firstTimeEventMessage;

	public override bool CanShow => !IsBlocked && gameObject.activeInHierarchy;

	public override void OnShow()
	{
		firstTimeEventMessage = Object.Instantiate(firstTimeEventMessagePrefab);
		if (firstTimeMessageParentTransform == null)
		{
			Debug.LogWarning("firstTimeMessageParentTransform == null, setting this to current script transform");
			firstTimeMessageParentTransform = transform;
		}
		firstTimeEventMessage.transform.SetParent(firstTimeMessageParentTransform, worldPositionStays: false);
		firstTimeEventMessage.FadeIn();
	}

	public void FadeOut()
	{
		firstTimeEventMessage.FadeOut(OnFinished);
	}

	public void DoDisabled()
	{
		OnFinished(firstTimeEventMessage.gameObject);
	}

	private void OnFinished(GameObject firstTimeEventMessage)
	{
		FirstTimeEventManager.SetFirstTimeEvent(firstTimeEvent);
		DestroyMessage();
	}

	protected void DestroyMessage()
	{
		Object.Destroy(firstTimeEventMessage.gameObject);
	}
}
