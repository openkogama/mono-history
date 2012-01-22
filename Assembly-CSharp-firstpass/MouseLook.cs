using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseXAndY,
		MouseX,
		MouseY
	}

	public RotationAxes axes;

	public float sensitivityX = 15f;

	public float sensitivityY = 15f;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public Transform character;

	public float distance = 10f;

	public float scrollSpeed = 2f;

	public float verticalOffset = 2f;

	private float rotationY;

	private void Update()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
		if (Input.GetMouseButton(1))
		{
			if (axes == RotationAxes.MouseXAndY)
			{
				float num = ((Component)this).transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
				((Component)this).transform.localEulerAngles = new Vector3(0f - rotationY, num, 0f);
			}
			else if (axes == RotationAxes.MouseX)
			{
				((Component)this).transform.Rotate(0f, Input.GetAxis("Mouse X") * sensitivityX, 0f);
			}
			else
			{
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
				((Component)this).transform.localEulerAngles = new Vector3(0f - rotationY, ((Component)this).transform.localEulerAngles.y, 0f);
			}
			Screen.lockCursor = true;
		}
		else
		{
			Screen.lockCursor = false;
		}
		if ((Object)(object)character != (Object)null)
		{
			Vector3 position = character.position;
			position.y += verticalOffset;
			Vector3 val = ((Component)this).transform.rotation * Vector3.forward;
			position += val.normalized * (0f - distance);
			((Component)this).transform.position = position;
		}
	}

	private void Start()
	{
		if (Object.op_Implicit((Object)(object)((Component)this).rigidbody))
		{
			((Component)this).rigidbody.freezeRotation = true;
		}
	}
}
