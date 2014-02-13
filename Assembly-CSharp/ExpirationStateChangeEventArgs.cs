using System;

public class ExpirationStateChangeEventArgs : EventArgs
{
	public readonly ProductExpirationState OldState;

	public readonly ProductExpirationState State;

	public ExpirationStateChangeEventArgs(ProductExpirationState oldState, ProductExpirationState state)
	{
		OldState = oldState;
		State = state;
	}
}
