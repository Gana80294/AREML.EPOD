using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Helpers
{
    public class NetworkFileHelper
    {

        private readonly string _username;
        private readonly string _password;
        private readonly string _domain;

        public NetworkFileHelper(IConfiguration configuration)
        {
            _username = configuration["NetworkCredentials:SharedFolderUserName"];
            _password = configuration["NetworkCredentials:SharedFolderPassword"];
            _domain = configuration["NetworkCredentials:SharedFolderDomain"];
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);


        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceType ResourceType;
            public string ResourceName;
            public string Provider;
            public string RemoteName;
        }

        public enum ResourceType
        {
            Disk = 1,
            Print = 2,
            Reserved = 8,
            Unknown = int.MaxValue,
        }

        public void ConnectToNetworkShare(string networkPath)
        {
            var netResource = new NetResource
            {
                ResourceType = ResourceType.Disk,
                RemoteName = networkPath,
                Provider = null
            };

            int result = WNetAddConnection2(netResource, _password, $"{_domain}\\{_username}", 0);
            if (result != 0)
            {
                throw new System.ComponentModel.Win32Exception(result);
            }
        }

        public void DisconnectFromNetworkShare(string networkPath)
        {
            WNetCancelConnection2(networkPath, 0, true);
        }

        public void SaveFile(string filename, byte[] content, string networkPath)
        {
            try
            {
                string networkShare = Path.GetPathRoot(networkPath);
                ConnectToNetworkShare(networkShare);
                string fullPath = Path.Combine(networkPath, filename);
                File.WriteAllBytes(fullPath, content);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving file: {ex.Message}");
            }
            finally
            {
                DisconnectFromNetworkShare(networkPath);
            }
        }

        public byte[] ReadFile(string completeNetworkPath)
        {
            try
            {
                string networkShare = Path.GetPathRoot(completeNetworkPath);
                ConnectToNetworkShare(networkShare);
                return File.ReadAllBytes(completeNetworkPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading file: {ex.Message}");
            }
            finally
            {
                DisconnectFromNetworkShare(completeNetworkPath);
            }
        }
    }
}
