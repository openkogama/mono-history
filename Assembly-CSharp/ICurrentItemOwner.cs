using System.Collections.Generic;

public interface ICurrentItemOwner
{
	Dictionary<object, object> GetCurrentItemState();

	void SetCurrentItemState(Dictionary<object, object> aNewState);
}
