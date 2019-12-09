using System;
using System.IO;

namespace DemandingJumps
{
    public class Logger
    {
        static string filePath = $"{DemandingJumps.ModDirectory}/DemandingJumps.log";
        public static void LogError(Exception ex)
        {
            if (DemandingJumps.DebugLevel >= 1)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    var prefix = "[DemandingJumps @ " + DateTime.Now.ToString() + "]";
                    writer.WriteLine("Message: " + ex.Message + "<br/>" + Environment.NewLine + "StackTrace: " + ex.StackTrace + "" + Environment.NewLine);
                    writer.WriteLine("----------------------------------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
        }

        public static void LogLine(String line, bool showPrefix = true)
        {
            if (DemandingJumps.DebugLevel >= 2)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    string prefix = "";
                    if (showPrefix)
                    {
                        prefix = "[DemandingJumps @ " + DateTime.Now.ToString() + "]";
                    }
                    writer.WriteLine(prefix + line);
                }
            }
        }
    }
}
