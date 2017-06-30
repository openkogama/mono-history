using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using CSUACSelfElevation;
using MV.Common;
using UnityEngine;

public class SelfElevator
{
	public enum ApplicationIntegrityLevel
	{
		Untrusted,
		Low,
		Medium,
		High,
		System,
		Unknown
	}

	private enum Bools
	{
		inAdminGroupSet,
		inAdminGroup,
		isRunningAsAdminSet,
		isRunningAsAdmin,
		isElevatedSet,
		isElevated,
		Size
	}

	private static BitArray bits = new BitArray(6);

	private static ApplicationIntegrityLevel integrityLevel = ApplicationIntegrityLevel.Unknown;

	private Dictionary<int, ApplicationIntegrityLevel> RID_to_ApplicationIntegrityLevel = new Dictionary<int, ApplicationIntegrityLevel>
	{
		{
			0,
			ApplicationIntegrityLevel.Untrusted
		},
		{
			4096,
			ApplicationIntegrityLevel.Low
		},
		{
			8192,
			ApplicationIntegrityLevel.Medium
		},
		{
			12288,
			ApplicationIntegrityLevel.High
		},
		{
			16384,
			ApplicationIntegrityLevel.System
		}
	};

	public static bool InAdminGroupSet => bits[0];

	public static bool InAdminGroup
	{
		get
		{
			return bits[1];
		}
		set
		{
			bits[1] = value;
			bits[0] = true;
		}
	}

	public static bool IsRunningAsAdminSet => bits[2];

	public static bool IsRunningAsAdmin
	{
		get
		{
			return bits[3];
		}
		set
		{
			bits[3] = value;
			bits[2] = true;
		}
	}

	public static bool IsElevatedSet => bits[4];

	public static bool IsElevated
	{
		get
		{
			return bits[5];
		}
		set
		{
			bits[5] = value;
			bits[4] = true;
		}
	}

	public static ApplicationIntegrityLevel IntegrityLevel
	{
		get
		{
			return integrityLevel;
		}
		set
		{
			integrityLevel = value;
		}
	}

