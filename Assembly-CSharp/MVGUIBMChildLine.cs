public class MVGUIBMChildLine : UXLine
{
	public delegate void OnRemoveChildLineDelegate(UXLine line);

	public OnRemoveChildLineDelegate OnRemoveChildLine;

	public UXTextField nameField;

	public UXTextField woIDField;

	public UXIconButton removeButton;

	public void BuildLine(string name, int woid)
	{
		nameField.Text = name;
		woIDField.Text = string.Empty + woid;
		removeButton.OnClick = () =>
		{
			if (OnRemoveChildLine != null)
			{
				OnRemoveChildLine(this);
			}
		};
	}

	public string GetName()
	{
		return nameField.Text;
	}

	public int GetWoId()
	{
		int result = 0;
		int.TryParse(woIDField.Text.Split(new char[1] { '.' })[0], out result);
		return result;
	}
}
