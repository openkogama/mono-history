public interface IFocusObjectListener
{
	void OnFocusEnter(UXFocusObject focusObject);

	void OnFocusExit(UXFocusObject focusObject);

	void OnNextFocusRequest(UXFocusObject focusObject);

	void OnPreviousFocusRequest(UXFocusObject focusObject);
}
