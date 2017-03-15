using UnityEngine;

public class ProtectedTransform
{
	private readonly Transform transform;

	public Vector3 localPosition
	{
		get
		{
			return transform.localPosition;
		}
		set
		{
			if (MathFunctions.IsVectorFloatsValid(value))
			{
				transform.localPosition = value;
			}
			else
			{
				Debug.LogError("localPosition invalid");
			}
		}
	}

	public Vector3 position
	{
		get
		{
			return transform.position;
		}
		set
		{
			if (MathFunctions.IsVectorFloatsValid(value))
			{
				transform.position = value;
			}
			else
			{
				Debug.LogError("position invalid");
			}
		}
	}

	public Quaternion localRotation
	{
		get
		{
			return transform.localRotation;
		}
		set
		{
			if (MathFunctions.IsQuaternionFloatsValid(value))
			{
				transform.localRotation = value;
			}
			else
			{
				Debug.LogError("localRotation invalid");
			}
		}
	}

	public Quaternion rotation
	{
		get
		{
			return transform.rotation;
		}
		set
		{
			if (MathFunctions.IsQuaternionFloatsValid(value))
			{
				transform.rotation = value;
			}
			else
			{
				Debug.LogError("rotation invalid");
			}
		}
	}

	public Vector3 localScale
	{
		get
		{
			return transform.localScale;
		}
		set
		{
			if (MathFunctions.IsVectorFloatsValid(value))
			{
				transform.localScale = value;
			}
			else
			{
				Debug.LogError("localScale invalid");
			}
		}
	}

	public ProtectedTransform(Transform transform)
	{
		this.transform = transform;
	}
}
