using System.Collections.Generic;
using UnityEngine;

public class MVGUILoading : UXViewScript
{
	public float MaterialChangeTime = 2f;

	public List<Material> materials = new List<Material>();

	private int currentMaterial;

	private float time;

	public GameObject loadingCube;

	public VersionNumber versionNumber;

	private MVGameController gameController;

	public override void OnHide()
	{
		base.OnHide();
		loadingCube.gameObject.active = false;
		((Component)versionNumber.uxText).gameObject.SetActiveRecursively(Debug.isDebugBuild);
	}

	public override void OnShow()
	{
		base.OnShow();
		loadingCube.gameObject.active = true;
		loadingCube.renderer.material = materials[0];
		((Component)versionNumber.uxText).gameObject.SetActiveRecursively(true);
	}

	public void Update()
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)gameController == (Object)null)
		{
			gameController = UXUtils.FindObjectOfType<MVGameController>();
		}
		if ((Object)(object)gameController != (Object)null && gameController.Game != null)
		{
			if (gameController.Game.JoinState == MVJoinState.Playing && View.isVisible)
			{
				View.Hide();
			}
			else if (gameController.Game.JoinState == MVJoinState.Joining && !View.isVisible)
			{
				View.Show();
			}
		}
		if (!View.isVisible)
		{
			return;
		}
		loadingCube.transform.Rotate(Vector3.forward, -60f * Time.deltaTime);
		time += Time.deltaTime;
		if (time > MaterialChangeTime)
		{
			currentMaterial++;
			if (currentMaterial == materials.Count)
			{
				currentMaterial = 0;
			}
			loadingCube.renderer.material = materials[currentMaterial];
			time = 0f;
		}
	}
}
