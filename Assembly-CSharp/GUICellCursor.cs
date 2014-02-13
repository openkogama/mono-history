using MV.WorldObject;
using UnityEngine;

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
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer val = ((Component)this).gameObject.AddComponent<MeshRenderer>();
		((Renderer)val).castShadows = false;
		((Renderer)val).receiveShadows = false;
		MeshFilter val2 = ((Component)this).gameObject.AddComponent<MeshFilter>();
		((Renderer)val).material = material;
		baseAlpha = ((Renderer)val).material.GetColor("_Color").a;
		currentAlpha = baseAlpha;
		Vector3[] array = new Vector3[CubeBase.IdentityCorners.Length];
		for (int i = 0; i < CubeBase.IdentityCorners.Length; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = CubeBase.IdentityCorners[i] * scale;
		}
		if (lineMesh)
		{
			SharedCubeFunctions.AddCubeMeshCubeLines(val2.mesh, array, lineWidth);
		}
		if (cubeMesh)
		{
			SharedCubeFunctions.AddCubeMesh(val2.mesh, array, insideOut: false);
		}
		if (invertedCubeMesh)
		{
			SharedCubeFunctions.AddCubeMesh(val2.mesh, array, insideOut: true);
		}
	}

	public void SetCursorCube(IntVector position, GameObject cubeGameObject)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		pos = position;
		prevCursorSetTime = Time.time;
		Transform transform = ((Component)this).gameObject.transform;
		Vector3 position2 = SharedCubeFunctions.LocalToWorld(cubeGameObject, pos);
		((Component)this).gameObject.transform.position = position2;
		transform.position = position2;
		((Component)this).gameObject.transform.localScale = cubeGameObject.transform.localScale;
		((Component)this).gameObject.transform.rotation = cubeGameObject.transform.rotation;
		((Component)this).gameObject.active = true;
	}

	private void SetMaterialOpacity(float alphaValue)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Material[] materials = ((Component)this).gameObject.renderer.materials;
		foreach (Material val in materials)
		{
			((Object)val).hideFlags = (HideFlags)4;
			Color color = val.GetColor("_Color");
			currentAlpha = alphaValue;
			color.a = currentAlpha;
			val.SetColor("_Color", color);
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
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
