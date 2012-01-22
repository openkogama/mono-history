using UnityEngine;

public class AvatarItem : MonoBehaviour
{
	public GameObject itemObject;

	public Avatar owner;

	public virtual int Quantity { get; set; }

	private void Awake()
	{
		((Behaviour)this).enabled = false;
	}

	public virtual void TriggerBegin(int instigatorActorNr)
	{
	}

	public virtual void TriggerEnd()
	{
	}
}
