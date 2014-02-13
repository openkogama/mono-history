using System;
using UnityEngine;

public class UXToolTipObject : UXViewScript
{
	private const float BOX_BUFFER_WIDTH = 0.5f;

	private const float BOX_BUFFER_HEIGHT = 0.5f;

	private const float BOX_MOUSE_OFFSET_Y = 9f;

	private const float BOX_MOUSE_OFFSET_X = 5f;

	private const float BOX_SCREEN_EDGE_BUFFER = 2f;

	private readonly Vector2 BOX_MOUSE_OFFSET = new Vector2(5f, -9f);

	private Camera uxCamera;

	private UXScreen screen;

	private string currentToolTip;

	private Vector3 toolTipPosition;

	private bool showing;

	private float hp;

	private float hpDecreaseRate = 2f;

	private float hpIncreaseRate = 4f;

	public UXWindow toolTipWindow;

	public UXText toolTipText;

	private Vector2 screenSize;

	private UXInputDispatcher uxInputDispatcher;

	public UXToolTipObject()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	}

	public void ReadyToolTip(string toolTip, Vector3 position)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)uxCamera == (Object)null)
		{
			screen = UXUtils.FindGUIObjectOfType<UXScreen>();
			UXScreen uXScreen = screen;
			uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, (UXScreen.OnFullScreenChangeDelegate)((bool full) =>
			{
				UpdateScreenSize();
			}));
			uxCamera = screen.Camera;
			uxInputDispatcher = UXUtils.FindGUIObjectOfType<UXInputDispatcher>();
			UpdateScreenSize();
		}
		currentToolTip = toolTip;
		toolTipPosition = uxCamera.ScreenToWorldPoint(position);
		if (showing)
		{
			ShowToolTip();
		}
	}

	private void UpdateScreenSize()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = new Vector2(uxCamera.GetScreenWidth(), uxCamera.GetScreenHeight());
		screenSize = Vector2.op_Implicit(uxCamera.ScreenToWorldPoint(Vector2.op_Implicit(val)));
	}

	private void ShowToolTip()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		View.Show();
		toolTipText.Text = currentToolTip;
		ScaleToolTipBox();
		Vector2 val = toolTipWindow.Size.xy() * screen.Scale / 2f;
		Vector2 offset = toolTipPosition.xy() + val + BOX_MOUSE_OFFSET;
		DoBoundsFix(ref offset, val);
		((Component)View).transform.position = new Vector3(offset.x, offset.y, ((Component)View).transform.position.z);
		showing = true;
	}

	private void DoBoundsFix(ref Vector2 offset, Vector2 size)
	{
		if (offset.x + size.x + 2f > screenSize.x)
		{
			offset.x = screenSize.x - size.x - 2f;
		}
		if (offset.x - size.x - 2f < 0f - screenSize.x)
		{
			offset.x = 0f - screenSize.x + size.x + 2f;
		}
		if (offset.y - size.y - 2f < 0f - screenSize.y)
		{
			offset.y = 0f - screenSize.y + size.y + 2f;
		}
		if (offset.y + size.y + 2f > screenSize.y)
		{
			offset.y = screenSize.y - size.y - 2f;
		}
	}

	private void ScaleToolTipBox()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.op_Implicit(toolTipText.Size);
		toolTipWindow.SetSize(Vector2.op_Implicit(new Vector3(val.x + 1f, val.y + 1f, 1f)));
	}

	public void HandleOnMouseOver()
	{
		if (!((Object)(object)uxInputDispatcher != (Object)null) || !uxInputDispatcher.BlockGUIInput)
		{
			hp += Time.deltaTime * hpIncreaseRate;
			if (hp > 1f)
			{
				hp = 1f;
			}
		}
	}

	private void Update()
	{
		hp -= Time.deltaTime * hpDecreaseRate;
		if (hp < 0f)
		{
			hp = 0f;
		}
		if (hp >= 0.9f && !showing)
		{
			ShowToolTip();
		}
		else if (hp < 0.1f)
		{
			View.Hide();
			if (showing)
			{
				hp = 0f;
			}
			showing = false;
		}
	}
}
