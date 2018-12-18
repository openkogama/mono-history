using System.Collections.Generic;

public class MVGamePoint : MVLogicObject
{
	public enum GamePointClientState
	{
		Visible,
		PickedUp,
		ReShowing,
		Invisible
	}

	private GamePointClientState state;

	private bool isVisible = true;

	private MVGamePointObject gamePointObject;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public MVGamePoint(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.GamePointPrefab, worldObjects)
	{
		Create();
	}

	private void Create()
	{
		gamePointObject = (MVGamePointObject)component;
		if (gamePointObject.TriggerBoxEvents != null)
		{
			gamePointObject.TriggerBoxEvents.TriggerEnter += Enter;
		}
		SetVisible();
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gamePointObject.VisualObject);
	}

	private void Enter(object sender, TriggerEventArgs e)
	{
		if (state == GamePointClientState.Visible)
		{
			state = GamePointClientState.PickedUp;
			isVisible = false;
			gamePointObject.PickupItem.GreyOut();
			MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, e.instigatorWOID);
		}
	}

	private void SetVisible()
	{
		if (!isVisible)
		{
			gamePointObject.PickupItem.GreyIn();
			isVisible = true;
		}
		state = GamePointClientState.Visible;
	}

	public override void Reset()
	{
		SetVisible();
	}
}
