using System;

public interface ISelectionController
{
	event EventHandler<WorldObjectDestroyedEventArgs> SelectedWorldObjectDeleted;
}
