internal struct PendingPrototypeData(int prevPrototypeId, RuntimePrototypeCubeModel pendingRuntimePrototype)
{
	public int prevPrototypeId = prevPrototypeId;

	public RuntimePrototypeCubeModel pendingRuntimePrototype = pendingRuntimePrototype;
}
