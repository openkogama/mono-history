using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace CSUACSelfElevation;

internal class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
{
	private SafeTokenHandle()
		: base(ownsHandle: true)
	{
	}

	internal SafeTokenHandle(IntPtr handle)
		: base(ownsHandle: true)
	{
		SetHandle(handle);
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern bool CloseHandle(IntPtr handle);

	protected override bool ReleaseHandle()
	{
		return CloseHandle(handle);
	}
}
