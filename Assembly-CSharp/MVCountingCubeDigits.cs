using UnityEngine;

public class MVCountingCubeDigits : MonoBehaviour
{
	public MVCountingCubeDigit FrontFirst;

	public MVCountingCubeDigit FrontSecond;

	public MVCountingCubeDigit BackFirst;

	public MVCountingCubeDigit BackSecond;

	public int Number
	{
		set
		{
			AssignNewNumber(value);
		}
	}

	public void AssignNewNumber(int newValue)
	{
		string text = newValue.ToString();
		if (text.Length < 2)
		{
			text = "0" + text;
		}
		int number = int.Parse(text[0].ToString());
		int number2 = int.Parse(text[1].ToString());
		FrontFirst.Number = number;
		FrontSecond.Number = number2;
		BackFirst.Number = number;
		BackSecond.Number = number2;
	}
}
