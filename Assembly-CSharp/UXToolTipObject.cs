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

	public void ReadyToolTip(string toolTip, Vector3 position)
	{
		if (uxCamera == null)
		{
			screen = UXUtils.UXScreen;
			UXScreen uXScreen = screen;
			uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, (UXScreen.OnFullScreenChangeDelegate)((bool full) =>
			{
				UpdateScreenSize();
			}));
			uxCamera = screen.Camera;
			uxInputDispatcher = UXUtils.UXInputDispatcher;
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
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		screenSize = uxCamera.ScreenToWorldPoint(vector);
	}

	private void ShowToolTip()
	{
		View.Show();
		toolTipText.Text = currentToolTip;
		ScaleToolTipBox();
		Vector2 vector = toolTipWindow.Size.xy() * screen.Scale / 2f;
		Vector2 offset = toolTipPosition.xy() + vector + BOX_MOUSE_OFFSET;
		DoBoundsFix(ref offset, vector);
		View.transform.position = new Vector3(offset.x, offset.y, View.transform.position.z);
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
		Vector2 vector = toolTipText.Size;
		toolTipWindow.SetSize(new Vector3(vector.x + 1f, vector.y + 1f, 1f));
	}

	public void HandleOnMouseOver()
	{
		if (!(uxInputDispatcher != null) || !uxInputDispatcher.BlockGUIInput)
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
