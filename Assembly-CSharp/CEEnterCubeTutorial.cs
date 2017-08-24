using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

internal class CEEnterCubeTutorial : ESStateBase
{
	private float cubeSize = 1f;

	private EditorStateMachine esm;

	public override void Enter(EditorStateMachine e)
	{
		esm = e;
		while (!e.ParentGroupIsRoot && e.ParentGroup.HasInteractionFlag(InteractionFlags.CantAddChildren))
		{
			e.ExitGroup();
		}
		World world = MVGameControllerBase.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
		SharedCubeFunctions.SetLayerRecursively(e.ParentGroup.Transform, select: false);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, cubeSize);
		dictionary.Add((byte)2, (byte)21);
		dictionary.Add((byte)3, MVGameControllerBase.Game.LocalPlayer.ProfileID);
		Dictionary<object, object> customData = dictionary;
		e.Data["IsNewPrototype"] = true;
		MVGameControllerBase.OperationRequests.RequestBuiltInItem(BuiltInItem.CubeModel, MVGameControllerBase.WOCM.RootGroup.Id, customData, Vector3.up * 10f, Quaternion.identity, Vector3.one * cubeSize, localOwner: true, transferOwnershipToServerOnLeave: false);
	}

	private void WOCM_InitializedGameQueryData(object sender, InitializedGameQueryDataEventArgs e)
	{
		World world = MVGameControllerBase.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
		if (MVGameControllerBase.Game.LocalPlayer.ActorNr == e.InstigatorActorNumber)
		{
			esm.SelectWO(e.RootWO.Id, addToSelection: false);
		}
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (e.SingleSelectedWO != null)
		{
			e.Event = EditorEvent.ESEditCubeTutorial;
		}
	}
}
