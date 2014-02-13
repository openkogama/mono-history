using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

namespace MV.WorldObject;

public struct InteractionData
{
	[Flags]
	private enum ByteFlags : byte
	{
		InteractionType = 1,
		Damage = 2,
		Impulse = 4,
		PlayerKilledByType = 8
	}

	private static Dictionary<InteractionPackageType, InteractionData> sharedStaticValues = new Dictionary<InteractionPackageType, InteractionData>
	{
		{
			InteractionPackageType.CenterGun,
			new InteractionData(InteractionPackageType.CenterGun, 13f)
		},
		{
			InteractionPackageType.RailGunHit,
			new InteractionData(InteractionPackageType.RailGunHit, 100f)
		},
		{
			InteractionPackageType.MutantHit,
			new InteractionData(InteractionPackageType.MutantHit, 110f)
		}
	};

	private float damage;

	private Vector3 impulse;

	private InteractionPackageType interactionType;

	private PlayerKilledByType playerKilledByType;

	public float Damage => damage;

	public Vector3 Impulse
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return impulse;
		}
	}

	public InteractionPackageType InteractionType => interactionType;

	public PlayerKilledByType PlayerKilledByType => playerKilledByType;

	private static InteractionData GetSharedData(InteractionPackageType interactionType)
	{
		if (!sharedStaticValues.ContainsKey(interactionType))
		{
			return default;
		}
		return sharedStaticValues[interactionType];
	}

	public InteractionData(InteractionPackageType interactionType)
		: this(interactionType, 0f, Vector3.zero, PlayerKilledByType.None)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
	}

	public InteractionData(InteractionPackageType interactionType, float damage)
		: this(interactionType, damage, Vector3.zero, PlayerKilledByType.None)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
	}

	public InteractionData(InteractionPackageType interactionType, Vector3 impulse)
		: this(interactionType, 0f, impulse, PlayerKilledByType.None)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
	}

	public InteractionData(InteractionPackageType interactionType, float damage, Vector3 impulse)
		: this(interactionType, damage, impulse, PlayerKilledByType.None)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
	}

	public InteractionData(InteractionPackageType interactionType, PlayerKilledByType playerKilledByType)
		: this(interactionType, 0f, Vector3.zero, playerKilledByType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
	}

	public InteractionData(InteractionPackageType interactionType, float damage, PlayerKilledByType playerKilledByType)
		: this(interactionType, damage, Vector3.zero, playerKilledByType)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
	}

	public InteractionData(InteractionPackageType interactionType, Vector3 impulse, PlayerKilledByType playerKilledByType)
		: this(interactionType, 0f, impulse, playerKilledByType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
	}

	public InteractionData(InteractionPackageType interactionType, float damage, Vector3 impulse, PlayerKilledByType playerKilledByType)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		this.damage = damage;
		this.interactionType = interactionType;
		this.impulse = impulse;
		this.playerKilledByType = playerKilledByType;
	}

	public InteractionData(byte[] byteArray, bool withSharedValues)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		damage = 0f;
		interactionType = InteractionPackageType.None;
		impulse = Vector3.zero;
		playerKilledByType = PlayerKilledByType.None;
		BytePacker bytePacker = new BytePacker(byteArray);
		ByteFlags byteFlags = (ByteFlags)bytePacker.ReadByte();
		if ((byteFlags & ByteFlags.InteractionType) != 0)
		{
			interactionType = (InteractionPackageType)bytePacker.ReadByte();
		}
		InteractionData sharedData = GetSharedData(interactionType);
		if ((byteFlags & ByteFlags.Damage) != 0)
		{
			damage = bytePacker.ReadSingle();
		}
		else if (withSharedValues)
		{
			damage = sharedData.damage;
		}
		if ((byteFlags & ByteFlags.Impulse) != 0)
		{
			impulse = new Vector3(bytePacker.ReadSingle(), bytePacker.ReadSingle(), bytePacker.ReadSingle());
		}
		else if (withSharedValues)
		{
			impulse = sharedData.impulse;
		}
		if ((byteFlags & ByteFlags.PlayerKilledByType) != 0)
		{
			playerKilledByType = (PlayerKilledByType)bytePacker.ReadByte();
		}
		else if (withSharedValues)
		{
			playerKilledByType = sharedData.playerKilledByType;
		}
	}

	public byte[] ToByteArray()
	{
		ByteFlags byteFlags = (ByteFlags)0;
		BytePacker bytePacker = new BytePacker();
		bytePacker.Write((byte)byteFlags);
		if (interactionType != InteractionPackageType.None)
		{
			bytePacker.Write((byte)interactionType);
			byteFlags |= ByteFlags.InteractionType;
		}
		if (damage != 0f)
		{
			bytePacker.Write(damage);
			byteFlags |= ByteFlags.Damage;
		}
		if (impulse.sqrMagnitude > 1E-05f)
		{
			bytePacker.Write(impulse.x);
			bytePacker.Write(impulse.y);
			bytePacker.Write(impulse.z);
			byteFlags |= ByteFlags.Impulse;
		}
		if (playerKilledByType != PlayerKilledByType.None)
		{
			bytePacker.Write((byte)playerKilledByType);
			byteFlags |= ByteFlags.PlayerKilledByType;
		}
		byte[] array = bytePacker.ToArray();
		array[0] = (byte)byteFlags;
		return array;
	}

	public override string ToString()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		return string.Format("damage: {0}, AvatarPackageType: {1}, impulse {2}, playerKilledByType {3}", new object[4] { damage, interactionType, impulse, playerKilledByType });
	}
}
