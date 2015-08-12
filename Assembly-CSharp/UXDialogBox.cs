using UnityEngine;

public class UXDialogBox : MonoBehaviour
{
	public delegate void OnDialogResult(UXDialogBox dialog);

	public OnDialogResult OnResult;

	public OnDialogResult OnIntermediateResult;

	public UXDialogFactory DialogFactory { get; protected set; }

	public UXWindow DialogWindow { get; protected set; }

	public UXDialogResult DialogResult { get; protected set; }

	public virtual void OnShowDialog()
	{
		DialogFactory = UXUtils.UXDialogFactory;
		DialogWindow = UXUtils.FindChild(gameObject, "Window").GetComponent<UXWindow>();
	}

	public virtual void OnCloseDialog()
	{
		if (OnResult != null)
		{
			OnResult(this);
		}
	}

	protected void FireIntermediateResult()
	{
		if (OnIntermediateResult != null)
		{
			OnIntermediateResult(this);
		}
	}

	public virtual object GetResult()
	{
		return null;
	}

	public virtual Vector2 GetSize()
	{
		if (DialogWindow == null)
		{
			DialogWindow = UXUtils.FindChild(gameObject, "Window").GetComponent<UXWindow>();
		}
		return DialogWindow.Size;
	}

	public virtual void OnPositiveClose()
	{
		DialogResult = UXDialogResult.Positive;
	}

	public virtual void OnNegativeClose()
	{
		DialogResult = UXDialogResult.Negative;
	}
}
