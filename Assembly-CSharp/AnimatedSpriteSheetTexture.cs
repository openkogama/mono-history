using System.Collections;
using UnityEngine;

public class AnimatedSpriteSheetTexture : MonoBehaviour
{
	[SerializeField]
	private int Columns = 5;

	[SerializeField]
	private int Rows = 5;

	[SerializeField]
	private int emptyRows;

	[SerializeField]
	private int emptyColumnsOnLastRow;

	[SerializeField]
	private int spriteWidth;

	[SerializeField]
	private int spriteHeight;

	[SerializeField]
	private int spriteCellWidth;

	[SerializeField]
	private int spriteCellHeight;

	[SerializeField]
	private int spriteUnusedPixelWidth;

	[SerializeField]
	private int spriteUnusedPixelHeight;

	[SerializeField]
	private float FramesPerSecond = 10f;

	[SerializeField]
	private bool RunOnce = true;

	[SerializeField]
	private Renderer textureRenderer;

	private float cellWidthMultiplier;

	private float cellHeightMultiplier;

	private Material materialCopy;

	public float RunTimeInSeconds => 1f / FramesPerSecond * (float)(Columns * Rows);

	private void Start()
	{
		materialCopy = new Material(textureRenderer.sharedMaterial);
		textureRenderer.material = materialCopy;
		cellHeightMultiplier = (float)(spriteHeight - spriteUnusedPixelHeight) / (float)spriteHeight;
		cellWidthMultiplier = (float)(spriteWidth - spriteUnusedPixelWidth) / (float)spriteWidth;
		Vector2 value = new Vector2(1f / (float)Columns, 1f / (float)Rows);
		textureRenderer.material.SetTextureScale("_MainTex", value);
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateTiling());
	}

	private IEnumerator UpdateTiling()
	{
		float x = 0f;
		float y = 0f;
		Vector2 offset = Vector2.zero;
		do
		{
			for (int i = Rows - 1; i >= 0; i--)
			{
				y = (float)i / (float)Rows;
				y *= cellHeightMultiplier;
				y = (y * (float)spriteHeight + (float)spriteUnusedPixelHeight) / (float)spriteHeight;
				for (int j = 0; j <= Columns - 1; j++)
				{
					if (i < emptyRows)
					{
						break;
					}
					if (i <= emptyRows && j >= Columns - emptyColumnsOnLastRow)
					{
						break;
					}
					x = (float)j / (float)Columns;
					x *= cellWidthMultiplier;
					offset.Set(x, y);
					textureRenderer.material.SetTextureOffset("_MainTex", offset);
					yield return new WaitForSeconds(1f / FramesPerSecond);
				}
			}
		}
		while (!RunOnce);
	}
}
