using System;
using System.IO;

namespace TCPServer.Logger
{
    public class Logger
    {
        private string Class;
        private string LogPath = @"D:\Projects\В процессе\TCPClientServer\Logs\";

        public Logger()
        {

        }
        public Logger(string className)
        {
            Class = className;
        }

        public void SetLocation(string location)
        {
            Class = location;
        }

        //Example
        public void ExampleReport(string report,
                            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
                            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            Console.WriteLine("Report" + ": " + report);
            Console.WriteLine("member name: " + memberName);
            Console.WriteLine("source file path: " + sourceFilePath);
            Console.WriteLine("source line number: " + sourceLineNumber);
        }


        public void InfoReport(string report,
                           [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                           [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            string convertLog = "Info:" + DateTime.Now + ": " + sourceFilePath + ": " + memberName + " ::: " + report;
            Console.WriteLine(convertLog);
            WriteInfoReport(convertLog);
        }
        
        public void ErrorReport(string report,
                           [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                           [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            string convertLog = "Error:" + DateTime.Now + ": " + sourceFilePath + ": " + memberName + " ::: " + report;
            Console.WriteLine(convertLog);
            WriteErrorReport(convertLog);
        }

        private void WriteInfoReport(string logs)
        {
            using (StreamWriter streamWriter = new StreamWriter(LogPath + "logs.txt", true))
            {
                streamWriter.WriteLine(logs);
            }
        }

        private void WriteErrorReport(string logs)
        {
            using (StreamWriter streamWriter = new StreamWriter(LogPath + "errors.txt", true))
            {
                streamWriter.WriteLine(logs);
            }
        }
    }
}
