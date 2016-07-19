using UnityEngine;
using UnityEngine.EventSystems;

public interface IFadeParent : IEventSystemHandler
{
	void AddFadeMaterial(Material addRenderer);

	void RemoveFadeMaterial(Material removeMaterial);
}
