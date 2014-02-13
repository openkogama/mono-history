using System;

public abstract class World
{
	protected MVWorldObjectClientManagerNetwork worldObjectClientManager;

	protected MVWorldInventory worldInventory;

	public EventHandler<InitializedGameQueryDataEventArgs> InitializedGameQueryData;

	public MVWorldInventory WorldInventory => worldInventory;

	public MVWorldObjectClientManager WorldObjectClientManager => worldObjectClientManager;
}
