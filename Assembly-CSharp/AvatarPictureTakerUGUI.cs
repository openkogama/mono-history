using System;
using System.Linq;
using UnityEngine;

public class AvatarPictureTakerUGUI : MonoBehaviour
{
	public int previewResolution = 128;

	private GameObject _avatarCloneGO;

	private BoneAnimation _boneAnimation;

	private bool isCurrentAvatarBody;

	[SerializeField]
	private Camera pictureCamera;

	public void TakePicture(MVWorldObjectClient avatar, int avatarIndex, Action<int, Texture2D> OnPictureTaken, bool isCurrentBody)
	{
		isCurrentAvatarBody = isCurrentBody;
		_avatarCloneGO = avatar.GameObject;
		if (!isCurrentBody)
		{
			AvatarAccessoryParticles[] componentsInChildren = _avatarCloneGO.GetComponentsInChildren<AvatarAccessoryParticles>();
			foreach (AvatarAccessoryParticles avatarAccessoryParticles in componentsInChildren)
			{
				avatarAccessoryParticles.RootParticleSystem.Simulate(1f, withChildren: true);
			}
		}
		if (!isCurrentBody)
		{
			_boneAnimation = _avatarCloneGO.GetComponent<BoneAnimation>();
			_boneAnimation.PlayAndPauseAt("Idle", 0.01f);
		}
		pictureCamera.cullingMask = 540928;
		pictureCamera.aspect = 1f;
		pictureCamera.enabled = true;
		RenderTexture temporary = RenderTexture.GetTemporary(previewResolution, previewResolution, 16);
		temporary.filterMode = FilterMode.Bilinear;
		temporary.hideFlags = HideFlags.DontSave;
		pictureCamera.targetTexture = temporary;
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
		Texture2D texture2D = new Texture2D(previewResolution, previewResolution, TextureFormat.ARGB32, mipChain: false);
		RenderTexture.active = temporary;
		texture2D.ReadPixels(new Rect(0f, 0f, temporary.width, temporary.height), 0, 0);
		texture2D.Apply();
		temporary = null;
		CleanupRenderTex();
		transform.parent = null;
		transform.position = localPosition;
		transform.rotation = localRotation;
		OnPictureTaken?.Invoke(avatarIndex, texture2D);
	}

	private void OnPreCull()
	{
		SharedCubeFunctions.SetLayerRecursively(_avatarCloneGO.transform, select: true);
		_avatarCloneGO.GetComponentsInChildren<MeshRenderer>(includeInactive: true).ToList().ForEach((MeshRenderer mr) =>
		{
			mr.enabled = true;
		});
	}

	private void OnPostRender()
	{
		if (!isCurrentAvatarBody)
		{
			_avatarCloneGO.GetComponentsInChildren<MeshRenderer>(includeInactive: true).ToList().ForEach((MeshRenderer mr) =>
			{
				mr.enabled = false;
			});
			SharedCubeFunctions.SetLayerRecursively(_avatarCloneGO.transform, select: false);
		}
	}

	private void CleanupRenderTex()
	{
		pictureCamera.targetTexture = null;
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(active);
	}
}
