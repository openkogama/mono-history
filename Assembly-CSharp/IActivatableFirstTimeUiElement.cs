using MV.WorldObject.MetaData;

public interface IActivatableFirstTimeUiElement
{
	FirstTimeEvent FirstTimeEvent { get; }

	int Priority { get; }

	bool CanShow { get; }

	bool IsShowing { get; }

	bool IsRegistered { get; }

	FirstTimeEvent PrerequisiteEvent { get; }

	void Show();
}
