using System;
using System.Collections;
using MV.Common;
using UnityEngine;

public class RewardCube : MonoBehaviour
{
	public Material[] rewardMaterials;

	public GameObject cube;

	private Rotate rotate;

	private Vector3 position;

	private Quaternion rotation;

	private Vector3 scale;

	private float rotateRotationSpeed;

	public float animationDuration = 1.3f;

	public float rotationAcceleration = 900f;

	public float moveAwaySpeed = 300f;

	public float rotationSpeed = -60f;

	public float scaleSpeed = -2f;

	public Vector3 moveDir;

	public RewardCube()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.up * 4f + Vector3.left;
		moveDir = val.normalized;
		((MonoBehaviour)this)._002Ector();
	}

	private void Awake()
	{
		rotate = cube.GetComponent<Rotate>();
	}

	public IEnumerator DoAnimation()
	{
		SetResetValues();
		moveDir = moveDir.normalized;
		float startTime = Time.time;
		while (Time.time - startTime < animationDuration)
		{
			rotate.rotationSpeed += rotationAcceleration * Time.deltaTime;
			Transform transform = ((Component)this).transform;
			transform.position += ((Component)this).transform.rotation * moveDir * moveAwaySpeed * Time.deltaTime;
			((Component)this).transform.RotateAroundLocal(Vector3.forward, rotationSpeed * ((float)Math.PI / 180f) * Time.deltaTime);
			Transform transform2 = ((Component)this).transform;
			transform2.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
			yield return 0;
		}
		Reset();
		((Component)rotate).gameObject.active = false;
	}

	public void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Color color = cube.renderer.material.GetColor("_Color");
		color.a = alpha;
		cube.renderer.material.SetColor("_Color", color);
	}

	public void SetRewardType(RewardType rewardType)
	{
		Material material = rewardMaterials[0];
		if (rewardType == RewardType.Silver)
		{
			material = rewardMaterials[1];
		}
		SetMaterial(material);
	}

	public void SetVisible(bool visible)
	{
		cube.active = visible;
	}

	private void SetMaterial(Material material)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		cube.renderer.material = new Material(material);
	}

	private void SetResetValues()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		rotateRotationSpeed = rotate.rotationSpeed;
		position = ((Component)this).transform.position;
		rotation = ((Component)this).transform.rotation;
		scale = ((Component)this).transform.localScale;
	}

	private void Reset()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		rotate.rotationSpeed = rotateRotationSpeed;
		((Component)this).transform.position = position;
		((Component)this).transform.rotation = rotation;
		((Component)this).transform.localScale = scale;
	}
}
