using System.Collections.Generic;

public class MVGUIMovableDebugDialog : UXCustomDialogBox
{
	public UXTextField positionTextField;

	public UXTextField rotationTextField;

	public UXTextField velocityTextField;

	public UXTextField angularVelocityTextField;

	public UXTextField distanceTextField;

	public UXTextField parentIDTextField;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("Pos", positionTextField.Text);
		dictionary.Add("Rot", rotationTextField.Text);
		dictionary.Add("Vel", velocityTextField.Text);
		dictionary.Add("AngVel", angularVelocityTextField.Text);
		dictionary.Add("Dist", distanceTextField.Text);
		dictionary.Add("ParentID", parentIDTextField.Text);
		return dictionary;
	}
}
