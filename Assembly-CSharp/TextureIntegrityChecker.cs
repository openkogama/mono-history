using UnityEngine;

public class TextureIntegrityChecker : ScriptableObject
{
	[SerializeField]
	private MaterialPlaneRenderer materialPlaneRenderer;

	public void Initialize()
	{
		materialPlaneRenderer.Initialize();
	}

	public bool VerifyTextureIntegrity()
	{
		return materialPlaneRenderer.VerifyTextureIntegrity();
	}
}
