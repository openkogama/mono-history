using System.Collections.Generic;
using UnityEngine;

namespace MV.WorldObject;

public static class TransformHelper
{
	public static Vector3 GetPosition(IPosition position)
	{
		return new Vector3(MVMath.TryValidateFloat(position.PosX), MVMath.TryValidateFloat(position.PosY), MVMath.TryValidateFloat(position.PosZ));
	}

	public static Quaternion GetRotation(IRotation rotation)
	{
		return new Quaternion(MVMath.TryValidateFloat(rotation.RotX), MVMath.TryValidateFloat(rotation.RotY), MVMath.TryValidateFloat(rotation.RotZ), MVMath.TryValidateFloat(rotation.RotW));
	}

	public static Vector3 GetScale(IScale scale)
	{
		return new Vector3(MVMath.TryValidateFloat(scale.ScaleX), MVMath.TryValidateFloat(scale.ScaleY), MVMath.TryValidateFloat(scale.ScaleZ));
	}

	public static Vector3 GetPosition(Dictionary<byte, object> positionData)
	{
		return new Vector3(MVMath.TryValidateFloat((float)positionData[24]), MVMath.TryValidateFloat((float)positionData[25]), MVMath.TryValidateFloat((float)positionData[26]));
	}

	public static Quaternion GetRotation(Dictionary<byte, object> rotationData)
	{
		return new Quaternion(MVMath.TryValidateFloat((float)rotationData[27]), MVMath.TryValidateFloat((float)rotationData[28]), MVMath.TryValidateFloat((float)rotationData[29]), MVMath.TryValidateFloat((float)rotationData[30]));
	}

	public static Vector3 GetScale(Dictionary<byte, object> scaleData)
	{
		return new Vector3(MVMath.TryValidateFloat((float)scaleData[31]), MVMath.TryValidateFloat((float)scaleData[32]), MVMath.TryValidateFloat((float)scaleData[33]));
	}

	public static void SetPosition(Vector3 position, IPosition positionObject)
	{
		positionObject.PosX = MVMath.TryValidateFloat(position.x);
		positionObject.PosY = MVMath.TryValidateFloat(position.y);
		positionObject.PosZ = MVMath.TryValidateFloat(position.z);
	}

	public static void SetRotation(Quaternion rotation, IRotation rotationObject)
	{
		rotationObject.RotX = MVMath.TryValidateFloat(rotation.x);
		rotationObject.RotY = MVMath.TryValidateFloat(rotation.y);
		rotationObject.RotZ = MVMath.TryValidateFloat(rotation.z);
		rotationObject.RotW = MVMath.TryValidateFloat(rotation.w);
	}

	public static void SetScale(Vector3 scale, IScale scaleObject)
	{
		scaleObject.ScaleX = MVMath.TryValidateFloat(scale.x);
		scaleObject.ScaleY = MVMath.TryValidateFloat(scale.y);
		scaleObject.ScaleZ = MVMath.TryValidateFloat(scale.z);
	}

	public static void SetPosition(Vector3 position, Dictionary<byte, object> data)
	{
		data.Add(24, MVMath.TryValidateFloat(position.x));
		data.Add(25, MVMath.TryValidateFloat(position.y));
		data.Add(26, MVMath.TryValidateFloat(position.z));
	}

	public static void SetRotation(Quaternion rotation, Dictionary<byte, object> data)
	{
		data.Add(27, MVMath.TryValidateFloat(rotation.x));
		data.Add(28, MVMath.TryValidateFloat(rotation.y));
		data.Add(29, MVMath.TryValidateFloat(rotation.z));
		data.Add(30, MVMath.TryValidateFloat(rotation.w));
	}

	public static void SetScale(Vector3 scale, Dictionary<byte, object> data)
	{
		data.Add(31, MVMath.TryValidateFloat(scale.x));
		data.Add(32, MVMath.TryValidateFloat(scale.y));
		data.Add(33, MVMath.TryValidateFloat(scale.z));
	}
}
