using System;
using Assets.CodeStage.AntiCheatToolkit.Scripts.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public struct ObscuredQuaternion
{
	[Serializable]
	public struct RawEncryptedQuaternion
	{
		public int x;

		public int y;

		public int z;

		public int w;
	}

	private static int cryptoKey = CryptoKeyGenerator.GenerateKey(int.MinValue, int.MaxValue);

	private static readonly Quaternion initialFakeValue = Quaternion.identity;

	[SerializeField]
	private int currentCryptoKey;

	[SerializeField]
	private RawEncryptedQuaternion hiddenValue;

	[SerializeField]
	private Quaternion fakeValue;

	[SerializeField]
	private bool inited;

	private ObscuredQuaternion(RawEncryptedQuaternion value)
	{
		currentCryptoKey = cryptoKey;
		hiddenValue = value;
		fakeValue = initialFakeValue;
		inited = true;
	}

	public ObscuredQuaternion(float x, float y, float z, float w)
	{
		currentCryptoKey = cryptoKey;
		hiddenValue = Encrypt(x, y, z, w, currentCryptoKey);
		fakeValue.x = x;
		fakeValue.y = y;
		fakeValue.z = z;
		fakeValue.w = w;
		inited = true;
	}

	public static void SetNewCryptoKey(int newKey)
	{
		cryptoKey = newKey;
	}

	public static RawEncryptedQuaternion Encrypt(Quaternion value)
	{
		return Encrypt(value, 0);
	}

	public static RawEncryptedQuaternion Encrypt(Quaternion value, int key)
	{
		return Encrypt(value.x, value.y, value.z, value.w, key);
	}

	public static RawEncryptedQuaternion Encrypt(float x, float y, float z, float w, int key)
	{
		if (key == 0)
		{
			key = cryptoKey;
		}
		RawEncryptedQuaternion result = default;
		result.x = ObscuredFloat.Encrypt(x, key);
		result.y = ObscuredFloat.Encrypt(y, key);
		result.z = ObscuredFloat.Encrypt(z, key);
		result.w = ObscuredFloat.Encrypt(w, key);
		return result;
	}

	public static Quaternion Decrypt(RawEncryptedQuaternion value)
	{
		return Decrypt(value, 0);
	}

	public static Quaternion Decrypt(RawEncryptedQuaternion value, int key)
	{
		if (key == 0)
		{
			key = cryptoKey;
		}
		Quaternion result = default;
		result.x = ObscuredFloat.Decrypt(value.x, key);
		result.y = ObscuredFloat.Decrypt(value.y, key);
		result.z = ObscuredFloat.Decrypt(value.z, key);
		result.w = ObscuredFloat.Decrypt(value.w, key);
		return result;
	}

	public void ApplyNewCryptoKey()
	{
		if (currentCryptoKey != cryptoKey)
		{
			hiddenValue = Encrypt(InternalDecrypt(), cryptoKey);
			currentCryptoKey = cryptoKey;
		}
	}

	public void RandomizeCryptoKey()
	{
		Quaternion value = InternalDecrypt();
		do
		{
			currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		}
		while (currentCryptoKey == 0);
		hiddenValue = Encrypt(value, currentCryptoKey);
	}

	public RawEncryptedQuaternion GetEncrypted()
	{
		ApplyNewCryptoKey();
		return hiddenValue;
	}

	public void SetEncrypted(RawEncryptedQuaternion encrypted)
	{
		inited = true;
		hiddenValue = encrypted;
		if (ObscuredCheatingDetector.IsRunning)
		{
			fakeValue = InternalDecrypt();
		}
	}

	public Quaternion GetDecrypted()
	{
		return InternalDecrypt();
	}

	private Quaternion InternalDecrypt()
	{
		if (!inited)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = Encrypt(initialFakeValue);
			fakeValue = initialFakeValue;
			inited = true;
		}
		Quaternion quaternion = default;
		quaternion.x = ObscuredFloat.Decrypt(hiddenValue.x, currentCryptoKey);
		quaternion.y = ObscuredFloat.Decrypt(hiddenValue.y, currentCryptoKey);
		quaternion.z = ObscuredFloat.Decrypt(hiddenValue.z, currentCryptoKey);
		quaternion.w = ObscuredFloat.Decrypt(hiddenValue.w, currentCryptoKey);
		if (ObscuredCheatingDetector.IsRunning && !fakeValue.Equals(initialFakeValue) && !CompareQuaternionsWithTolerance(quaternion, fakeValue))
		{
			ObscuredCheatingDetector.Instance.OnCheatingDetected();
		}
		return quaternion;
	}

	private bool CompareQuaternionsWithTolerance(Quaternion q1, Quaternion q2)
	{
		float quaternionEpsilon = ObscuredCheatingDetector.Instance.quaternionEpsilon;
		return Math.Abs(q1.x - q2.x) < quaternionEpsilon && Math.Abs(q1.y - q2.y) < quaternionEpsilon && Math.Abs(q1.z - q2.z) < quaternionEpsilon && Math.Abs(q1.w - q2.w) < quaternionEpsilon;
	}

	public static implicit operator ObscuredQuaternion(Quaternion value)
	{
		ObscuredQuaternion result = new ObscuredQuaternion(Encrypt(value));
		if (ObscuredCheatingDetector.IsRunning)
		{
			result.fakeValue = value;
		}
		return result;
	}

	public static implicit operator Quaternion(ObscuredQuaternion value)
	{
		return value.InternalDecrypt();
	}

	public override int GetHashCode()
	{
		return InternalDecrypt().GetHashCode();
	}

	public override string ToString()
	{
		return InternalDecrypt().ToString();
	}

	public string ToString(string format)
	{
		return InternalDecrypt().ToString(format);
	}
}
