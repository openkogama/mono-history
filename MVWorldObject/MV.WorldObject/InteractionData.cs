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
			new InteractionData(InteractionPackageType.CenterGun, 11.5f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		},
		{
			InteractionPackageType.MutantHit,
			new InteractionData(InteractionPackageType.MutantHit, 110f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		},
		{
			InteractionPackageType.RailGunHit,
			new InteractionData(InteractionPackageType.RailGunHit, 100f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		},
		{
			InteractionPackageType.ShotgunHit,
			new InteractionData(InteractionPackageType.ShotgunHit, 13f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		},
		{
			InteractionPackageType.SixShooterHit,
			new InteractionData(InteractionPackageType.SixShooterHit, 12.5f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		},
		{
			InteractionPackageType.DoubleSixShooterHit,
			new InteractionData(InteractionPackageType.DoubleSixShooterHit, 12.5f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		},
		{
			InteractionPackageType.SwordHit,
			new InteractionData(InteractionPackageType.SwordHit, 15f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		},
		{
			InteractionPackageType.ThrowingStarHit,
			new InteractionData(InteractionPackageType.ThrowingStarHit, 15f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		},
		{
			InteractionPackageType.MultiThrowingStarHit,
			new InteractionData(InteractionPackageType.MultiThrowingStarHit, 12f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		},
		{
			InteractionPackageType.SlapGunHit,
			new InteractionData(InteractionPackageType.SlapGunHit, 25f, Vector3.zero, PlayerKilledByType.None, isShared: true)
		}
	};

	private float damage;

	private Vector3 impulse;

	private InteractionPackageType interactionType;

	private PlayerKilledByType playerKilledByType;

	public float Damage => damage;

	public Vector3 Impulse => impulse;

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
	}

	public InteractionData(InteractionPackageType interactionType, float damage)
		: this(interactionType, damage, Vector3.zero, PlayerKilledByType.None)
	{
	}

	public InteractionData(InteractionPackageType interactionType, Vector3 impulse)
		: this(interactionType, 0f, impulse, PlayerKilledByType.None)
	{
	}

	public InteractionData(InteractionPackageType interactionType, float damage, Vector3 impulse)
		: this(interactionType, damage, impulse, PlayerKilledByType.None)
	{
	}

	public InteractionData(InteractionPackageType interactionType, PlayerKilledByType playerKilledByType)
		: this(interactionType, 0f, Vector3.zero, playerKilledByType)
	{
	}

	public InteractionData(InteractionPackageType interactionType, float damage, PlayerKilledByType playerKilledByType)
		: this(interactionType, damage, Vector3.zero, playerKilledByType)
	{
	}

	public InteractionData(InteractionPackageType interactionType, Vector3 impulse, PlayerKilledByType playerKilledByType)
		: this(interactionType, 0f, impulse, playerKilledByType)
	{
	}

	public InteractionData(InteractionPackageType interactionType, float damage, Vector3 impulse, PlayerKilledByType playerKilledByType)
	{
		this.interactionType = interactionType;
		InteractionData sharedData = GetSharedData(interactionType);
		Validate(sharedData, interactionType, damage, impulse, playerKilledByType);
		this.damage = (ValidateFloat(damage) ? damage : 0f);
		this.impulse = (ValidateVector3(impulse) ? impulse : Vector3.zero);
		this.playerKilledByType = playerKilledByType;
		if (damage == 0f)
		{
			this.damage = sharedData.damage;
		}
		if (impulse.sqrMagnitude <= 1E-05f)
		{
			this.impulse = sharedData.impulse;
		}
		if (playerKilledByType == PlayerKilledByType.None)
		{
			this.playerKilledByType = sharedData.playerKilledByType;
		}
	}

	private InteractionData(InteractionPackageType interactionType, float damage, Vector3 impulse, PlayerKilledByType playerKilledByType, bool isShared)
	{
		this.interactionType = interactionType;
		this.damage = (ValidateFloat(damage) ? damage : 0f);
		this.impulse = (ValidateVector3(impulse) ? impulse : Vector3.zero);
		this.playerKilledByType = playerKilledByType;
	}

	private static void Validate(InteractionData sharedInteractionData, InteractionPackageType interactionType, float damage, Vector3 impulse, PlayerKilledByType playerKilledByType)
	{
		if (damage != 0f && sharedInteractionData.damage != 0f)
		{
			throw new Exception("Both sharedValues.damage and construction argument damage defined for type: " + interactionType);
		}
		if (impulse.sqrMagnitude > 1E-05f && sharedInteractionData.impulse.sqrMagnitude > 1E-05f)
		{
			throw new Exception("Both sharedValues.impulse and construction argument impulse defined for type: " + interactionType);
		}
		if (playerKilledByType != PlayerKilledByType.None && sharedInteractionData.playerKilledByType != PlayerKilledByType.None)
		{
			throw new Exception("Both sharedValues.playerKilledByType and construction argument playerKilledByType defined for type: " + interactionType);
		}
	}

	public InteractionData(byte[] byteArray)
	{
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
		else
		{
			damage = sharedData.damage;
		}
		if ((byteFlags & ByteFlags.Impulse) != 0)
		{
			impulse = new Vector3(bytePacker.ReadSingle(), bytePacker.ReadSingle(), bytePacker.ReadSingle());
		}
		else
		{
			impulse = sharedData.impulse;
		}
		if ((byteFlags & ByteFlags.PlayerKilledByType) != 0)
		{
			playerKilledByType = (PlayerKilledByType)bytePacker.ReadByte();
		}
		else
		{
			playerKilledByType = sharedData.playerKilledByType;
		}
	}

	private static bool ValidateVector3(Vector3 validateVector)
	{
		if (!ValidateFloat(validateVector.x) || !ValidateFloat(validateVector.y) || !ValidateFloat(validateVector.z))
		{
			return false;
		}
		return true;
	}

	private static bool ValidateFloat(float validateFloat)
	{
		if (float.IsInfinity(validateFloat) || float.IsNaN(validateFloat))
		{
			return false;
		}
		return true;
	}

	public byte[] ToByteArray()
	{
		ByteFlags byteFlags = (ByteFlags)0;
		BytePacker bytePacker = new BytePacker();
		InteractionData sharedData = GetSharedData(interactionType);
		bytePacker.Write((byte)byteFlags);
		if (interactionType != InteractionPackageType.None)
		{
			bytePacker.Write((byte)interactionType);
			byteFlags |= ByteFlags.InteractionType;
		}
		if (damage != 0f && sharedData.damage == 0f)
		{
			bytePacker.Write(damage);
			byteFlags |= ByteFlags.Damage;
		}
		if (impulse.sqrMagnitude > 1E-05f && sharedData.impulse.sqrMagnitude <= 1E-05f)
		{
			bytePacker.Write(impulse.x);
			bytePacker.Write(impulse.y);
			bytePacker.Write(impulse.z);
			byteFlags |= ByteFlags.Impulse;
		}
		if (playerKilledByType != PlayerKilledByType.None && sharedData.playerKilledByType == PlayerKilledByType.None)
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
		return $"damage: {damage}, AvatarPackageType: {interactionType}, impulse {impulse}, playerKilledByType {playerKilledByType}";
	}
}