	public SelfElevator()
	{
		try
		{
			InAdminGroup = IsUserInAdminGroup();
		}
		catch (Exception)
		{
			UnityEngine.Debug.LogError("Unable to determine if user is in admin group.");
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Unable to determine if user is in admin group.");
		}
		try
		{
			IsRunningAsAdmin = IsRunAsAdmin();
		}
		catch (Exception)
		{
			UnityEngine.Debug.LogError("Unable to determine wether process is run as admin.");
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Unable to determine wether process is run as admin.");
		}
		if (Environment.OSVersion.Version.Major >= 6)
		{
			try
			{
				IsElevated = IsProcessElevated();
			}
			catch (Exception)
			{
				UnityEngine.Debug.LogError("Unable to determine wether process is elevated.");
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Unable to determine wether process is elevated.");
			}
			try
			{
				int processIntegrityLevel = GetProcessIntegrityLevel();
				IntegrityLevel = RID_to_ApplicationIntegrityLevel[processIntegrityLevel];
				return;
			}
			catch (Exception)
			{
				UnityEngine.Debug.LogError("Unable to determine process integrity level.");
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Unable to determine process integrity level.");
				return;
			}
		}
		UnityEngine.Debug.LogError("OS version is to old to make use of SelfElevator");
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "OS version is to old to make use of SelfElevator");
	}

	internal bool IsUserInAdminGroup()
	{
		bool flag = false;
		SafeTokenHandle hToken = null;
		SafeTokenHandle DuplicateTokenHandle = null;
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = IntPtr.Zero;
		int num = 0;
		try
		{
			if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle, 10u, out hToken))
			{
				throw new Win32Exception();
			}
			if (Environment.OSVersion.Version.Major >= 6)
			{
				num = 4;
				intPtr = Marshal.AllocHGlobal(num);
				if (intPtr == IntPtr.Zero)
				{
					throw new Win32Exception();
				}
				if (!NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenElevationType, intPtr, num, out num))
				{
					throw new Win32Exception();
				}
				TOKEN_ELEVATION_TYPE tOKEN_ELEVATION_TYPE = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(intPtr);
				if (tOKEN_ELEVATION_TYPE == TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
				{
					num = IntPtr.Size;
					intPtr2 = Marshal.AllocHGlobal(num);
					if (intPtr2 == IntPtr.Zero)
					{
						throw new Win32Exception();
					}
					if (!NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenLinkedToken, intPtr2, num, out num))
					{
						throw new Win32Exception();
					}
					IntPtr handle = Marshal.ReadIntPtr(intPtr2);
					DuplicateTokenHandle = new SafeTokenHandle(handle);
				}
			}
			if (DuplicateTokenHandle == null && !NativeMethods.DuplicateToken(hToken, SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, out DuplicateTokenHandle))
			{
				throw new Win32Exception();
			}
			WindowsIdentity ntIdentity = new WindowsIdentity(DuplicateTokenHandle.DangerousGetHandle());
			WindowsPrincipal windowsPrincipal = new WindowsPrincipal(ntIdentity);
			return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
		}
		finally
		{
			if (hToken != null)
			{
				hToken.Close();
				hToken = null;
			}
			if (DuplicateTokenHandle != null)
			{
				DuplicateTokenHandle.Close();
				DuplicateTokenHandle = null;
			}
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
			}
			if (intPtr2 != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr2);
				intPtr2 = IntPtr.Zero;
			}
		}
	}

	internal bool IsRunAsAdmin()
	{
		WindowsIdentity current = WindowsIdentity.GetCurrent();
		WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
		return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
	}

	internal bool IsProcessElevated()
	{
		bool flag = false;
		SafeTokenHandle hToken = null;
		int num = 0;
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle, 8u, out hToken))
			{
				throw new Win32Exception();
			}
			num = Marshal.SizeOf(typeof(TOKEN_ELEVATION));
			intPtr = Marshal.AllocHGlobal(num);
			if (intPtr == IntPtr.Zero)
			{
				throw new Win32Exception();
			}
			if (!NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenElevation, intPtr, num, out num))
			{
				throw new Win32Exception();
			}
			return ((TOKEN_ELEVATION)Marshal.PtrToStructure(intPtr, typeof(TOKEN_ELEVATION))).TokenIsElevated != 0;
		}
		finally
		{
			if (hToken != null)
			{
				hToken.Close();
				hToken = null;
			}
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				num = 0;
			}
		}
	}

	internal int GetProcessIntegrityLevel()
	{
		int num = -1;
		SafeTokenHandle hToken = null;
		int returnLength = 0;
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle, 8u, out hToken))
			{
				throw new Win32Exception();
			}
			if (!NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, 0, out returnLength))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 122)
				{
					throw new Win32Exception(lastWin32Error);
				}
			}
			intPtr = Marshal.AllocHGlobal(returnLength);
			if (intPtr == IntPtr.Zero)
			{
				throw new Win32Exception();
			}
			if (!NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, intPtr, returnLength, out returnLength))
			{
				throw new Win32Exception();
			}
			IntPtr sidSubAuthority = NativeMethods.GetSidSubAuthority(((TOKEN_MANDATORY_LABEL)Marshal.PtrToStructure(intPtr, typeof(TOKEN_MANDATORY_LABEL))).Label.Sid, 0u);
			return Marshal.ReadInt32(sidSubAuthority);
		}
		finally
		{
			if (hToken != null)
			{
				hToken.Close();
				hToken = null;
			}
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				returnLength = 0;
			}
		}
	}

	private string GetExecutableName()
	{
		string fileName = Process.GetCurrentProcess().MainModule.FileName;
		return fileName.Substring(fileName.LastIndexOf('\\'));
	}

	public void Elevate()
	{
		if (!IsRunAsAdmin())
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.UseShellExecute = true;
			processStartInfo.WorkingDirectory = Environment.CurrentDirectory;
			processStartInfo.FileName = GetExecutableName();
			processStartInfo.Verb = "runas";
			try
			{
				Process.Start(processStartInfo);
			}
			catch
			{
				StatHatWrapper.Count("User actively refused elevation.", 1);
				UnityEngine.Debug.Log("User actively refused elevation.");
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "User actively refused elevation.");
				return;
			}
			Application.Quit();
		}
		else
		{
			UnityEngine.Debug.Log("The process is already running as administrator");
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "The process is already running as administrator");
		}
	}
}
