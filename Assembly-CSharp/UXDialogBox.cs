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
		DialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		DialogWindow = UXUtils.FindChild(((Component)this).gameObject, "Window").GetComponent<UXWindow>();
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)DialogWindow == (Object)null)
		{
			DialogWindow = UXUtils.FindChild(((Component)this).gameObject, "Window").GetComponent<UXWindow>();
		}
		return Vector2.op_Implicit(DialogWindow.Size);
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
