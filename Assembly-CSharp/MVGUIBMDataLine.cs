public abstract class MVGUIBMDataLine : UXLine
{
	public delegate void OnRemoveDataLineDelegate(UXLine line);

	public OnRemoveDataLineDelegate OnRemoveDataLine;

	public UXText typeText;

	public UXText nameText;

	public UXText valueText;

	public UXTextButton modifyButton;

	public UXIconButton deleteButton;

	public virtual void BuildLine(string name, object data)
	{
		modifyButton.OnClick = () =>
		{
			ModifyLine();
		};
		deleteButton.OnClick = () =>
		{
			if (OnRemoveDataLine != null)
			{
				OnRemoveDataLine(this);
			}
		};
		nameText.Text = name;
	}

	protected abstract void ModifyLine();

	public string GetName()
	{
		return nameText.Text;
	}

	public abstract object GetValue();
}
