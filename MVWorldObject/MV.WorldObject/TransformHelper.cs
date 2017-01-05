using System.Collections.Generic;
using UnityEngine;

namespace MV.WorldObject;

public static class TransformHelper
{
	public static Vector3 GetPosition(IPosition position)
	{
		return new Vector3(position.PosX, position.PosY, position.PosZ);
	}

	public static Quaternion GetRotation(IRotation rotation)
	{
		return new Quaternion(rotation.RotX, rotation.RotY, rotation.RotZ, rotation.RotW);
	}

	public static Vector3 GetScale(IScale scale)
	{
		return new Vector3(scale.ScaleX, scale.ScaleY, scale.ScaleZ);
	}

	public static Vector3 GetPosition(Dictionary<byte, object> positionData)
	{
		return new Vector3((float)positionData[22], (float)positionData[23], (float)positionData[24]);
	}

	public static Quaternion GetRotation(Dictionary<byte, object> rotationData)
	{
		return new Quaternion((float)rotationData[25], (float)rotationData[26], (float)rotationData[27], (float)rotationData[28]);
	}

	public static Vector3 GetScale(Dictionary<byte, object> scaleData)
	{
		return new Vector3((float)scaleData[29], (float)scaleData[30], (float)scaleData[31]);
	}

	public static void SetPosition(Vector3 position, IPosition positionObject)
	{
		positionObject.PosX = position.x;
		positionObject.PosY = position.y;
		positionObject.PosZ = position.z;
	}

	public static void SetRotation(Quaternion rotation, IRotation rotationObject)
	{
		rotationObject.RotX = rotation.x;
		rotationObject.RotY = rotation.y;
		rotationObject.RotZ = rotation.z;
		rotationObject.RotW = rotation.w;
	}

	public static void SetScale(Vector3 scale, IScale scaleObject)
	{
		scaleObject.ScaleX = scale.x;
		scaleObject.ScaleY = scale.y;
		scaleObject.ScaleZ = scale.z;
	}

	public static void SetPosition(Vector3 position, Dictionary<byte, object> data)
	{
		data.Add(22, position.x);
		data.Add(23, position.y);
		data.Add(24, position.z);
	}

	public static void SetRotation(Quaternion rotation, Dictionary<byte, object> data)
	{
		data.Add(25, rotation.x);
		data.Add(26, rotation.y);
		data.Add(27, rotation.z);
		data.Add(28, rotation.w);
	}

	public static void SetScale(Vector3 scale, Dictionary<byte, object> data)
	{
		data.Add(29, scale.x);
		data.Add(30, scale.y);
		data.Add(31, scale.z);
	}
}
