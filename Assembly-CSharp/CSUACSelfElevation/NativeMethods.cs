using System;
using System.Runtime.InteropServices;

namespace CSUACSelfElevation;

internal class NativeMethods
{
	public const uint STANDARD_RIGHTS_REQUIRED = 983040u;

	public const uint STANDARD_RIGHTS_READ = 131072u;

	public const uint TOKEN_ASSIGN_PRIMARY = 1u;

	public const uint TOKEN_DUPLICATE = 2u;

	public const uint TOKEN_IMPERSONATE = 4u;

	public const uint TOKEN_QUERY = 8u;

	public const uint TOKEN_QUERY_SOURCE = 16u;

	public const uint TOKEN_ADJUST_PRIVILEGES = 32u;

	public const uint TOKEN_ADJUST_GROUPS = 64u;

	public const uint TOKEN_ADJUST_DEFAULT = 128u;

	public const uint TOKEN_ADJUST_SESSIONID = 256u;

	public const uint TOKEN_READ = 131080u;

	public const uint TOKEN_ALL_ACCESS = 983551u;

	public const int ERROR_INSUFFICIENT_BUFFER = 122;

	public const int SECURITY_MANDATORY_UNTRUSTED_RID = 0;

	public const int SECURITY_MANDATORY_LOW_RID = 4096;

	public const int SECURITY_MANDATORY_MEDIUM_RID = 8192;

	public const int SECURITY_MANDATORY_HIGH_RID = 12288;

	public const int SECURITY_MANDATORY_SYSTEM_RID = 16384;

	public const uint BCM_SETSHIELD = 5644u;

	[DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool OpenProcessToken(IntPtr hProcess, uint desiredAccess, out SafeTokenHandle hToken);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool DuplicateToken(SafeTokenHandle ExistingTokenHandle, SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, out SafeTokenHandle DuplicateTokenHandle);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetTokenInformation(SafeTokenHandle hToken, TOKEN_INFORMATION_CLASS tokenInfoClass, IntPtr pTokenInfo, int tokenInfoLength, out int returnLength);

	[DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr GetSidSubAuthority(IntPtr pSid, uint nSubAuthority);
}
