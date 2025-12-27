using System;
using System.IO;

namespace DemoAPI.Data
{
    public static class Log
    {
        private static readonly string LogFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "DemoAPI", "audit.log");

      

        // Write a log entry
        public static void Logs(string message)
        {
            // Ensure folder exists
            var directory = Path.GetDirectoryName(LogFilePath);
            if (!Directory.Exists(directory!))
                Directory.CreateDirectory(directory!);

            // Format log entry with readable timestamp
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

            // Append to file
            File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
        }
    }
}
