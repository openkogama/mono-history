using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVMovingPlatformGroup : MVBlueprintBase
{
	private const string prefabPath = "Prefabs/Blueprints/MovingPlatformGroup";

	private LineRenderer lineRenderer;

	private Hashtable nodeMap;

	private Hashtable nextNodeMap;

	private MVMovingPlatformNode startNode;

	private Dictionary<int, MVMovingPlatformNode> nodeIdToWoMap = new Dictionary<int, MVMovingPlatformNode>();

	private Dictionary<int, int> woIdToNodeIdMap = new Dictionary<int, int>();

	private MVMovingPlatform platform;

	private bool initializeFailed;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	public MVMovingPlatform Platform => platform;

	public MVMovingPlatformGroup(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Blueprints/MovingPlatformGroup", worldObjects)
	{
		nextNodeMap = (Hashtable)blueprintData["NextNodeMap"];
		lineRenderer = GameObject.GetComponentInChildren<LineRenderer>();
		interactionFlags |= InteractionFlags.CanEdit | InteractionFlags.HasSettings;
		previewLayerMask |= LayerFlags.Logic;
	}

	public override void Initialize()
	{
		base.Initialize();
		InitializeCommon();
		foreach (MVMovingPlatformNode value in nodeIdToWoMap.Values)
		{
			value.PositionChanged += WorldObjectClient_PositionChangedHandler;
			value.SelectedChanged += WorldObjectClient_SelectedChangedHandler;
		}
		RotationChanged += WorldObjectClient_RotationChangedHandler;
		interactionFlags |= InteractionFlags.DontPushGroupToSelectionStack;
		if (!HasInteractionFlag(InteractionFlags.IsPreview))
		{
			return;
		}
		AddPreviewBoxesToChildren();
		Platform.CubeModel.PreviewOwnerProfileId = PreviewOwnerProfileId;
		Platform.CubeModel.InteractionFlags |= InteractionFlags.IsPreview;
		foreach (MVMovingPlatformNode value2 in nodeIdToWoMap.Values)
		{
			value2.PreviewOwnerProfileId = PreviewOwnerProfileId;
			value2.InteractionFlags |= InteractionFlags.IsPreview;
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
	}

	private void InitializeCommon()
	{
		foreach (DictionaryEntry item in childIdMap)
		{
			if (!item.Key.ToString().Equals("Platform"))
			{
				int num = Convert.ToInt32(item.Key.ToString());
				int num2 = (int)item.Value;
				MVMovingPlatformNode mVMovingPlatformNode = (MVMovingPlatformNode)GetChild(num2);
				if (mVMovingPlatformNode == null)
				{
					Debug.LogWarning((object)("Platform group " + id + " init - Could not find child node " + num2 + "! If this is a new platform group restart the session. Otherwise it is broken."));
					initializeFailed = true;
					return;
				}
				nodeIdToWoMap[num] = mVMovingPlatformNode;
				woIdToNodeIdMap[mVMovingPlatformNode.Id] = num;
			}
		}
		foreach (DictionaryEntry item2 in nextNodeMap)
		{
			int key = Convert.ToInt32(item2.Key.ToString());
			int key2 = (int)item2.Value;
			MVMovingPlatformNode mVMovingPlatformNode2 = nodeIdToWoMap[key];
			MVMovingPlatformNode mVMovingPlatformNode3 = (mVMovingPlatformNode2.Next = nodeIdToWoMap[key2]);
			mVMovingPlatformNode3.Previous = mVMovingPlatformNode2;
		}
		int key3 = (int)blueprintData["StartNode"];
		startNode = nodeIdToWoMap[key3];
		platform = (MVMovingPlatform)GetChild("Platform");
		if (platform != null)
		{
			platform.MoveBetweenNodes(startNode, startNode.Next);
			ScaleChanged += WorldObjectClient_ScaleChangedHandler;
			UpdateLine();
		}
		else
		{
			Debug.LogWarning((object)("Platform group " + id + " init - Could not find child platform! If this is a new platform group restart the session. Otherwise it is broken."));
			initializeFailed = true;
		}
	}

	private void UpdateLine()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!initializeFailed)
		{
			lineRenderer.useWorldSpace = false;
			lineRenderer.SetVertexCount(nodeIdToWoMap.Count);
			float num = 0.3f * Scale.x;
			lineRenderer.SetWidth(num, num);
			int num2 = 0;
			MVMovingPlatformNode next = startNode;
			lineRenderer.SetPosition(num2, next.Position);
			while (next.Next != null && next.Next != startNode)
			{
				num2++;
				lineRenderer.SetPosition(num2, next.Next.Position);
				next = next.Next;
			}
		}
	}

	private void WorldObjectClient_ScaleChangedHandler(object sender, ScaleChangedEventArgs e)
	{
		UpdateLine();
	}

	private void WorldObjectClient_PositionChangedHandler(object sender, PositionChangedEventArgs e)
	{
		if (sender is MVMovingPlatformNode mVMovingPlatformNode && (platform.Start == mVMovingPlatformNode || platform.End == mVMovingPlatformNode))
		{
			platform.RecalculateMovement();
		}
		UpdateLine();
	}

	private void WorldObjectClient_RotationChangedHandler(object sender, RotationChangedEventArgs e)
	{
		platform.RecalculateMovement();
		UpdateLine();
	}

	private void WorldObjectClient_SelectedChangedHandler(object sender, SelectedEventArgs e)
	{
		platform.PausedMovement = e.Selected;
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(Platform.CubeModel);
		e.EnterGroup(Platform);
		e.SelectWO(Platform.CubeModelID, addToSelection: false);
		e.Event = EditorEvent.EditCubes;
		return true;
	}

	public override void SetWorldObjectToPurchased()
	{
		base.SetWorldObjectToPurchased();
		Platform.CubeModel.SetWorldObjectToPurchased();
		foreach (MVMovingPlatformNode value in nodeIdToWoMap.Values)
		{
			value.SetWorldObjectToPurchased();
		}
	}

	public override void AddPreviewBox()
	{
	}

	private void AddPreviewBoxesToChildren()
	{
		Platform.CubeModel.AddPreviewBox();
		foreach (MVMovingPlatformNode value in nodeIdToWoMap.Values)
		{
			value.AddPreviewBox();
		}
	}
}
