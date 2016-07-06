using System;
using UnityEngine;

public class RotatingShieldLine : MonoBehaviour
{
	[SerializeField]
	private Transform lineTransform;

	[SerializeField]
	private Vector3 lineRotationSpeed;

	[SerializeField]
	private ParticleSystem orbSpawner;

	[SerializeField]
	private int segments;

	[SerializeField]
	private float radius;

	[SerializeField]
	private LineRenderer line;

	private Vector3[] positions = new Vector3[3];

	private int currIndex;

	private bool recreatOrbs;

	public void Initialize()
	{
		CreatePoints();
		for (int i = 0; i < positions.Length; i++)
		{
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
			{
				position = positions[i],
				velocity = Vector3.zero,
				startSize = 0.3f,
				startColor = Color.white,
				startLifetime = 100000f
			};
			orbSpawner.Emit(emitParams, 1);
		}
		recreatOrbs = false;
	}

	public void OnSetHidden()
	{
		recreatOrbs = true;
	}

	public void OnSetVisible()
	{
		if (recreatOrbs)
		{
			float z = 0f;
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				float x = Mathf.Sin((float)Math.PI / 180f * num);
				float y = Mathf.Cos((float)Math.PI / 180f * num);
				Vector3 vector = new Vector3(x, y, z) * radius;
				num += 120f;
				ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
				{
					position = vector * 2.5f,
					velocity = Vector3.zero,
					startSize = 0.3f,
					startColor = Color.white,
					startLifetime = 100000f
				};
				orbSpawner.Emit(emitParams, 1);
			}
		}
	}

	private void CreatePoints()
	{
		float z = 0f;
		float num = 0f;
		for (int i = 0; i < segments + 1; i++)
		{
			float x = Mathf.Sin((float)Math.PI / 180f * num);
			float y = Mathf.Cos((float)Math.PI / 180f * num);
			Vector3 vector = new Vector3(x, y, z) * radius;
			line.SetPosition(i, vector);
			if (i % (segments / 3) == 0 && currIndex <= 2)
			{
				ref Vector3 reference = ref positions[currIndex++];
				reference = vector * 2.5f;
			}
			num += 360f / (float)segments;
		}
	}

	private void Update()
	{
		lineTransform.Rotate(lineRotationSpeed.x, lineRotationSpeed.y, lineRotationSpeed.z, Space.Self);
	}
}
