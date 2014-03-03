using System.Collections.Generic;
using UnityEngine;

public class MVGUILoading : UXViewScript
{
	public float MaterialChangeTime = 2f;

	public List<Material> materials = new List<Material>();

	private int currentMaterial;

	private float time;

	public GameObject loadingCube;

	private MVGameController gameController;

	public override void OnHide()
	{
		base.OnHide();
		loadingCube.gameObject.active = false;
	}

	public override void OnShow()
	{
		base.OnShow();
		loadingCube.gameObject.active = true;
		loadingCube.renderer.material = materials[0];
	}

	public void Update()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)gameController == (Object)null)
		{
			gameController = MVGameController.Instance;
		}
		if ((Object)(object)gameController != (Object)null && gameController.Game != null)
		{
			if (gameController.Game.JoinState == MVJoinState.Playing && View.isVisible)
			{
				View.Hide();
			}
			else if (gameController.Game.JoinState != MVJoinState.Playing && !View.isVisible)
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
