using MV.Common;

namespace MV.WorldObject.RuntimeEvents;

public class SingleCubeFineGrainedEvent : RuntimeEvent
{
	private readonly byte material;

	public byte Material => material;

	public override byte[] Data
	{
		get
		{
			BytePacker bytePacker = new BytePacker();
			bytePacker.Write((byte)RuntimeEventType);
			if (RuntimeEventType == RuntimeEventType.FineGrainedSingleCubeAdd)
			{
				bytePacker.Write(material);
			}
			bytePacker.Write(position.x);
			bytePacker.Write(position.y);
			bytePacker.Write(position.z);
			return bytePacker.ToArray();
		}
	}

	public void OverrideRuntimeType(RuntimeEventType runtimeEventType)
	{
		RuntimeEventType = runtimeEventType;
	}

	public SingleCubeFineGrainedEvent(IntVector position, byte material)
	{
		RuntimeEventType = RuntimeEventType.FineGrainedSingleCubeAdd;
		base.position = position;
		this.material = material;
	}

	public SingleCubeFineGrainedEvent(IntVector position)
	{
		RuntimeEventType = RuntimeEventType.FineGrainedSingleCubeRemove;
		base.position = position;
	}

	public SingleCubeFineGrainedEvent(RuntimeEventType runtimeEventType, byte[] data)
		: this(runtimeEventType, new BytePacker(data))
	{
	}

	public SingleCubeFineGrainedEvent(RuntimeEventType runtimeEventType, BytePacker bp)
	{
		RuntimeEventType = runtimeEventType;
		if (runtimeEventType == RuntimeEventType.FineGrainedSingleCubeAdd)
		{
			material = bp.ReadByte();
		}
		position = new IntVector(bp.ReadInt16(), bp.ReadInt16(), bp.ReadInt16());
	}

	public override string ToString()
	{
		return $"RuntimeEventType: {RuntimeEventType}, Position: {position}, Material {material}";
	}
}
