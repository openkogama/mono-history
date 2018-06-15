using System.Collections.Generic;
using UnityEngine;

namespace MV.WorldObject;

public static class TransformHelper
{
	public static Vector3 GetPosition(IPosition position)
	{
		return new Vector3(ValidateFloat(position.PosX), ValidateFloat(position.PosY), ValidateFloat(position.PosZ));
	}

	public static Quaternion GetRotation(IRotation rotation)
	{
		return new Quaternion(ValidateFloat(rotation.RotX), ValidateFloat(rotation.RotY), ValidateFloat(rotation.RotZ), ValidateFloat(rotation.RotW));
	}

	public static Vector3 GetScale(IScale scale)
	{
		return new Vector3(ValidateFloat(scale.ScaleX), ValidateFloat(scale.ScaleY), ValidateFloat(scale.ScaleZ));
	}

	public static Vector3 GetPosition(Dictionary<byte, object> positionData)
	{
		return new Vector3(ValidateFloat((float)positionData[24]), ValidateFloat((float)positionData[25]), ValidateFloat((float)positionData[26]));
	}

	public static Quaternion GetRotation(Dictionary<byte, object> rotationData)
	{
		return new Quaternion(ValidateFloat((float)rotationData[27]), ValidateFloat((float)rotationData[28]), ValidateFloat((float)rotationData[29]), ValidateFloat((float)rotationData[30]));
	}

	public static Vector3 GetScale(Dictionary<byte, object> scaleData)
	{
		return new Vector3(ValidateFloat((float)scaleData[31]), ValidateFloat((float)scaleData[32]), ValidateFloat((float)scaleData[33]));
	}

	public static void SetPosition(Vector3 position, IPosition positionObject)
	{
		positionObject.PosX = ValidateFloat(position.x);
		positionObject.PosY = ValidateFloat(position.y);
		positionObject.PosZ = ValidateFloat(position.z);
	}

	public static void SetRotation(Quaternion rotation, IRotation rotationObject)
	{
		rotationObject.RotX = ValidateFloat(rotation.x);
		rotationObject.RotY = ValidateFloat(rotation.y);
		rotationObject.RotZ = ValidateFloat(rotation.z);
		rotationObject.RotW = ValidateFloat(rotation.w);
	}

	public static void SetScale(Vector3 scale, IScale scaleObject)
	{
		scaleObject.ScaleX = ValidateFloat(scale.x);
		scaleObject.ScaleY = ValidateFloat(scale.y);
		scaleObject.ScaleZ = ValidateFloat(scale.z);
	}

	public static void SetPosition(Vector3 position, Dictionary<byte, object> data)
	{
		data.Add(24, ValidateFloat(position.x));
		data.Add(25, ValidateFloat(position.y));
		data.Add(26, ValidateFloat(position.z));
	}

	public static void SetRotation(Quaternion rotation, Dictionary<byte, object> data)
	{
		data.Add(27, ValidateFloat(rotation.x));
		data.Add(28, ValidateFloat(rotation.y));
		data.Add(29, ValidateFloat(rotation.z));
		data.Add(30, ValidateFloat(rotation.w));
	}

	public static void SetScale(Vector3 scale, Dictionary<byte, object> data)
	{
		data.Add(31, ValidateFloat(scale.x));
		data.Add(32, ValidateFloat(scale.y));
		data.Add(33, ValidateFloat(scale.z));
	}

	private static float ValidateFloat(float f)
	{
		if (float.IsInfinity(f) || float.IsNaN(f))
		{
			throw new InvalidFloatException();
		}
		return f;
	}
}
