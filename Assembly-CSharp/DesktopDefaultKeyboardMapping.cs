using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DesktopDefaultKeyboardMapping : IKogamaInputMap
{
	protected Dictionary<KogamaControls, KeyCode[]> keyMapping;

	protected BitArray controlDown = new BitArray(52);

	public DesktopDefaultKeyboardMapping()
	{
		MVGameControllerDesktop.OnApplicationLostFocus = (UnityAction)Delegate.Combine(MVGameControllerDesktop.OnApplicationLostFocus, new UnityAction(OnApplicationLostFocus));
		MVGameControllerDesktop.OnApplicationRegainedFocus = (UnityAction)Delegate.Combine(MVGameControllerDesktop.OnApplicationRegainedFocus, new UnityAction(OnApplicationRegainedFocus));
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
				new KeyCode[1] { KeyCode.V }
			},
			{
				KogamaControls.Holster,
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

	private void OnApplicationLostFocus()
	{
		controlDown.SetAll(value: false);
	}

	private void OnApplicationRegainedFocus()
	{
	}

	public bool GetBooleanControl(KogamaControls control, KeyState keyState, int index = -1)
	{
		KeyCode[] array = keyMapping[control];
		for (int i = 0; i < array.Length; i++)
		{
			if (index != -1 && i != index)
			{
				continue;
			}
			KeyCode key = array[i];
			switch (keyState)
			{
			case KeyState.Pressed:
				if (!controlDown[(int)control] && Input.GetKeyDown(key))
				{
					controlDown[(int)control] = true;
				}
				else if (controlDown[(int)control] && Input.GetKeyUp(key))
				{
					controlDown[(int)control] = false;
				}
				return controlDown[(int)control];
			case KeyState.Down:
			{
				bool keyDown = Input.GetKeyDown(key);
				if (keyDown)
				{
					controlDown[(int)control] = true;
				}
				return keyDown;
			}
			case KeyState.Up:
			{
				bool keyUp = Input.GetKeyUp(key);
				if (keyUp)
				{
					controlDown[(int)control] = false;
				}
				return keyUp;
			}
			}
		}
		return false;
	}
}
