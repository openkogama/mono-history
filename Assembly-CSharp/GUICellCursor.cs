using MV.WorldObject;
using UnityEngine;
using UnityEngine.Rendering;

public class GUICellCursor : MonoBehaviour
{
	public class Fader
	{
		private float start;

		private float end;

		private float durationInv;

		private float startMulDurationInv;

		private float startValue;

		private float endValue;

		public Fader(float duration, float startValue, float endValue)
		{
			start = Time.time;
			end = start + duration;
			durationInv = 1f / duration;
			startMulDurationInv = start / duration;
			this.startValue = startValue;
			this.endValue = endValue;
		}

		public bool Update(ref float fadeVal)
		{
			float time = Time.time;
			if (time < end)
			{
				fadeVal = Mathf.Lerp(startValue, endValue, time * durationInv - startMulDurationInv);
				return true;
			}
			fadeVal = endValue;
			return false;
		}
	}

	private IntVector pos = default;

	public float fadeInTime = 0.5f;

	public float fadeOutTime = 0.5f;

	public float scale = 1f;

	private float baseAlpha;

	private float currentAlpha;

	private float prevCursorSetTime;

	public Material material;

	public float lineWidth = 1f;

	public bool lineMesh;

	public bool cubeMesh;

	public bool invertedCubeMesh;

	private Fader fader;

	private FadeState fadeState;

	public float PrevCursorSetTime => prevCursorSetTime;

	public IntVector LocalPos => pos;

	public FadeOverride FadeOverride
	{
		set
		{
			switch (value)
			{
			case FadeOverride.FadeAllIn:
				fader = null;
				SetMaterialOpacity(baseAlpha);
				break;
			case FadeOverride.FadeAllOut:
				fader = null;
				SetMaterialOpacity(0f);
				break;
			}
			fadeState = FadeState.Done;
		}
	}

	public FadeState FadeState
	{
		get
		{
			return fadeState;
		}
		set
		{
			if (fadeState != value)
			{
				switch (value)
				{
				case FadeState.Done:
					fader = null;
					break;
				case FadeState.FadeIn:
					fader = new Fader(fadeInTime * ((baseAlpha - currentAlpha) / baseAlpha), currentAlpha, baseAlpha);
					break;
				case FadeState.FadeOut:
					fader = new Fader(fadeOutTime * (currentAlpha / baseAlpha), currentAlpha, 0f);
					break;
				}
				fadeState = value;
			}
		}
	}

	private void Awake()
	{
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
		MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
		if (component == null && component2 == null)
		{
			component = gameObject.AddComponent<MeshRenderer>();
			component2 = gameObject.AddComponent<MeshFilter>();
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.material = material;
			baseAlpha = component.material.GetColor("_Color").a;
			currentAlpha = baseAlpha;
			Vector3[] array = new Vector3[CubeBase.IdentityCorners.Length];
			for (int i = 0; i < CubeBase.IdentityCorners.Length; i++)
			{
				ref Vector3 reference = ref array[i];
				reference = CubeBase.IdentityCorners[i] * scale;
			}
			if (lineMesh)
			{
				SharedCubeFunctions.AddCubeMeshCubeLines(component2.mesh, array, lineWidth);
			}
			if (cubeMesh)
			{
				SharedCubeFunctions.AddCubeMesh(component2.mesh, array, insideOut: false);
			}
			if (invertedCubeMesh)
			{
				SharedCubeFunctions.AddCubeMesh(component2.mesh, array, insideOut: true);
			}
		}
	}

	public void SetCursorCube(IntVector position, GameObject cubeGameObject)
	{
		pos = position;
		prevCursorSetTime = Time.time;
		Transform transform = gameObject.transform;
		Vector3 position2 = SharedCubeFunctions.LocalToWorld(cubeGameObject, pos);
		gameObject.transform.position = position2;
		transform.position = position2;
		gameObject.transform.localScale = cubeGameObject.transform.localScale;
		gameObject.transform.rotation = cubeGameObject.transform.rotation;
		gameObject.SetActive(value: true);
	}

	private void SetMaterialOpacity(float alphaValue)
	{
		Material[] materials = gameObject.GetComponent<Renderer>().materials;
		foreach (Material material in materials)
		{
			material.hideFlags = HideFlags.DontSave;
			Color color = material.GetColor("_Color");
			currentAlpha = alphaValue;
			color.a = currentAlpha;
			material.SetColor("_Color", color);
		}
	}

	private void Update()
	{
		if (fader != null)
		{
			float fadeVal = 0f;
			if (!fader.Update(ref fadeVal))
			{
				fader = null;
			}
			SetMaterialOpacity(fadeVal);
		}
	}

	public void Destroy()
	{
		Object.Destroy(gameObject);
	}
}
