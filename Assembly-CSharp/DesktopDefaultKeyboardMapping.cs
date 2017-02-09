using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DesktopDefaultKeyboardMapping : IKogamaInputMap
{
	protected Dictionary<KogamaControls, KeyCode[]> keyMapping;

	private HashSet<KeyCode> currentKeyDownStates = new HashSet<KeyCode>();

	public DesktopDefaultKeyboardMapping()
	{
		keyMapping = new Dictionary<KogamaControls, KeyCode[]>
		{
			{
				KogamaControls.EditMoveUp,
				new KeyCode[1] { KeyCode.E }
			},
			{
				KogamaControls.EditMoveDown,
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
				KogamaControls.Holster,
				new KeyCode[1] { KeyCode.V }
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
			},
			{
				KogamaControls.Escape,
				new KeyCode[1] { KeyCode.Escape }
			},
			{
				KogamaControls.EditMoveFast,
				new KeyCode[2]
				{
					KeyCode.LeftShift,
					KeyCode.RightShift
				}
			},
			{
				KogamaControls.EditMoveForward,
				new KeyCode[2]
				{
					KeyCode.W,
					KeyCode.UpArrow
				}
			},
			{
				KogamaControls.EditMoveLeft,
				new KeyCode[2]
				{
					KeyCode.A,
					KeyCode.LeftArrow
				}
			},
			{
				KogamaControls.EditMoveRight,
				new KeyCode[2]
				{
					KeyCode.D,
					KeyCode.RightArrow
				}
			},
			{
				KogamaControls.EditMoveBackwards,
				new KeyCode[2]
				{
					KeyCode.S,
					KeyCode.DownArrow
				}
			},
			{
				KogamaControls.NotificationAcceptFriendshipRequest,
				new KeyCode[1] { KeyCode.R }
			}
		};
	}

	public virtual bool GetBooleanControl(KogamaControls control, KeyState keyState, int index = -1)
	{
		Func<KeyCode, bool> func = null;
		switch (keyState)
		{
		case KeyState.Pressed:
			func = Input.GetKey;
			break;
		case KeyState.Down:
			func = Input.GetKeyDown;
			break;
		case KeyState.Up:
			func = Input.GetKeyUp;
			break;
		}
		bool result = false;
		if (!keyMapping.Keys.Contains(control))
		{
			Debug.Log("Trying to retreive an unmapped control for " + control);
			return false;
		}
		int num = 0;
		KeyCode[] array = keyMapping[control];
		foreach (KeyCode keyCode in array)
		{
			if (func == new Func<KeyCode, bool>(Input.GetKeyUp) && currentKeyDownStates.Contains(keyCode) && MVInputWrapper.hasLostFocus)
			{
				currentKeyDownStates.Remove(keyCode);
				result = true;
			}
			if (func(keyCode) && (num == index || index == -1))
			{
				if (func == new Func<KeyCode, bool>(Input.GetKeyUp))
				{
					if (currentKeyDownStates.Contains(keyCode))
					{
						currentKeyDownStates.Remove(keyCode);
					}
				}
				else if (func == new Func<KeyCode, bool>(Input.GetKeyDown))
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
