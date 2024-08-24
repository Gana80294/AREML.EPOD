using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Logging
{
    public static class LogWriter
    {
        public static void WriteToFile(string Message, Exception ex = null)
        {
            StreamWriter sw = null;
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorFiles");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DateTime dt = DateTime.Today;
                DateTime ystrdy = DateTime.Today.AddDays(-60);//keep 15 days backup
                string yday = ystrdy.ToString("yyyyMMdd");
                string today = dt.ToString("yyyyMMdd");
                string Log = today + ".txt";
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorFiles\\Log_" + yday + ".txt"))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorFiles\\Log_" + yday + ".txt");
                }
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorFiles\\Log_" + Log, true);
                WriteExceptionMessageInStreamWriter(sw, Message, ex);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }

        }
        public static void WriteToFile(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorFiles");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DateTime dt = DateTime.Today;
                DateTime ystrdy = DateTime.Today.AddDays(-15);//keep 15 days backup
                string yday = ystrdy.ToString("yyyyMMdd");
                string today = dt.ToString("yyyyMMdd");
                string Log = today + ".txt";
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorFiles\\Log_" + yday + ".txt"))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorFiles\\Log_" + yday + ".txt");
                }
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorFiles\\Log_" + Log, true);
                WriteExceptionMessageInStreamWriter(sw, null, ex);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }

        }
        public static void WriteProcessLog(string Message, Exception ex = null)
        {
            StreamWriter sw = null;
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProcessLogFiles");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DateTime dt = DateTime.Today;
                DateTime ystrdy = DateTime.Today.AddDays(-60);//keep 60 days backup
                string yday = ystrdy.ToString("yyyyMMdd");
                string today = dt.ToString("yyyyMMdd");
                string Log = today + ".txt";
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\ProcessLogFiles\\Log_" + yday + ".txt"))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\ProcessLogFiles\\Log_" + yday + ".txt");
                }
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ProcessLogFiles\\Log_" + Log, true);
                sw.WriteLine("***** " + DateTime.Now.ToString() + " *****");
                sw.WriteLine($"Message : {Message}");
                if (ex != null) { WriteExceptionMessageInStreamWriter(sw, Message, ex); }
                sw.WriteLine(String.Concat(Enumerable.Repeat("*", 25)));
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }

        }
        public static void WriteExceptionMessageInStreamWriter(StreamWriter sw, string message = null, Exception ex = null)
        {
            sw.WriteLine(String.Concat(Enumerable.Repeat("*", 10)) + DateTime.Now.ToString() + String.Concat(Enumerable.Repeat("*", 10)));
            if (!string.IsNullOrWhiteSpace(message))
            {
                sw.WriteLine($"Message : {message}");
            }
            if (ex.StackTrace != null)
            {
                sw.WriteLine("Stack Trace : " + ex.StackTrace.ToString());
            }
            sw.WriteLine("Excception : " + ex.Message.ToString().Trim());
            if (ex.InnerException != null && ex.Message.Contains("inner exception"))
            {
                sw.WriteLine("Inner Exception : " + ex.InnerException.Message.ToString().Trim());
            }
            sw.WriteLine(String.Concat(Enumerable.Repeat("*", 25)));
        }
    }
}
