using UnityEngine;

[AddComponentMenu("UX/Elements/Text Field  (numbers only)")]
public class UXNumberField : UXTextField
{
	protected override void InsertCharacter(char c)
	{
		if (char.IsDigit(c))
		{
			base.InsertCharacter(c);
		}
	}
}
