using UnityEngine;

public class MVObjectEnablerObject : ObjectPrefab
{
	[SerializeField]
	private ObjectEnabler objectEnabler;

	public ObjectEnabler ObjectEnabler => objectEnabler;
}
