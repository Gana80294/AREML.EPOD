using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Configurations
{
    public class AppSetting
    {
        public string EncryptionKey { get; set; }
        public string PasswordChangeFrequency {  get; set; }
        public string Invoice_Output {  get; set; }
        public string FTP_UserName {  get; set; }
        public string FTP_Password { get; set;}
        public string ReverseAttachmentsPath { get; set; }
        public string SharedFolderUserName {  get; set; }
        public string SharedFolderPassword { get; set;}
        public string SharedFolderDomain { get; set; }
    }
}
