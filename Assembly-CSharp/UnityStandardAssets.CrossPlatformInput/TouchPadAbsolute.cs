using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Image))]
public class TouchPadAbsolute : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public enum AxisOption
	{
		Both,
		OnlyHorizontal,
		OnlyVertical
	}

	public AxisOption axesToUse;

	public string horizontalAxisName = "Horizontal";

	public string verticalAxisName = "Vertical";

	public float Xsensitivity = 10f;

	public float Ysensitivity = 10f;

	private Vector3 m_StartPos;

	private Vector2 m_PreviousDelta;

	private Vector3 m_JoytickOutput;

	private bool m_UseX;

	private bool m_UseY;

	private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

	private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

	private bool m_Dragging;

	private int m_Id = -1;

	private Vector2 m_PreviousTouchPos;

	private Vector3 m_Center;

	private Image m_Image;

	private void Awake()
	{
		CreateVirtualAxes();
	}

	private void Start()
	{
		m_Image = GetComponent<Image>();
		m_Center = m_Image.transform.position;
	}

	private void CreateVirtualAxes()
	{
		m_UseX = axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal;
		m_UseY = axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical;
		if (m_UseX)
		{
			m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
			CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
		}
		if (m_UseY)
		{
			m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
			CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
		}
	}

	private void UpdateVirtualAxes(Vector3 value)
	{
		if (m_UseX)
		{
			m_HorizontalVirtualAxis.Update(value.x);
		}
		if (m_UseY)
		{
			m_VerticalVirtualAxis.Update(value.y);
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		m_Dragging = true;
		m_Id = data.pointerId;
		m_PreviousTouchPos = data.position;
	}

	public void OnPointerUp(PointerEventData data)
	{
		m_Dragging = false;
		m_Id = -1;
		UpdateVirtualAxes(Vector3.zero);
	}

	private void Update()
	{
		if (m_Dragging && Input.touchCount >= m_Id + 1 && m_Id != -1)
		{
			float num = AngleDiffPitch(m_PreviousTouchPos.y, Input.touches[m_Id].position.y);
			float num2 = AngleDiffYaw(m_PreviousTouchPos.x, Input.touches[m_Id].position.x);
			m_PreviousTouchPos = Input.touches[m_Id].position;
			UpdateVirtualAxes(new Vector3(num2 * Xsensitivity, num * Ysensitivity, 0f));
		}
	}

	private float AngleDiffYaw(float prevPosX, float posX)
	{
		Vector3 direction = Camera.main.ScreenPointToRay(new Vector3(prevPosX, 0f)).direction;
		Vector3 direction2 = Camera.main.ScreenPointToRay(new Vector3(posX, 0f)).direction;
		direction = Camera.main.transform.worldToLocalMatrix.MultiplyVector(direction);
		direction2 = Camera.main.transform.worldToLocalMatrix.MultiplyVector(direction2);
		direction.y = 0f;
		direction.Normalize();
		direction2.y = 0f;
		direction2.Normalize();
		return MathFunctions.SignedAngle(direction, direction2, Vector3.up) * 57.29578f;
	}

	private float AngleDiffPitch(float prevPosY, float posY)
	{
		Vector3 direction = Camera.main.ScreenPointToRay(new Vector3(0f, prevPosY)).direction;
		Vector3 direction2 = Camera.main.ScreenPointToRay(new Vector3(0f, posY)).direction;
		direction = Camera.main.transform.worldToLocalMatrix.MultiplyVector(direction);
		direction2 = Camera.main.transform.worldToLocalMatrix.MultiplyVector(direction2);
		direction.x = 0f;
		direction.Normalize();
		direction2.x = 0f;
		direction2.Normalize();
		return MathFunctions.SignedAngle(direction, direction2, Vector3.right) * 57.29578f;
	}

	private void OnDisable()
	{
	}
}
