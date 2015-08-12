using UnityEngine;

public class RollingNumberDigit : MonoBehaviour
{
	public UXPlane uxPlaneNumbers;

	public UXPlane uxPlaneOverlay;

	public Material overlayMat;

	public Material numbersMat;

	public int textureTileSize = 64;

	public int targetNumber;

	public int prevTargetNumber;

	private bool useOverlay = true;

	private float rollingSpeed = 6.8f;

	public int Number
	{
		get
		{
			return targetNumber;
		}
		set
		{
			prevTargetNumber = targetNumber;
			targetNumber = Mathf.Clamp(value, 0, 9);
		}
	}

	public bool UseOverlay
	{
		get
		{
			return useOverlay;
		}
		set
		{
			useOverlay = value;
			if (!value)
			{
				uxPlaneOverlay.GetComponent<Renderer>().enabled = false;
			}
			else
			{
				uxPlaneOverlay.GetComponent<Renderer>().enabled = uxPlaneNumbers.GetComponent<Renderer>().enabled;
			}
		}
	}

	public int DisplayWidth => (int)uxPlaneNumbers.Width;

	private void Start()
	{
		uxPlaneOverlay.SetMaterial(overlayMat);
		uxPlaneNumbers.SetMaterial(numbersMat);
	}

	public void Update()
	{
		uxPlaneNumbers.GetComponent<Renderer>().material.mainTextureOffset = GetTexOffset();
	}

	public Vector2 GetTexOffset()
	{
		Vector2 mainTextureOffset = uxPlaneNumbers.GetComponent<Renderer>().material.mainTextureOffset;
		float y = mainTextureOffset.y;
		float num = (9f - (float)targetNumber) / 10f;
		if (y == num)
		{
			return mainTextureOffset;
		}
		float num2 = Mathf.Lerp(y, num, rollingSpeed * Time.deltaTime);
		if (num > y && num - y > 0.6f)
		{
			num2++;
		}
		if (num < y && y - num > 0.6f)
		{
			num2--;
		}
		return new Vector2(0f, num2);
	}
}
