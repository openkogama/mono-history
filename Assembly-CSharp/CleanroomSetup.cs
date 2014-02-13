using UnityEngine;

public class CleanroomSetup : MonoBehaviour
{
	private void Awake()
	{
		UXInputDispatcher component = ((Component)this).GetComponent<UXInputDispatcher>();
		MVInputHandlerPrioritizer component2 = ((Component)this).GetComponent<MVInputHandlerPrioritizer>();
		component2.Register(component);
	}

	private void Start()
	{
	}
}
