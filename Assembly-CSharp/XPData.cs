public class XPData
{
	public byte XPId { get; set; }

	public int XPAmount { get; set; }

	public XPData()
	{
	}

	public XPData(byte id, int amount)
	{
		XPId = id;
		XPAmount = amount;
	}

	public override string ToString()
	{
		return $"XpId {XPId}. XPAmount {XPAmount}.";
	}
}
