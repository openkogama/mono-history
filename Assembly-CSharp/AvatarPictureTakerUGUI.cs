using System;
using System.Linq;
using UnityEngine;

public class AvatarPictureTakerUGUI : MonoBehaviour
{
	public int previewResolution = 128;

	private GameObject _avatarCloneGO;

	private BoneAnimation _boneAnimation;

	private LayerFlags _layersToRender;

	[SerializeField]
	private Camera pictureCamera;

	private Vector3 pictureLocation = new Vector3(20f, 1000f, 0f);

	public void TakePicture(MVWorldObjectClient avatar, int avatarIndex, Action<int, Texture2D> OnPictureTaken)
	{
		_layersToRender = avatar.PreviewLayerMask | LayerFlags.Hidden;
		GameObject original = avatar.GameObject;
		_avatarCloneGO = (GameObject)UnityEngine.Object.Instantiate(original, pictureLocation, Quaternion.identity);
		AvatarAccessoryParticles[] componentsInChildren = _avatarCloneGO.GetComponentsInChildren<AvatarAccessoryParticles>();
		foreach (AvatarAccessoryParticles avatarAccessoryParticles in componentsInChildren)
		{
			avatarAccessoryParticles.RootParticleSystem.Simulate(1f, withChildren: true);
		}
		_avatarCloneGO.transform.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Hidden));
		_boneAnimation = _avatarCloneGO.GetComponent<BoneAnimation>();
		_boneAnimation.PlayAndPauseAt("Idle", 0.01f);
		pictureCamera.aspect = 1f;
		pictureCamera.enabled = true;
		RenderTexture renderTexture = new RenderTexture(previewResolution, previewResolution, 16);
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.hideFlags = HideFlags.DontSave;
		pictureCamera.targetTexture = renderTexture;
		Vector3 localPosition = transform.localPosition;
		Quaternion localRotation = transform.localRotation;
		transform.parent = _avatarCloneGO.transform;
		transform.localPosition = localPosition;
		transform.localRotation = localRotation;
		_avatarCloneGO.GetComponentsInChildren<MeshRenderer>(includeInactive: true).ToList().ForEach((MeshRenderer mr) =>
		{
			mr.enabled = true;
		});
		pictureCamera.Render();
		pictureCamera.enabled = false;
		Texture2D texture2D = new Texture2D(previewResolution, previewResolution, TextureFormat.ARGB32, mipmap: false);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		renderTexture = null;
		transform.parent = null;
		transform.position = localPosition;
		transform.rotation = localRotation;
		UnityEngine.Object.Destroy(_avatarCloneGO);
		OnPictureTaken?.Invoke(avatarIndex, texture2D);
	}

	private void OnPreCull()
	{
		LayerUtil.SetLayerRecursively(_avatarCloneGO.transform, (int)_layersToRender, LayerUtil.GetLayerNumber(LayerFlags.Preview));
	}

	private void OnPostRender()
	{
		LayerUtil.SetLayerRecursively(_avatarCloneGO.transform, 8192, LayerUtil.GetLayerNumber(LayerFlags.Hidden));
	}

	private void OnDestroy()
	{
		RenderTexture targetTexture = pictureCamera.targetTexture;
		pictureCamera.targetTexture = null;
		targetTexture.Release();
	}
}
