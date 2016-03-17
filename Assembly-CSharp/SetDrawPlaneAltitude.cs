using UnityEngine;

public class SetDrawPlaneAltitude : MonoBehaviour
{
	public void Up()
	{
		DrawPlane.MoveDrawPlane(1);
	}

	public void Down()
	{
		DrawPlane.MoveDrawPlane(-1);
	}
}
