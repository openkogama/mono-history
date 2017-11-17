using System;
using System.Collections;
using UnityEngine;

public static class ImageGenerator
{
	public static IEnumerator CreateTextureFromData(MVWorldObjectClient wo, Action<byte[]> callback)
	{
		int textureSize = 512;
		Texture2D previewTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, mipmap: false);
		GameObject previewRoot = new GameObject("Item Preview");
		MVComponent[] mvComponents = wo.GameObject.GetComponentsInChildren<MVComponent>();
		MVComponent[] array = mvComponents;
		foreach (MVComponent mVComponent in array)
		{
			mVComponent.findWorldObjectParent = false;
		}
		GameObject itemCopy = UnityEngine.Object.Instantiate(wo.GameObject);
		MVComponent[] array2 = mvComponents;
		foreach (MVComponent mVComponent2 in array2)
		{
			mVComponent2.findWorldObjectParent = true;
		}
		ObjectPreviewer objectPreviewer = ObjectPreviewer.Create(textureSize, CameraClearFlags.Skybox, wo.PreviewLayerMask, new Vector3(1.2f, 0.1f, 0.3f), previewRoot.transform, itemCopy.transform.position, "Model preview", wo, itemCopy);
		yield return 0;
		RenderTexture.active = objectPreviewer.PreviewTexture;
		previewTexture.ReadPixels(new Rect(0f, 0f, textureSize, textureSize), 0, 0);
		previewTexture.Apply();
		RenderTexture.active = null;
		UnityEngine.Object.Destroy(previewRoot);
		UnityEngine.Object.Destroy(itemCopy);
		objectPreviewer.Destroy();
		byte[] bytes = previewTexture.EncodeToPNG();
		callback((byte[])bytes.Clone());
	}
}
