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

	private int screenWidthBeforeFullscreen = 940;

	private int screenHeightBeforeFullscreen = 482;

	public Camera Camera => GetComponent<Camera>();

	public bool Fullscreen
	{
		get
		{
			return fullscreen;
		}
		set
		{
			if (value != fullscreen && AllowFullscreenChange())
			{
				fullscreen = value;
				if (fullscreen)
				{
					Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen: true);
				}
				else
				{
					Screen.fullScreen = false;
					Screen.SetResolution(screenWidthBeforeFullscreen, screenHeightBeforeFullscreen, fullscreen: false);
				}
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
		return positions[(int)horizontal, (int)vertical] + Vector3.forward * depth;
	}

	public void Init(int width, int height)
	{
		screenWidthBeforeFullscreen = width;
		screenHeightBeforeFullscreen = height;
	}

	private void LateUpdate()
	{
		DetectScreenResize();
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
			Fullscreen = Screen.fullScreen;
		}
	}

	private bool AllowFullscreenChange()
	{
		bool flag = !(BrowserComm.BrowserName == "Microsoft Internet Explorer") || BrowserComm.BrowserVersion < 8;
		if (!flag)
		{
			UXUtils.FindGUIObjectOfType<MVGUIMenu>().View.Hide();
			UXUtils.UXDialogFactory.CreateDialog(TM._("Fullscreen is not supported in \nInternet Explorer version 8+.\n\nPlease use another browser,\n to use fullscreen."), TM._("Error")).Show();
		}
		return flag;
	}

	private void Resize()
	{
		float z = 0f - GetComponent<Camera>().transform.position.z;
		int num = horizontalPadding;
		int num2 = Screen.width - horizontalPadding;
		int num3 = Screen.width / 2;
		int num4 = verticalPadding;
		int num5 = Screen.height - verticalPadding;
		int num6 = Screen.height / 2;
		positions[0, 0] = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(num, num5, z));
		positions[2, 0] = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(num2, num5, z));
		positions[0, 2] = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(num, num4, z));
		positions[2, 2] = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(num2, num4, z));
		positions[1, 0] = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(num3, num5, z));
		positions[1, 2] = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(num3, num4, z));
		positions[0, 1] = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(num, num6, z));
		positions[2, 1] = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(num2, num6, z));
		scale = GetComponent<Camera>().ScreenToWorldPoint(new Vector3((float)num3 + screenUnit, num6, z)).x / worldUnit;
		NotifyResize();
	}

	private void NotifyResize()
	{
		if (OnResize != null)
		{
			OnResize();
		}
	}
}
