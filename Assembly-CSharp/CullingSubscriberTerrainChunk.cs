using MV.WorldObject;
using UnityEngine;

public class CullingSubscriberTerrainChunk : ICullingSubscriber
{
	private readonly IntVector chunkPosition;

	private readonly MVCubeModelBase cubeModelBase;

	private int distanceBand;

	public int CullingIndex { get; set; }

	public CullingSubscriberTerrainChunk(MVCubeModelBase cubeModelBase, IntVector chunkPosition, Bounds bounds)
	{
		this.chunkPosition = chunkPosition;
		this.cubeModelBase = cubeModelBase;
		CullingApiWrapper.Subscribe(this);
		Setup(bounds);
	}

	public void Setup(Bounds bounds)
	{
		float magnitude = bounds.extents.magnitude;
		CullingApiWrapper.spheres[CullingIndex].position = bounds.center;
		CullingApiWrapper.spheres[CullingIndex].radius = magnitude;
		distanceBand = CullingApiWrapper.GetDistanceBand(magnitude);
	}

	public void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		cubeModelBase.ChunkInstances.GetChunk(chunkPosition).renderer.enabled = CullingApiWrapper.Visible(cullingGroupEvent, distanceBand);
	}

	public void HandleChange()
	{
		if (CullingApiWrapper.IsVisible(CullingIndex) && CullingApiWrapper.GetDistance(CullingIndex) <= distanceBand)
		{
			cubeModelBase.ChunkInstances.GetChunk(chunkPosition).renderer.enabled = true;
		}
	}

	public void Destroy()
	{
		CullingApiWrapper.UnSubscribe(this);
	}
}
