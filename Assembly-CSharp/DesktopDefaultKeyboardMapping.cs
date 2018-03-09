using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DesktopDefaultKeyboardMapping : IKogamaInputMap
{
	protected class ControlBitArray
	{
		private BitArray controlDown = new BitArray(52);

		public bool this[KogamaControls ctrl]
		{
			get
			{
				return controlDown[(int)ctrl];
			}
			set
			{
				controlDown[(int)ctrl] = value;
			}
		}

		public void Reset()
		{
			controlDown.SetAll(value: false);
		}
	}

	protected Dictionary<KogamaControls, KeyCode[]> keyMapping;

	protected ControlBitArray controlDown = new ControlBitArray();

	public DesktopDefaultKeyboardMapping()
	{
		MVGameControllerDesktop.OnApplicationLostFocus = (UnityAction)Delegate.Combine(MVGameControllerDesktop.OnApplicationLostFocus, new UnityAction(Reset));
		keyMapping = new Dictionary<KogamaControls, KeyCode[]>
		{
			{
				KogamaControls.EditMoveUp,
				new KeyCode[2]
				{
					KeyCode.E,
					KeyCode.Space
				}
			},
			{
				KogamaControls.EditMoveDown,
				new KeyCode[2]
				{
					KeyCode.C,
					KeyCode.LeftControl
				}
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

	public void Reset()
	{
		controlDown.Reset();
	}

	public bool GetBooleanControl(KogamaControls control, KeyState keyState)
	{
		bool flag = KeyDown(control);
		if (flag)
		{
			controlDown[control] = true;
		}
		bool result = controlDown[control];
		bool flag2 = KeyUp(control);
		if (flag2)
		{
			controlDown[control] = false;
		}
		switch (keyState)
		{
		case KeyState.Pressed:
			return result;
		case KeyState.Down:
			return flag;
		case KeyState.Up:
			return flag2;
		default:
			Debug.LogError("Unexpected key state.");
			return false;
		}
	}

	private bool KeyDown(KogamaControls control)
	{
		KeyCode[] array = keyMapping[control];
		foreach (KeyCode key in array)
		{
			if (Input.GetKeyDown(key))
			{
				return true;
			}
		}
		return false;
	}

	private bool KeyUp(KogamaControls control)
	{
		bool result = false;
		KeyCode[] array = keyMapping[control];
		foreach (KeyCode key in array)
		{
			bool flag = (controlDown[control] && !Input.GetKey(key)) || Input.GetKeyUp(key);
			if (!flag && controlDown[control])
			{
				return false;
			}
			if (flag)
			{
				result = true;
			}
		}
		return result;
	}
}
