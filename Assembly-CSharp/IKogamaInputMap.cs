internal interface IKogamaInputMap
{
	bool GetBooleanControl(KogamaControls control, KeyState keyState);

	void ForceReleaseAllKeys();
}
