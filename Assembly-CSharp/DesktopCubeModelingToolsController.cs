using UnityEngine;
using UnityEngine.UI;

public class DesktopCubeModelingToolsController : MonoBehaviour
{
	private CubeModelingStateMachine cubeModelingStateMachine;

	[SerializeField]
	private Button defaultTool;

	[SerializeField]
	protected Button editCube;

	[SerializeField]
	protected Button deletecube;

	[SerializeField]
	protected Button paintCube;

	[SerializeField]
	private float disabledAlpha = 0.3f;

	[SerializeField]
	private float enabledAlpha = 1f;

	public CubeModelingEvent ActiveTool { get; private set; }

	private void Awake()
	{
		SetupButtons();
	}

	public CubeModelingStateMachine.HoverType CurrentlyHovered()
	{
		return cubeModelingStateMachine.CurrentlyHovered();
	}

	public virtual void SetupButtons()
	{
		editCube.onClick.AddListener(() =>
		{
			SetToolActive(CubeModelingEvent.EditCubes);
		});
		deletecube.onClick.AddListener(() =>
		{
			SetToolActive(CubeModelingEvent.DeleteCubes);
		});
		paintCube.onClick.AddListener(() =>
		{
			SetToolActive(CubeModelingEvent.PaintCubes);
		});
	}

	public void Initialize(CubeModelingStateMachine cubeModelingStateMachine)
	{
		this.cubeModelingStateMachine = cubeModelingStateMachine;
	}

	private void Start()
	{
		ActiveTool = (CubeModelingEvent)(int)cubeModelingStateMachine.curEvent;
		if (defaultTool == null)
		{
			SetButtonTransparency(ActiveTool);
		}
		else
		{
			defaultTool.onClick.Invoke();
		}
	}

	protected void SetToolActive(CubeModelingEvent cubeTool)
	{
		ActiveTool = cubeTool;
		SetButtonTransparency(cubeTool);
		if (gameObject.activeInHierarchy)
		{
			Select(cubeTool);
		}
	}

	private void SetButtonTransparency(CubeModelingEvent cubeTool)
	{
		SetAllToTransparent();
		switch (cubeTool)
		{
		case CubeModelingEvent.EditCubes:
			SetAlpha(editCube.image, enabledAlpha);
			break;
		case CubeModelingEvent.EditCubes2D:
			SetAlpha(editCube.image, enabledAlpha);
			break;
		case CubeModelingEvent.DeleteCubes:
			SetAlpha(deletecube.image, enabledAlpha);
			break;
		case CubeModelingEvent.PaintCubes:
			SetAlpha(paintCube.image, enabledAlpha);
			break;
		case CubeModelingEvent.SprayCubes:
			break;
		}
	}

	public void Select(CubeModelingEvent tool)
	{
		cubeModelingStateMachine.Event = tool;
	}

	public void SetAllToTransparent()
	{
		SetAlpha(editCube.image, disabledAlpha);
		SetAlpha(deletecube.image, disabledAlpha);
		SetAlpha(paintCube.image, disabledAlpha);
	}

	private void SetAlpha(Image image, float alpha)
	{
		Color color = image.color;
		color.a = alpha;
		image.color = color;
	}
}
