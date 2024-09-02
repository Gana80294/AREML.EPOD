using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

public class ImpersonationHelper : IDisposable
{
    private IntPtr _userHandle = IntPtr.Zero;
    private IntPtr _duplicateTokenHandle = IntPtr.Zero;
    private WindowsIdentity? _windowsIdentity;

    public void Dispose()
    {
        _windowsIdentity?.Dispose();
        if (_duplicateTokenHandle != IntPtr.Zero)
        {
            CloseHandle(_duplicateTokenHandle);
        }
        if (_userHandle != IntPtr.Zero)
        {
            CloseHandle(_userHandle);
        }
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private extern static bool DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private extern static bool CloseHandle(IntPtr handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool RevertToSelf();

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool SetThreadToken(IntPtr pHandle, IntPtr hToken);

    public ImpersonationHelper(string username, string domain, string password)
    {
        const int logon32ProviderDefault = 0;
        const int logon32LogonNewCredentials = 9;

        if (LogonUser(username, domain, password, logon32LogonNewCredentials, logon32ProviderDefault, out _userHandle))
        {
            if (DuplicateToken(_userHandle, 2, ref _duplicateTokenHandle))
            {
                _windowsIdentity = new WindowsIdentity(_duplicateTokenHandle);
                if (!SetThreadToken(IntPtr.Zero, _duplicateTokenHandle))
                {
                    throw new UnauthorizedAccessException("Failed to impersonate the user.");
                }
            }
            else
            {
                int ret = Marshal.GetLastWin32Error();
                throw new UnauthorizedAccessException($"DuplicateToken failed with error code: {ret}");
            }
        }
        else
        {
            int ret = Marshal.GetLastWin32Error();
            throw new UnauthorizedAccessException($"LogonUser failed with error code: {ret}");
        }
    }
}
