using UnityEngine;

public class MVCountingCubeDigit : MonoBehaviour
{
	public Material CountingCubeDigitMaterial;

	private MeshRenderer meshRenderer;

	private int _number;

	public int Number
	{
		get
		{
			return _number;
		}
		set
		{
			_number = value;
			SetMaterialOffset();
		}
	}

	public MeshRenderer MeshRenderer
	{
		get
		{
			if (meshRenderer == null)
			{
				meshRenderer = GetComponent<MeshRenderer>();
			}
			return meshRenderer;
		}
	}

	private void Awake()
	{
		MeshRenderer.material = new Material(CountingCubeDigitMaterial);
	}

	private void SetMaterialOffset()
	{
		Vector2 mainTextureOffset = MeshRenderer.material.mainTextureOffset;
		mainTextureOffset.y = 0f - (float)Number / 10f;
		MeshRenderer.material.mainTextureOffset = mainTextureOffset;
	}
}
