using UnityEngine;

public class CleanroomSetup : MonoBehaviour
{
	private void Awake()
	{
		UXInputDispatcher component = GetComponent<UXInputDispatcher>();
		MVInputHandlerPrioritizer component2 = GetComponent<MVInputHandlerPrioritizer>();
		component2.Register(component);
	}

	private void Start()
	{
	}
}
