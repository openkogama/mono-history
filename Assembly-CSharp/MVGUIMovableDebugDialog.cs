using System.Collections;

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
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Pos", positionTextField.Text);
		hashtable.Add("Rot", rotationTextField.Text);
		hashtable.Add("Vel", velocityTextField.Text);
		hashtable.Add("AngVel", angularVelocityTextField.Text);
		hashtable.Add("Dist", distanceTextField.Text);
		hashtable.Add("ParentID", parentIDTextField.Text);
		return hashtable;
	}
}
