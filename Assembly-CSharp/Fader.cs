using UnityEngine;

public class Fader : MonoBehaviour
{
	private Renderer _renderer;

	private Shader normalShader;

	public Shader fadeShader;

	private bool faded;

	public void Awake()
	{
		_renderer = GetComponent<Renderer>();
		normalShader = _renderer.material.shader;
	}

	public void Fade(float fadeFactor)
	{
		if (faded != fadeFactor < 1f)
		{
			Debug.Log("Faded " + faded + " factor " + fadeFactor);
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
