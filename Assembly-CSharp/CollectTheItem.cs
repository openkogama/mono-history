using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class CollectTheItem : MVBlueprintBase
{
	private enum LinePositionIndex
	{
		DropOff,
		Collectable
	}

	private CollectTheItemDropOff dropOff;

	private CollectTheItemCollectable collectable;

	private CollectTheItemLineObject objectPrefab;

	private bool hasInitializedReferences;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.CollectTheItem;

	public int WoKeyInstance { get; set; }

	public int DropOffId => dropOff.Id;

	public CollectTheItem(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.CollectTheItemPrefab, worldObjects)
	{
		objectPrefab = (CollectTheItemLineObject)component;
		InteractionFlags &= ~InteractionFlags.CanClone;
		InteractionFlags |= InteractionFlags.DontPushGroupToSelectionStack;
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupReferences();
	}

	public void SetupReferences()
	{
		if (!hasInitializedReferences)
		{
			hasInitializedReferences = true;
			Dictionary<object, object> dictionary = (Dictionary<object, object>)Data["BlueprintData"];
			Dictionary<object, object> table = (Dictionary<object, object>)dictionary["ChildrenMap"];
			dropOff = (CollectTheItemDropOff)RetrieveWorldObject(table, "CollectTheItemDropOff");
			collectable = (CollectTheItemCollectable)RetrieveWorldObject(table, "CollectTheItemCollectable");
			CollectTheItemDropOff collectTheItemDropOff = dropOff;
			collectTheItemDropOff.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(collectTheItemDropOff.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnDropOffPositionChanged));
			CollectTheItemCollectable collectTheItemCollectable = collectable;
			collectTheItemCollectable.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(collectTheItemCollectable.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnCollectablePositionChanged));
			objectPrefab.lineRenderer.SetPosition(0, dropOff.Transform.position);
			objectPrefab.lineRenderer.SetPosition(1, collectable.Transform.position);
			dropOff.InitializeWithController(this);
			collectable.InitializeWithController(this);
		}
	}

	private void OnDropOffPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		objectPrefab.lineRenderer.SetPosition(0, positionChangedEventArgs.NewPos);
	}

	private void OnCollectablePositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		objectPrefab.lineRenderer.SetPosition(1, positionChangedEventArgs.NewPos);
	}

	public bool GetDoesWoFitDropOff(int keyId)
	{
		return WoKeyInstance == keyId;
	}

	private MVWorldObjectClient RetrieveWorldObject(Dictionary<object, object> table, string id)
	{
		if (table.ContainsKey(id))
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient((int)table[id]);
			if (worldObjectClient == null || (!(worldObjectClient is CollectTheItemDropOff) && !(worldObjectClient is CollectTheItemCollectable)))
			{
				return null;
			}
			return worldObjectClient;
		}
		return null;
	}
}
