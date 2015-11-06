using System.Collections.Generic;

public interface ICameraSettings
{
	void UpdateFromCameraSettings(Dictionary<object, object> data);

	void SetDefaultSettings();

	void ScaleCameraValues(float scale);
}
