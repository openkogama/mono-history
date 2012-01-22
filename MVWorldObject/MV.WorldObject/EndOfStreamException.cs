using System;
using System.Runtime.Serialization;

namespace MV.WorldObject;

[Serializable]
public class EndOfStreamException : Exception
{
	public EndOfStreamException()
	{
	}

	public EndOfStreamException(string message)
		: base(message)
	{
	}

	public EndOfStreamException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected EndOfStreamException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
