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
			if ((Object)(object)uiText != (Object)null)
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
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>().IsInitialized)
		{
			Object val = Object.Instantiate(Resources.Load("Prefabs/UX/Text"));
			uiText = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
			((Component)uiText).transform.parent = ((Component)this).transform;
			((Component)uiText).transform.localPosition = new Vector3(0f - Alignment.x, 0f, 0f);
			((Component)uiText).transform.localScale = Vector3.one;
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
		uiText.SetAlpha(alpha);
	}

	public override void Update()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
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
			float num2 = Mathf.PingPong(time * 2f, textWidth - Width + 2f) - 2f;
			num2 = Mathf.Clamp(num2, 0f, textWidth - Width);
			num += -1f * num2;
		}
		else if (centerTextIfFit)
		{
			num += (Width - textWidth) / 2f;
		}
		((Component)uiText).transform.localPosition = new Vector3(num, 0f, 0f);
	}
}
