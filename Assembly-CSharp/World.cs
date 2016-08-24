using System;

public abstract class World
{
	protected MVWorldObjectClientManagerNetwork worldObjectClientManager;

	protected MVWorldInventory worldInventory;

	protected RuntimeEventManagerNetwork runtimeEventManagerNetwork;

	public EventHandler<InitializedGameQueryDataEventArgs> InitializedGameQueryData;

	public MVWorldInventory WorldInventory => worldInventory;

	public MVWorldObjectClientManager WorldObjectClientManager => worldObjectClientManager;

	public RuntimeEventManager RuntimeEventManager => runtimeEventManagerNetwork;

	public abstract void RemoveLink(int linkID);
}
