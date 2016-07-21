using UnityEngine;

public interface ICullingSubscriber
{
	int CullingIndex { get; set; }

	void OnStateChanged(CullingGroupEvent cullingGroupEvent);
}
