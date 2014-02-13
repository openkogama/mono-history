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
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)m);
		blinkMaterial = (Material)(object)((val is Material) ? val : null);
		blinkMaterial.color = color;
		blinkInterval = interval;
	}

	public void Start(float duration)
	{
		blinkStartTime = Time.time;
		blinkDuration = Mathf.Max(blinkDuration, duration);
	}

	public void Stop()
	{
		blinkDuration = 0f;
		blinkStartTime = 0f;
	}

	public void Draw(Mesh mesh, Transform tfm)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!IsExpired && Mathf.Repeat(Time.time * blinkInterval, 1f) < 0.5f)
		{
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				Graphics.DrawMesh(mesh, tfm.localToWorldMatrix, blinkMaterial, 0, Camera.main, i);
			}
		}
	}
}
