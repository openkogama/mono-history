using UnityEngine;

public class Blinker
{
	private Material blinkMaterial;

	private float blinkStartTime;

	private float blinkDuration;

	private float blinkInterval = 2f;

	public bool IsExpired => blinkStartTime + blinkDuration < Time.time;

	public Blinker(float interval, Material m, Color color)
	{
		blinkMaterial = Object.Instantiate(m);
		blinkMaterial.color = color;
		blinkInterval = interval;
	}

	public void Start(float duration)
	{
		blinkStartTime = Time.time;
		blinkDuration = duration;
	}

	public void Stop()
	{
		blinkDuration = 0f;
		blinkStartTime = 0f;
	}

	public void Draw(Mesh mesh, Transform tfm)
	{
		if (!IsExpired && Mathf.Repeat(Time.time * blinkInterval, 1f) < 0.5f)
		{
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				Graphics.DrawMesh(mesh, tfm.localToWorldMatrix, blinkMaterial, 0, Camera.main, i);
			}
		}
	}

	public void DestroyBlinkerMaterial()
	{
		if (blinkMaterial != null)
		{
			Object.Destroy(blinkMaterial);
		}
	}
}
