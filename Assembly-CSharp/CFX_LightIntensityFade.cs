using UnityEngine;

[RequireComponent(typeof(Light))]
public class CFX_LightIntensityFade : MonoBehaviour
{
	public float duration = 1f;

	public float delay;

	public float finalIntensity;

	private float baseIntensity;

	public bool autodestruct;

	private float p_lifetime;

	private float p_delay;

	private void Start()
	{
		baseIntensity = ((Component)this).light.intensity;
	}

	private void OnEnable()
	{
		p_lifetime = 0f;
		p_delay = delay;
		if (delay > 0f)
		{
			((Behaviour)((Component)this).light).enabled = false;
		}
	}

	private void Update()
	{
		if (p_delay > 0f)
		{
			p_delay -= Time.deltaTime;
			if (p_delay <= 0f)
			{
				((Behaviour)((Component)this).light).enabled = true;
			}
		}
		else if (p_lifetime / duration < 1f)
		{
			((Component)this).light.intensity = Mathf.Lerp(baseIntensity, finalIntensity, p_lifetime / duration);
			p_lifetime += Time.deltaTime;
		}
		else if (autodestruct)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
