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

	public Vector3 moveDir = (Vector3.up * 4f + Vector3.left).normalized;

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
			transform.position += transform.rotation * moveDir * moveAwaySpeed * Time.deltaTime;
			transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
			transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
			yield return 0;
		}
		Reset();
		rotate.gameObject.SetActive(value: false);
	}

	public void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		Color color = cube.GetComponent<Renderer>().material.GetColor("_Color");
		color.a = alpha;
		cube.GetComponent<Renderer>().material.SetColor("_Color", color);
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
		cube.SetActive(visible);
	}

	private void SetMaterial(Material material)
	{
		cube.GetComponent<Renderer>().material = new Material(material);
	}

	private void SetResetValues()
	{
		rotateRotationSpeed = rotate.rotationSpeed;
		position = transform.position;
		rotation = transform.rotation;
		scale = transform.localScale;
	}

	private void Reset()
	{
		rotate.rotationSpeed = rotateRotationSpeed;
		transform.position = position;
		transform.rotation = rotation;
		transform.localScale = scale;
	}
}
