using System.Collections.Generic;
using System.Text;

namespace MV.WorldObject.AntiCheat;

public class FileData
{
	public List<byte> name;

	public uint crc;

	public string NameAsString()
	{
		return Encoding.UTF8.GetString(name.ToArray());
	}

	public void SetName(string a)
	{
		name = new List<byte>(Encoding.UTF8.GetBytes(a));
	}

	public FileData()
	{
	}

	public FileData(string name, uint crc)
	{
		SetName(name);
		this.crc = crc;
	}

	public FileData(byte[] name, uint crc)
	{
		this.name = new List<byte>(name);
		this.crc = crc;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is FileData fileData))
		{
			return false;
		}
		if (crc == fileData.crc)
		{
			return NameAsString() == fileData.NameAsString();
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return "Dll: " + NameAsString() + " CRC: " + crc;
	}
}
