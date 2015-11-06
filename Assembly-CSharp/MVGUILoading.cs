using System.Collections.Generic;
using UnityEngine;

public class MVGUILoading : UXViewScript
{
	public float MaterialChangeTime = 2f;

	public List<Material> materials = new List<Material>();

	private int currentMaterial;

	private float time;

	public GameObject loadingCube;

	public override void OnHide()
	{
		base.OnHide();
		loadingCube.gameObject.SetActive(value: false);
	}

	public override void OnShow()
	{
		base.OnShow();
		loadingCube.gameObject.SetActive(value: true);
		loadingCube.GetComponent<Renderer>().material = materials[0];
	}

	public void Update()
	{
		if (MVGameControllerBase.Game == null)
		{
			return;
		}
		if (MVGameControllerBase.JoinState == MVJoinState.Playing && View.isVisible)
		{
			View.Hide();
		}
		else if (MVGameControllerBase.JoinState != MVJoinState.Playing && !View.isVisible)
		{
			View.Show();
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
			loadingCube.GetComponent<Renderer>().material = materials[currentMaterial];
			time = 0f;
		}
	}
}
