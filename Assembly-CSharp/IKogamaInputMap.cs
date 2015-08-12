using System;
using UnityEngine;

internal interface IKogamaInputMap
{
	bool GetBooleanControl(KogamaControls control, Func<KeyCode, bool> inputFun, int index = -1);
}
