using System;
using UnityEngine;

[Serializable]
public class FirstPersonWeaponBob
{
	[Tooltip("Bobs per second.")]
	[SerializeField]
	private float bobFrequency = 1f;

	[Tooltip("Units by time")]
	[SerializeField]
	private AnimationCurve bob;

	[SerializeField]
	private Vector3 bobAxis = new Vector3(0f, 1f, 0f);

	[SerializeField]
	private float bobMultiplier;

	[SerializeField]
	[Tooltip("Degrees by time")]
	private AnimationCurve rotation;

	[SerializeField]
	private Vector3 rotationAxis = new Vector3(0f, 1f, 0f);

	[SerializeField]
	private float rotationMultiplier;

	private Transform weapon;

	private Vector3 weaponPosition;

	private Quaternion weaponRotation;

	public FirstPersonWeaponBob()
	{
		bobAxis.Normalize();
		rotationAxis.Normalize();
	}

	public void Initialize(Transform weapon)
	{
		this.weapon = weapon;
		weaponPosition = weapon.localPosition;
		weaponRotation = weapon.localRotation;
	}

	public void Update()
	{
		float num = Mathf.Abs(MVInputWrapper.GetAxis("Vertical")) + Mathf.Abs(MVInputWrapper.GetAxis("Horizontal"));
		num /= 2f;
		float num2 = bob.Evaluate(Time.time * (bobFrequency / 2f)) * bobMultiplier * num;
		weapon.localPosition = weaponPosition + bobAxis * num2;
		float angle = rotation.Evaluate(Time.time * (bobFrequency / 2f)) * rotationMultiplier * num;
		weapon.localRotation = weaponRotation * Quaternion.AngleAxis(angle, rotationAxis);
	}
}
