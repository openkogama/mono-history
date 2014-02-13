using UnityEngine;

public class Fader : MonoBehaviour
{
	private Renderer _renderer;

	private Shader normalShader;

	public Shader fadeShader;

	private bool faded;

	public void Awake()
	{
		_renderer = ((Component)this).GetComponent<Renderer>();
		normalShader = _renderer.material.shader;
	}

	public void Fade(float fadeFactor)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (faded != fadeFactor < 1f)
		{
			Debug.Log((object)("Faded " + faded + " factor " + fadeFactor));
		}
		if (fadeFactor < 1f)
		{
			_renderer.material.shader = fadeShader;
			Color color = _renderer.material.color;
			color.a = fadeFactor;
			_renderer.material.color = color;
		}
		else
		{
			_renderer.material.shader = normalShader;
			Color color2 = _renderer.material.color;
			color2.a = 1f;
			_renderer.material.color = color2;
		}
		faded = fadeFactor < 1f;
	}
}
