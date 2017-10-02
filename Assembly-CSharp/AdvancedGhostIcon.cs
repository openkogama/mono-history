using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class AdvancedGhostIcon : MonoBehaviour
{
	private const float advancedGhostBodyMaxRadius = 4f;

	private const int ghostIconDistanceBand = 3;

	private bool visible;

	private bool wantsVisible;

	private CullingSubscriberBase cullingSubscriberBase;

	private SphereVolumeIndicator sphereVolumeIndicator;

	public GhostBody GhostBody;

	public float Radius
	{
		set
		{
			sphereVolumeIndicator.Radius = value;
		}
	}

	public void Init(MVAdvancedGhost advancedGhost, MVCubeModelBase body, bool enabledCulling)
	{
		AddSphereVolumeIndicator(advancedGhost.Id);
		body.Changed = (Action<CubeModelChangedEventArgs>)Delegate.Combine(body.Changed, new Action<CubeModelChangedEventArgs>(body_Changed));
		CloneCubeMeshes(body);
		if (enabledCulling)
		{
			SetupCulling(advancedGhost);
			return;
		}
		visible = true;
		wantsVisible = true;
	}

	public void SetGameMode(bool isPlayMode)
	{
		wantsVisible = !isPlayMode;
		SetVisibility();
	}

	private void SetVisibility()
	{
		gameObject.SetActive(wantsVisible && visible);
	}

	private void SetupCulling(MVAdvancedGhost advancedGhost)
	{
		cullingSubscriberBase = new CullingSubscriberBase(4f, transform.position, OnStateChange);
		cullingSubscriberBase.DistanceBandIndex = 3;
		advancedGhost.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(advancedGhost.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(AdvancedGhostOnPositionChanged));
	}

	private void AdvancedGhostOnPositionChanged(object sender, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = positionChangedEventArgs.NewPos;
	}

	private void OnDestroy()
	{
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
	}

	private void OnStateChange(CullingGroupEvent cullingGroupEvent)
	{
		visible = CullingApiWrapper.Visible(cullingGroupEvent, 3);
		SetVisibility();
	}

	private void CloneCubeMeshes(MVCubeModelBase body)
	{
		GhostBody.transform.localScale = Vector3.one;
		foreach (Transform item in GhostBody.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item2 in (IEnumerable)body.ChunkInstances)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(item2.Value.gameObject);
			gameObject.transform.parent = GhostBody.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.gameObject.layer = GhostBody.gameObject.layer;
			gameObject.gameObject.SetActive(GhostBody.gameObject.activeSelf);
		}
		GhostBody.transform.localScale = body.Transform.localScale;
	}

	private void body_Changed(CubeModelChangedEventArgs e)
	{
		CloneCubeMeshes(e.Sender);
	}

	private void AddSphereVolumeIndicator(int Id)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(Id);
		if (worldObjectClient != null)
		{
			sphereVolumeIndicator = UnityEngine.Object.Instantiate(PrefabPool.Instance.RangeVisualizationObject);
			sphereVolumeIndicator.transform.parent = worldObjectClient.GameObject.transform;
			sphereVolumeIndicator.transform.localPosition = Vector3.zero;
			sphereVolumeIndicator.transform.localRotation = Quaternion.identity;
			sphereVolumeIndicator.Initialize(Id);
		}
	}
}
