using UnityEngine;

public class UXScrollingText : UXGUIElement, IUXContainer
{
	private const float WAIT_OFFSET = 2f;

	private UXText uiText;

	public bool centerTextIfFit = true;

	[SerializeField]
	private string _text;

	private float time;

	private bool scroll = true;

	private bool isInitialized;

	private float textWidth;

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			if (uiText != null)
			{
				uiText.Text = _text;
				textWidth = uiText.TextWidth;
				scroll = textWidth > Width;
				time = 0f;
			}
		}
	}

	private void Initialize()
	{
		if (UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>().IsInitialized)
		{
			uiText = Object.Instantiate(PrefabPool.Instance.UXTextObject).GetComponent<UXText>();
			uiText.transform.parent = transform;
			uiText.transform.localPosition = new Vector3(0f - Alignment.x, 0f, 0f);
			uiText.transform.localScale = Vector3.one;
			uiText.SetAlignment(UXHorizontal.Left, verticalAlign);
			uiText.Text = _text;
			textWidth = uiText.TextWidth;
			scroll = textWidth > Width;
			time = 0f;
			isInitialized = true;
		}
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		uiText.SetAlpha(alpha, string.Empty);
	}

	public override void Update()
	{
		base.Update();
		if (!isInitialized)
		{
			Initialize();
			return;
		}
		time += Time.deltaTime;
		float num = 0f - Alignment.x;
		if (scroll)
		{
			float value = Mathf.PingPong(time * 2f, textWidth - Width + 2f) - 2f;
			value = Mathf.Clamp(value, 0f, textWidth - Width);
			num += -1f * value;
		}
		else if (centerTextIfFit)
		{
			num += (Width - textWidth) / 2f;
		}
		uiText.transform.localPosition = new Vector3(num, 0f, 0f);
	}
}
