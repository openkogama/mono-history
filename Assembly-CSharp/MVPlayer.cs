using System.Collections;

public class MVPlayer
{
	public int ProfileID { get; set; }

	public int ActorNr { get; set; }

	public string Username { get; set; }

	public string Password { get; set; }

	public int PlanetOwnershipTypeID { get; set; }

	public MVAvatar Avatar { get; set; }

	public Hashtable InitAvatarStatus { get; set; }
}
