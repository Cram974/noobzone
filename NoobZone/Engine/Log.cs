using System;
using System.IO;

namespace NoobZone
{
    class Log
    {
        public enum MessageType
        {
            ERROR,
            WARNING,
            INFO,
            CUSTOM
        };

        static FileStream fs_logFile;
        static StreamWriter sw_logWriter;

        public static bool Create(string fileName)
        {
            //Creation du fichier de log
            fs_logFile = new FileStream(fileName, FileMode.Create);

            if(fs_logFile == null)
                return false;

            sw_logWriter = new StreamWriter(fs_logFile);
            if (sw_logWriter == null)
                return false;

            return true;
        }
        public static void Destroy()
        {
            if (sw_logWriter != null)
                sw_logWriter.Close();

            sw_logWriter = null;

            if (fs_logFile != null)
                fs_logFile.Close();
            
            fs_logFile = null;
        }

        public static void Message(string message, MessageType T)
        {
            if (fs_logFile != null)
            {
                
                string icon = "";

                switch (T)
                {
                    case MessageType.ERROR:
                        icon = "[X] ";
                        break;
                    case MessageType.INFO:
                        icon = "[?] ";
                        break;
                    case MessageType.WARNING:
                        icon = "[!] ";
                        break;
                    case MessageType.CUSTOM:
                        icon = "";
                        break;
                }

                sw_logWriter.WriteLine(icon + message);
            }
        }
        public static void NewLine()
        {
            sw_logWriter.WriteLine();
        }
    }
}
