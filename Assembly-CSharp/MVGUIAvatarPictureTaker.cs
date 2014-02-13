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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected Obj, but got Unknown
		if (_takingPicture)
		{
			_takingPicture = false;
			((MonoBehaviour)this).StopAllCoroutines();
			Object.Destroy((Object)(object)_avatarCloneGO);
		}
		this.OnPictureTaken = OnPictureTaken;
		_layersToRender = avatar.PreviewLayerMask | LayerFlags.Hidden;
		GameObject gameObject = avatar.GameObject;
		Vector3 val = Vector3.up * 1000f + Vector3.right * 20f * (float)(++shotsInProgress);
		_avatarCloneGO = (GameObject)Object.Instantiate((Object)(object)gameObject, val, Quaternion.identity);
		AvatarAccessoryParticles[] componentsInChildren = _avatarCloneGO.GetComponentsInChildren<AvatarAccessoryParticles>();
		foreach (AvatarAccessoryParticles avatarAccessoryParticles in componentsInChildren)
		{
			avatarAccessoryParticles.RootParticleSystem.Simulate(1f, true);
		}
		_avatarCloneGO.transform.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Hidden));
		_boneAnimation = _avatarCloneGO.GetComponent<BoneAnimation>();
		_boneAnimation.PlayAndPauseAt("Idle", 0.01f);
		_takingPicture = true;
		((MonoBehaviour)this).StartCoroutine(TakePictureRoutine());
	}

	private IEnumerator TakePictureRoutine()
	{
		Camera pictureCamera = ((Component)this).gameObject.GetComponent<Camera>();
		pictureCamera.aspect = 1f;
		((Behaviour)pictureCamera).enabled = true;
		RenderTexture renderTexture = new RenderTexture(previewResolution, previewResolution, 16);
		((Texture)renderTexture).filterMode = (FilterMode)1;
		((Object)renderTexture).hideFlags = (HideFlags)4;
		pictureCamera.targetTexture = renderTexture;
		Vector3 cameraPos = ((Component)this).transform.localPosition;
		Quaternion cameraRotation = ((Component)this).transform.localRotation;
		((Component)this).transform.parent = _avatarCloneGO.transform;
		((Component)this).transform.localPosition = cameraPos;
		((Component)this).transform.localRotation = cameraRotation;
		_avatarCloneGO.GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach((MeshRenderer mr) =>
		{
			((Renderer)mr).enabled = true;
		});
		yield return (object)new WaitForEndOfFrame();
		Texture2D pictureTexture = new Texture2D(previewResolution, previewResolution, (TextureFormat)5, false);
		RenderTexture.active = renderTexture;
		pictureTexture.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
		pictureTexture.Apply();
		RenderTexture.active = null;
		Object.DestroyImmediate((Object)(object)renderTexture);
		Object.DestroyImmediate((Object)(object)((Component)this).gameObject);
		Object.DestroyImmediate((Object)(object)_avatarCloneGO);
		if (OnPictureTaken != null)
		{
			OnPictureTaken(pictureTexture);
		}
		shotsInProgress--;
		_takingPicture = false;
	}

	private void OnPreCull()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		LayerUtil.SetLayerRecursively(_avatarCloneGO.transform, LayerMask.op_Implicit((int)_layersToRender), LayerUtil.GetLayerNumber(LayerFlags.Preview));
	}

	private void OnPostRender()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		LayerUtil.SetLayerRecursively(_avatarCloneGO.transform, LayerMask.op_Implicit(8192), LayerUtil.GetLayerNumber(LayerFlags.Hidden));
	}
}
