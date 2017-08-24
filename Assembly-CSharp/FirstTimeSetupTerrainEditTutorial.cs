using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeSetupTerrainEditTutorial : FirstTimeActivatableElementBase
{
	[SerializeField]
	private List<UIPushOption> pushOptions = new List<UIPushOption>();

	[SerializeField]
	private TerrainCubeModelingControllerTutorial terrainCubeModelingControllerTutorialPrefab;

	private CubeModelingStateMachine cubeModelingStateMachine;

	private MaterialsController materialsController;

	public override bool CanShow
	{
		get
		{
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			return !isBlocked && activeInHierarchy;
		}
	}

	public override void OnShow()
	{
		UIPushOption options = UIPushOption.None;
		for (int i = 0; i < pushOptions.Count; i++)
		{
			options |= pushOptions[i];
		}
		TerrainCubeModelingControllerTutorial terrainCubeModelingControllerTutorial = Object.Instantiate(terrainCubeModelingControllerTutorialPrefab);
		terrainCubeModelingControllerTutorial.Initialize(cubeModelingStateMachine, materialsController);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(terrainCubeModelingControllerTutorial.gameObject, options);
		});
		FirstTimeEventManager.SetFirstTimeEvent(firstTimeEvent);
	}

	public void Initialize(CubeModelingStateMachine cubeModelingStateMachine, MaterialsController materialsController)
	{
		this.cubeModelingStateMachine = cubeModelingStateMachine;
		this.materialsController = materialsController;
	}
}
