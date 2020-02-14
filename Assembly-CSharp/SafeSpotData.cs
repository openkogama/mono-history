using UnityEngine;

public struct SafeSpotData(Vector3 pos, Quaternion rot, Vector3 camPos, Quaternion camRot)
{
	public Vector3 Position = pos;

	public Quaternion Rotation = rot;

	public Vector3 CameraPosition = camPos;

	public Quaternion CameraRotation = camRot;
}
