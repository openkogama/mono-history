using System;
using MV.Common;

public class TakeDamageEventArgs : EventArgs
{
	public float damage;

	public MVPlayer damageSource;

	public PlayerKilledByType damageType;

	public TakeDamageEventArgs(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		damage = amount;
		damageSource = damageDealer;
		this.damageType = damageType;
	}
}
