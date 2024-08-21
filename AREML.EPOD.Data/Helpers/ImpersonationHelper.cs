using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Helpers
{
    public class ImpersonationHelper : IDisposable
    {
        //private WindowsImpersonationContext? _impersonationContext;
        private IntPtr _userHandle;
        public void Dispose()
        {
            //_impersonationContext?.Undo();
            if (_userHandle != IntPtr.Zero)
            {
                CloseHandle(_userHandle);
            }
        }


        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private extern static bool DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private extern static bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);

        public ImpersonationHelper(string username, string domain, string password)
        {
            _userHandle = IntPtr.Zero;

            const int logon32ProviderDefault = 0;
            const int logon32LogonNewCredentials = 9;

            //if (LogonUser(username, domain, password, logon32LogonNewCredentials, logon32ProviderDefault, out _userHandle))
            //{
            //    IntPtr duplicateTokenHandle = IntPtr.Zero;
            //    if (DuplicateToken(_userHandle, 2, ref duplicateTokenHandle))
            //    {
            //        var tempWindowsIdentity = new WindowsIdentity(duplicateTokenHandle);
            //        _impersonationContext = tempWindowsIdentity.Impersonate();

            //        if (_impersonationContext != null)
            //        {
            //            //WriteLog.WriteProcessLog("Impersonation succeeded.");
            //        }
            //        else
            //        {
            //            throw new UnauthorizedAccessException("Impersonation failed.");
            //        }

            //        CloseHandle(duplicateTokenHandle); // Close the duplicated token handle
            //    }
            //    else
            //    {
            //        int ret = Marshal.GetLastWin32Error();
            //        throw new UnauthorizedAccessException($"DuplicateToken failed with error code: {ret}");
            //    }

            //}
            //else
            //{
            //    int ret = Marshal.GetLastWin32Error();
            //    throw new UnauthorizedAccessException($"LogonUser failed with error code: {ret}");
            //}
        }
    }
}
