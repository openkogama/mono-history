using System.Collections;
using System.Linq;
using UnityEngine;

public class MVGUIAvatarPictureTaker : MonoBehaviour
{
	public delegate void OnPictureTakenDelegate(Texture2D picture);

	private OnPictureTakenDelegate OnPictureTaken;

	public int previewResolution = 128;

	private bool _takingPicture;

	private GameObject _avatarCloneGO;

	private BoneAnimation _boneAnimation;

	private LayerFlags _layersToRender;

	private static int shotsInProgress;

	public void TakePicture(MVWorldObjectClient avatar, OnPictureTakenDelegate OnPictureTaken)
	{
		if (_takingPicture)
		{
			_takingPicture = false;
			StopAllCoroutines();
			Object.Destroy(_avatarCloneGO);
		}
		this.OnPictureTaken = OnPictureTaken;
		_layersToRender = avatar.PreviewLayerMask | LayerFlags.Hidden;
		GameObject original = avatar.GameObject;
		Vector3 position = Vector3.up * 1000f + Vector3.right * 20f * ++shotsInProgress;
		_avatarCloneGO = (GameObject)Object.Instantiate(original, position, Quaternion.identity);
		AvatarAccessoryParticles[] componentsInChildren = _avatarCloneGO.GetComponentsInChildren<AvatarAccessoryParticles>();
		foreach (AvatarAccessoryParticles avatarAccessoryParticles in componentsInChildren)
		{
			avatarAccessoryParticles.RootParticleSystem.Simulate(1f, withChildren: true);
		}
		_avatarCloneGO.transform.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Hidden));
		_boneAnimation = _avatarCloneGO.GetComponent<BoneAnimation>();
		_boneAnimation.PlayAndPauseAt("Idle", 0.01f);
		_takingPicture = true;
		StartCoroutine(TakePictureRoutine());
	}

	private IEnumerator TakePictureRoutine()
	{
		Camera pictureCamera = gameObject.GetComponent<Camera>();
		pictureCamera.aspect = 1f;
		pictureCamera.enabled = true;
		RenderTexture renderTexture = new RenderTexture(previewResolution, previewResolution, 16)
		{
			filterMode = FilterMode.Bilinear,
			hideFlags = HideFlags.DontSave
		};
		pictureCamera.targetTexture = renderTexture;
		Vector3 cameraPos = transform.localPosition;
		Quaternion cameraRotation = transform.localRotation;
		transform.parent = _avatarCloneGO.transform;
		transform.localPosition = cameraPos;
		transform.localRotation = cameraRotation;
		_avatarCloneGO.GetComponentsInChildren<MeshRenderer>(includeInactive: true).ToList().ForEach((MeshRenderer mr) =>
		{
			mr.enabled = true;
		});
		yield return new WaitForEndOfFrame();
		Texture2D pictureTexture = new Texture2D(previewResolution, previewResolution, TextureFormat.ARGB32, mipmap: false);
		RenderTexture.active = renderTexture;
		pictureTexture.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
		pictureTexture.Apply();
		RenderTexture.active = null;
		Object.DestroyImmediate(renderTexture);
		Object.DestroyImmediate(gameObject);
		Object.DestroyImmediate(_avatarCloneGO);
		if (OnPictureTaken != null)
		{
			OnPictureTaken(pictureTexture);
		}
		shotsInProgress--;
		_takingPicture = false;
	}

	private void OnPreCull()
	{
		LayerUtil.SetLayerRecursively(_avatarCloneGO.transform, (int)_layersToRender, LayerUtil.GetLayerNumber(LayerFlags.Preview));
	}

	private void OnPostRender()
	{
		LayerUtil.SetLayerRecursively(_avatarCloneGO.transform, 8192, LayerUtil.GetLayerNumber(LayerFlags.Hidden));
	}
}
