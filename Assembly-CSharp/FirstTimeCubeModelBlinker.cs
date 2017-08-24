using System.Collections.Generic;
using UnityEngine;

public class FirstTimeCubeModelBlinker : BlinkerBase
{
	private MVCubeModelBase targetCubeModelBase;

	public void Initialize(Material material, Camera targetCamera, MVCubeModelBase targetCubeModelBase)
	{
		this.targetCubeModelBase = targetCubeModelBase;
		base.targetCamera = targetCamera;
		layerMask = LayerMask.NameToLayer("CamRotateTarget");
		blinkMaterial = material;
		blinkers = new Dictionary<BlinkType, Blinker> { 
		{
			BlinkType.OnBoardingCubeModelSuccess,
			new Blinker(2f, blinkMaterial, new Color(0f, 1f, 0f, 0.2f))
		} };
	}

	protected override void BeforeDraw()
	{
		meshFilters = targetCubeModelBase.MeshFilters;
	}
}
