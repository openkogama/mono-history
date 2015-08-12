using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultKeyboardMapping : IKogamaInputMap
{
	protected Dictionary<KogamaControls, KeyCode[]> KeyMapping;

	private HashSet<KeyCode> currentKeyDownStates = new HashSet<KeyCode>();

	public DefaultKeyboardMapping()
	{
		KeyMapping = new Dictionary<KogamaControls, KeyCode[]>
		{
			{
				KogamaControls.MoveForward,
				new KeyCode[2]
				{
					KeyCode.W,
					KeyCode.UpArrow
				}
			},
			{
				KogamaControls.MoveLeft,
				new KeyCode[2]
				{
					KeyCode.A,
					KeyCode.LeftArrow
				}
			},
			{
				KogamaControls.MoveRight,
				new KeyCode[2]
				{
					KeyCode.D,
					KeyCode.RightArrow
				}
			},
			{
				KogamaControls.MoveBackwards,
				new KeyCode[2]
				{
					KeyCode.S,
					KeyCode.DownArrow
				}
			},
			{
				KogamaControls.AlternateCameraControls,
				new KeyCode[1] { KeyCode.LeftAlt }
			},
			{
				KogamaControls.MoveUp,
				new KeyCode[1] { KeyCode.E }
			},
			{
				KogamaControls.MoveDown,
				new KeyCode[1] { KeyCode.C }
			},
			{
				KogamaControls.PointerSelect,
				new KeyCode[1] { KeyCode.Mouse0 }
			},
			{
				KogamaControls.PointerSelectAlt,
				new KeyCode[1] { KeyCode.Mouse1 }
			},
			{
				KogamaControls.EnterObject,
				new KeyCode[1] { KeyCode.Return }
			},
			{
				KogamaControls.DeleteObject,
				new KeyCode[1] { KeyCode.Delete }
			},
			{
				KogamaControls.LeaveObject,
				new KeyCode[1] { KeyCode.Escape }
			},
			{
				KogamaControls.AddToSelection,
				new KeyCode[2]
				{
					KeyCode.LeftShift,
					KeyCode.RightShift
				}
			},
			{
				KogamaControls.MoveDrawPlaneUp,
				new KeyCode[1] { KeyCode.PageUp }
			},
			{
				KogamaControls.MoveDrawPlaneDown,
				new KeyCode[1] { KeyCode.PageDown }
			},
			{
				KogamaControls.EmbedChangeGame,
				new KeyCode[4]
				{
					KeyCode.Alpha1,
					KeyCode.Alpha2,
					KeyCode.Alpha3,
					KeyCode.Alpha4
				}
			},
			{
				KogamaControls.ToggleFullScreen,
				new KeyCode[1] { KeyCode.O }
			},
			{
				KogamaControls.ToggleHD,
				new KeyCode[1] { KeyCode.H }
			},
			{
				KogamaControls.ShowChat,
				new KeyCode[2]
				{
					KeyCode.T,
					KeyCode.Return
				}
			},
			{
				KogamaControls.Respawn,
				new KeyCode[1] { KeyCode.K }
			},
			{
				KogamaControls.TogglePlayerParticles,
				new KeyCode[1] { KeyCode.Y }
			},
			{
				KogamaControls.ShowPlayerWindow,
				new KeyCode[1] { KeyCode.Tab }
			},
			{
				KogamaControls.DropCurrentItem,
				new KeyCode[1] { KeyCode.Q }
			},
			{
				KogamaControls.Use,
				new KeyCode[1] { KeyCode.E }
			},
			{
				KogamaControls.FocusOnSelectedModel,
				new KeyCode[1] { KeyCode.V }
			},
			{
				KogamaControls.TogglePlayInEditor,
				new KeyCode[1] { KeyCode.P }
			},
			{
				KogamaControls.ToggleLogicRendering,
				new KeyCode[1] { KeyCode.L }
			},
			{
				KogamaControls.ToggleGripdSnapSize,
				new KeyCode[1] { KeyCode.G }
			},
			{
				KogamaControls.ActivateEditCubeTool,
				new KeyCode[1] { KeyCode.Alpha1 }
			},
			{
				KogamaControls.ActivateDeleteCubeTool,
				new KeyCode[1] { KeyCode.Alpha2 }
			},
			{
				KogamaControls.ActivetaPaintCubeTool,
				new KeyCode[1] { KeyCode.Alpha3 }
			},
			{
				KogamaControls.ChangeMaterial,
				new KeyCode[1] { KeyCode.R }
			},
			{
				KogamaControls.OpenInventory,
				new KeyCode[1] { KeyCode.I }
			},
			{
				KogamaControls.CreateNewModel,
				new KeyCode[1] { KeyCode.N }
			},
			{
				KogamaControls.ToggleDrawPlane,
				new KeyCode[1] { KeyCode.F }
			},
			{
				KogamaControls.Run,
				new KeyCode[2]
				{
					KeyCode.LeftShift,
					KeyCode.RightShift
				}
			},
			{
				KogamaControls.Jump,
				new KeyCode[2]
				{
					KeyCode.Space,
					KeyCode.Keypad0
				}
			},
			{
				KogamaControls.Fire,
				new KeyCode[1] { KeyCode.Mouse0 }
			},
			{
				KogamaControls.DrawAudioBox,
				new KeyCode[9]
				{
					KeyCode.Alpha1,
					KeyCode.Alpha2,
					KeyCode.Alpha3,
					KeyCode.Alpha4,
					KeyCode.Alpha5,
					KeyCode.Alpha6,
					KeyCode.Alpha7,
					KeyCode.Alpha8,
					KeyCode.Alpha9
				}
			},
			{
				KogamaControls.ChatSendLine,
				new KeyCode[2]
				{
					KeyCode.Return,
					KeyCode.KeypadEnter
				}
			},
			{
				KogamaControls.ChatShiftLineDown,
				new KeyCode[1] { KeyCode.DownArrow }
			},
			{
				KogamaControls.ChatShiftLineUp,
				new KeyCode[1] { KeyCode.UpArrow }
			},
			{
				KogamaControls.ChangeFocus,
				new KeyCode[1] { KeyCode.Tab }
			},
			{
				KogamaControls.ChangeChangeFocusDirection,
				new KeyCode[2]
				{
					KeyCode.LeftShift,
					KeyCode.RightShift
				}
			},
			{
				KogamaControls.LobbyMenu,
				new KeyCode[2]
				{
					KeyCode.M,
					KeyCode.Escape
				}
			}
		};
	}

	public bool GetBooleanControl(KogamaControls control, Func<KeyCode, bool> inputFun, int index = -1)
	{
		bool result = false;
		if (!KeyMapping.Keys.Contains(control))
		{
			Debug.Log("Trying to retreive an unmapped control for " + control);
			return false;
		}
		int num = 0;
		KeyCode[] array = KeyMapping[control];
		foreach (KeyCode keyCode in array)
		{
			if (inputFun == new Func<KeyCode, bool>(Input.GetKeyUp) && currentKeyDownStates.Contains(keyCode) && MVInputWrapper.hasLostFocus)
			{
				currentKeyDownStates.Remove(keyCode);
				result = true;
			}
			if (inputFun(keyCode) && (num == index || index == -1))
			{
				if (inputFun == new Func<KeyCode, bool>(Input.GetKeyUp))
				{
					if (currentKeyDownStates.Contains(keyCode))
					{
						currentKeyDownStates.Remove(keyCode);
					}
				}
				else if (inputFun == new Func<KeyCode, bool>(Input.GetKeyDown))
				{
					currentKeyDownStates.Add(keyCode);
				}
				result = true;
				break;
			}
			num++;
		}
		return result;
	}
}
