using UnityEngine;

[AddComponentMenu("UX/Management/Screen")]
public class UXScreen : MonoBehaviour
{
	public delegate void OnResizeDelegate();

	public delegate void OnFullScreenChangeDelegate(bool fullscreen);

	public int horizontalPadding;

	public int verticalPadding;

	public float worldUnit = 6f;

	public float screenUnit = 100f;

	private Vector3[,] positions = new Vector3[3, 3];

	private float scale;

	private int screenWidth;

	private int screenHeight;

	private bool fullscreen;

	public OnResizeDelegate OnResize;

	public OnFullScreenChangeDelegate OnFullScreenChange;

	private Resolution? lastResolution;

	public Camera Camera => ((Component)this).camera;

	public bool Fullscreen
	{
		get
		{
			return fullscreen;
		}
		set
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			if (value != fullscreen)
			{
				fullscreen = value;
				if (fullscreen)
				{
					lastResolution = Screen.currentResolution;
					Resolution currentResolution = Screen.currentResolution;
					int width = currentResolution.width;
					Resolution currentResolution2 = Screen.currentResolution;
					int height = currentResolution2.height;
					Resolution currentResolution3 = Screen.currentResolution;
					Screen.SetResolution(width, height, true, currentResolution3.refreshRate);
				}
				else if (lastResolution.HasValue)
				{
					Resolution value2 = lastResolution.Value;
					int width2 = value2.width;
					Resolution value3 = lastResolution.Value;
					int height2 = value3.height;
					Resolution value4 = lastResolution.Value;
					Screen.SetResolution(width2, height2, false, value4.refreshRate);
				}
				else
				{
					Screen.fullScreen = false;
				}
				fullscreen = Screen.fullScreen;
				if (OnFullScreenChange != null)
				{
					OnFullScreenChange(fullscreen);
				}
			}
		}
	}

	public float Scale => scale;

	public void Start()
	{
		fullscreen = Screen.fullScreen;
		DetectScreenResize();
	}

	public Vector3 GetPosition(UXHorizontal horizontal, UXVertical vertical, float depth)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return positions[(int)horizontal, (int)vertical] + Vector3.forward * depth;
	}

	private void LateUpdate()
	{
		DetectScreenResize();
		DetectFullScreenChange();
	}

	private void DetectScreenResize()
	{
		if (screenWidth != Screen.width || screenHeight != Screen.height)
		{
			Resize();
			screenWidth = Screen.width;
			screenHeight = Screen.height;
		}
	}

	private void DetectFullScreenChange()
	{
		if (fullscreen != Screen.fullScreen)
		{
			Debug.Log((object)("Fullscreen change detected. " + fullscreen + " " + Screen.fullScreen));
			Fullscreen = Screen.fullScreen;
		}
	}

	private void Resize()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f - ((Component)((Component)this).camera).transform.position.z;
		int num2 = horizontalPadding;
		int num3 = Screen.width - horizontalPadding;
		int num4 = Screen.width / 2;
		int num5 = verticalPadding;
		int num6 = Screen.height - verticalPadding;
		int num7 = Screen.height / 2;
		positions[0, 0] = ((Component)this).camera.ScreenToWorldPoint(new Vector3((float)num2, (float)num6, num));
		positions[2, 0] = ((Component)this).camera.ScreenToWorldPoint(new Vector3((float)num3, (float)num6, num));
		positions[0, 2] = ((Component)this).camera.ScreenToWorldPoint(new Vector3((float)num2, (float)num5, num));
		positions[2, 2] = ((Component)this).camera.ScreenToWorldPoint(new Vector3((float)num3, (float)num5, num));
		positions[1, 0] = ((Component)this).camera.ScreenToWorldPoint(new Vector3((float)num4, (float)num6, num));
		positions[1, 2] = ((Component)this).camera.ScreenToWorldPoint(new Vector3((float)num4, (float)num5, num));
		positions[0, 1] = ((Component)this).camera.ScreenToWorldPoint(new Vector3((float)num2, (float)num7, num));
		positions[2, 1] = ((Component)this).camera.ScreenToWorldPoint(new Vector3((float)num3, (float)num7, num));
		scale = ((Component)this).camera.ScreenToWorldPoint(new Vector3((float)num4 + screenUnit, (float)num7, num)).x / worldUnit;
		NotifyResize();
	}

	private void NotifyResize()
	{
		if (OnResize != null)
		{
			OnResize();
		}
	}

	public void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(positions[0, 0], positions[2, 0]);
		Gizmos.DrawLine(positions[0, 0], positions[0, 2]);
		Gizmos.DrawLine(positions[0, 2], positions[2, 2]);
		Gizmos.DrawLine(positions[2, 0], positions[2, 2]);
		Gizmos.color = 0.8f * Color.yellow;
		Gizmos.DrawLine(positions[1, 0], positions[1, 2]);
		Gizmos.DrawLine(positions[0, 1], positions[2, 1]);
	}
}
