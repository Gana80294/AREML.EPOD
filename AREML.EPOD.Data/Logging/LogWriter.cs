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
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DateTime dt = DateTime.Today;
                DateTime ystrdy = DateTime.Today.AddDays(-60);//keep 15 days backup
                string yday = ystrdy.ToString("yyyyMMdd");
                string today = dt.ToString("yyyyMMdd");
                string Log = today + ".txt";
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\Log_" + yday + ".txt"))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\Log_" + yday + ".txt");
                }
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\Log_" + Log, true);
                sw.WriteLine(string.Format(DateTime.Now.ToString()) + ":" + Message);
                if (ex != null && ex.Message.Contains("inner exception") && ex.InnerException != null)
                {
                    sw.WriteLine($"{DateTime.Now.ToString()} : Inner :- {ex.InnerException.Message}");
                }
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
                DateTime ystrdy = DateTime.Today.AddDays(-60);//keep 15 days backup
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
                sw.WriteLine(string.Format(DateTime.Now.ToString()) + ":" + Message);
                WriteInnerExceptionDetails(sw, ex);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }

        }
        public static void WriteInnerExceptionDetails(StreamWriter sw, Exception ex)
        {
            if (ex != null && ex.Message.Contains("inner exception") && ex.InnerException != null)
            {
                sw.WriteLine($"{DateTime.Now.ToString()} : Inner :- {ex.InnerException.Message}");
                WriteInnerExceptionDetails(sw, ex.InnerException);
            }
            else
            {
                return;
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
                sw.WriteLine("***** " + DateTime.Now.ToString() + " *****");
                sw.WriteLine(ex.StackTrace != null ? "Stack Trace : " + ex.StackTrace.ToString() : "Stack Trace : null");
                sw.WriteLine("Excception : " + ex.Message.ToString().Trim());
                sw.WriteLine(ex.InnerException != null ? "Inner Exception : " + ex.InnerException.Message.ToString().Trim() : "Inner Exception : null");
                sw.WriteLine(String.Concat(Enumerable.Repeat("*", 25)));
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }

        }
    
    
    
        public static void WriteSensitiveLog(string query)
        {
            StreamWriter sw = null;
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QueryLogs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DateTime dt = DateTime.Today;
                DateTime ystrdy = DateTime.Today.AddDays(-15);//keep 15 days backup
                string yday = ystrdy.ToString("yyyyMMdd");
                string today = dt.ToString("yyyyMMdd");
                string Log = today + ".txt";
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\QueryLogs\\Log_" + yday + ".txt"))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\QueryLogs\\Log_" + yday + ".txt");
                }
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\QueryLogs\\Log_" + Log, true);
                sw.WriteLine("***** " + DateTime.Now.ToString() + " *****");
                sw.WriteLine($"Executed SQL Query: {query}");
                sw.WriteLine(String.Concat(Enumerable.Repeat("*", 25)));
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }
    }
}
