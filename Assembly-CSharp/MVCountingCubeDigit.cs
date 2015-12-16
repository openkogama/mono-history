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

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material = new Material(CountingCubeDigitMaterial);
	}

	private void SetMaterialOffset()
	{
		Vector2 mainTextureOffset = meshRenderer.material.mainTextureOffset;
		mainTextureOffset.y = 0f - (float)Number / 10f;
		meshRenderer.material.mainTextureOffset = mainTextureOffset;
	}
}
